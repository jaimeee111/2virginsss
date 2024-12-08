using System.Linq;
using UnityEngine;
using System.Collections.Generic; // Needed for Lists and Dictionaries

public class ChunkManager : MonoBehaviour
{

     [Range(0, 100)] // Esto limita los valores en el editor entre 0 y 100
    public float chanceOfTrees = 10; // Porcentaje de árboles (10%)
    [Range(0, 100)]
    public float chanceOfRocks = 5;  // Porcentaje de rocas (5%)
    [Range(0, 100)]
    public float chanceOfWater = 2;  // Porcentaje de agua (2%)

    // Valores normalizados para cálculos internos
    private float normalizedChanceOfTrees;
    private float normalizedChanceOfRocks;
    private float normalizedChanceOfWater;

    //The different spawnnable sprites
    public GameObject tilePrefab;   // The basic ground prefab
    public GameObject treePrefab;  // Tree prefab
    public GameObject rockPrefab;  // Rock prefab
    public GameObject lakePrefab;  // Lake prefab

    public Transform player;       // Reference to the player

    public int chunkSize = 10;     // Size of a chunk in tiles
    public int visibleChunks = 2;  // Number of chunks around the player

    private Dictionary<Vector2, Chunk> activeChunks = new Dictionary<Vector2, Chunk>(); // Active chunks
    private HashSet<Vector2> loadedChunks = new HashSet<Vector2>(); // To track already-loaded chunks

    private Vector2 GetChunkCoord(Vector3 position)
{
    return new Vector2(
        Mathf.Floor(position.x / chunkSize),
        Mathf.Floor(position.y / chunkSize)
    );
}

public class Chunk
{
    public Vector2 ChunkCoord; // Chunk coordinate
    public List<GameObject> Tiles = new List<GameObject>(); // Tiles in the chunk
    public List<DecorationData> DecorationData = new List<DecorationData>(); // Decorations in the chunk
    public List<GameObject> Decorations = new List<GameObject>(); // Active decorations in the chunk

    // Helper method to count decorations by type
    public int CountDecorationType(string type)
    {
        return DecorationData.Count(data => data.Type == type);
    }
}

public class DecorationData
{
    public Vector3 Position;
    public string Type;
}

void Start()
    {
        // Convertimos los valores de porcentaje en números entre 0 y 1
        normalizedChanceOfTrees = chanceOfTrees / 100f;
        normalizedChanceOfRocks = chanceOfRocks / 100f;
        normalizedChanceOfWater = chanceOfWater / 100f;
    }


    void Update()
{
    Vector2 playerChunk = GetChunkCoord(player.position);

    // Load all chunks within the visible range
    for (int x = -visibleChunks; x <= visibleChunks; x++)
    {
        for (int y = -visibleChunks; y <= visibleChunks; y++)
        {
            Vector2 chunkCoord = new Vector2(playerChunk.x + x, playerChunk.y + y);
            if (!loadedChunks.Contains(chunkCoord))
            {
                LoadChunk(chunkCoord);
            }
        }
    }

    // Unload chunks outside the visible range
    foreach (var chunkCoord in new List<Vector2>(loadedChunks))
    {
        if (Mathf.Abs(chunkCoord.x - playerChunk.x) > visibleChunks ||
            Mathf.Abs(chunkCoord.y - playerChunk.y) > visibleChunks)
        {
            UnloadChunk(chunkCoord);
        }
    }

    void LoadChunk(Vector2 chunkCoord)
{
    if (loadedChunks.Contains(chunkCoord)) return;

    Chunk newChunk;

    if (activeChunks.TryGetValue(chunkCoord, out newChunk))
    {
        // Log details about the decorations in the chunk
        Debug.Log($"Restoring chunk at {chunkCoord}");
        Debug.Log($"Trees: {newChunk.CountDecorationType("Tree")}, Rocks: {newChunk.CountDecorationType("Rock")}, Lakes: {newChunk.CountDecorationType("Lake")}");
    }
    else
    {
        newChunk = new Chunk { ChunkCoord = chunkCoord };
        Debug.Log($"Generating new chunk at {chunkCoord}");
    }

    Vector3 chunkWorldPos = new Vector3(chunkCoord.x * chunkSize, chunkCoord.y * chunkSize, 0);

    // Generate tiles
    for (int x = 0; x < chunkSize; x++)
    {
        for (int y = 0; y < chunkSize; y++)
        {
            Vector3 tilePos = chunkWorldPos + new Vector3(x, y, 0);
            GameObject tile = Instantiate(tilePrefab, tilePos, Quaternion.identity);
            newChunk.Tiles.Add(tile);
        }
    }

    // Restore or generate decorations
    if (newChunk.DecorationData.Count > 0)
    {
        foreach (var data in newChunk.DecorationData)
        {
            InstantiateDecoration(data.Position, data.Type, newChunk);
        }
    }
    else
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector3 tilePos = chunkWorldPos + new Vector3(x, y, 0);
                GenerateDecoration(tilePos, newChunk);
            }
        }
    }

    activeChunks[chunkCoord] = newChunk;
    loadedChunks.Add(chunkCoord);
}


    void UnloadChunk(Vector2 chunkCoord)
{
    if (!loadedChunks.Contains(chunkCoord)) return;

    if (activeChunks.TryGetValue(chunkCoord, out Chunk chunk))
    {
        // Destroy all tile GameObjects
        foreach (GameObject tile in chunk.Tiles)
        {
            Destroy(tile);
        }

        // Destroy all decoration GameObjects
        foreach (GameObject decoration in chunk.Decorations)
        {
            Destroy(decoration);
        }

        // Keep decoration data intact but clear the GameObject references
        chunk.Tiles.Clear();
        chunk.Decorations.Clear();

        activeChunks.Remove(chunkCoord);
    }

    loadedChunks.Remove(chunkCoord);
}

void GenerateDecoration(Vector3 position, Chunk chunk)
    {
        float chance = Random.Range(0f, 1f);
        string decorationType = null;

        if (chance < normalizedChanceOfWater)
        {
            decorationType = "Lake"; // Lago
        }
        else if (chance < normalizedChanceOfWater + normalizedChanceOfRocks)
        {
            decorationType = "Rock"; // Roca
        }
        else if (chance < normalizedChanceOfWater + normalizedChanceOfRocks + normalizedChanceOfTrees)
        {
            decorationType = "Tree"; // Árbol
        }

        if (decorationType != null)
    {
        // Asegúrate de que no haya un objeto ya en esa posición
        if (chunk.DecorationData.Any(data => Vector3.Distance(data.Position, position) < 0.5f))
        {
            return; // Si hay una decoración cerca, no colocamos nada aquí
        }

        // Reglas específicas para lagos
        if (decorationType == "Lake")
        {
            // Asegúrate de que no haya árboles o rocas cerca
            if (chunk.DecorationData.Any(data => 
                (data.Type == "Tree" || data.Type == "Rock") && 
                Vector3.Distance(data.Position, position) < 1.5f)) // Distancia mínima
            {
                return; // Si hay árboles o rocas cerca, no colocamos el lago
            }
        }

        // Guardar los datos de la decoración
        chunk.DecorationData.Add(new DecorationData
        {
            Position = position,
            Type = decorationType
        });

        // Instanciar la decoración
        InstantiateDecoration(position, decorationType, chunk);
    }
    }

void InstantiateDecoration(Vector3 position, string decorationType, Chunk chunk)
{
    GameObject prefab = null;

    switch (decorationType)
    {
        case "Tree":
            prefab = treePrefab;
            break;
        case "Rock":
            prefab = rockPrefab;
            break;
        case "Lake":
            prefab = lakePrefab;
            break;
    }

    if (prefab != null)
    {
        GameObject decoration = Instantiate(prefab, position + new Vector3(0, 0, -0.1f), Quaternion.identity);

        // Configurar sorting order para simular profundidad
        SpriteRenderer renderer = decoration.GetComponent<SpriteRenderer>();
        var collisionScript = decoration.GetComponent<DecorationCollision>();
        if (collisionScript != null)
        {
            collisionScript.isPassable = (decorationType != "Rock"); // Example: only rocks are impassable
        }
        if (renderer != null)
        {
            // Caso 1: Árboles con profundidad
            if (decorationType == "Tree")
            {
                renderer.sortingOrder = Mathf.RoundToInt(-position.y * 10); // Cuanto más abajo, mayor el sortingOrder
            }
            // Caso 2: Rocas
            else if (decorationType == "Rock")
            {
                renderer.sortingOrder = Mathf.RoundToInt(-position.y * 10 - 1); // Rocas detrás de árboles en igualdad de posición
            }
            // Caso 3: Lagos
            else if (decorationType == "Lake")
            {
                renderer.sortingOrder = Mathf.RoundToInt(-position.y * 10 - 2); // Lagos detrás de árboles y rocas
            }
        }

        chunk.Decorations.Add(decoration); // Agregar la decoración a la lista activa del chunk
    }
}

}

}

