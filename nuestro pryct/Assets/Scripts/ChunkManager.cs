using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class ChunkManager : MonoBehaviour
{
    [Header("Probabilidades de decoración (porcentaje)")]
    public float chanceOfTrees = 0.5f;  // 50% probabilidad de árbol
    public float chanceOfRocks = 0.3f;  // 30% probabilidad de roca
    public float chanceOfWater = 0.2f;  // 20% probabilidad de lago

    [Header("Distancias mínimas")]
    public float minDistanceBetweenDecorations = 1.0f; // Mínima entre decoraciones generales
    public float minDistanceForLakes = 1.5f;           // Mínima para lagos
    public float minLakeToTreeOrRock = 2.0f;           // Lago respecto a árboles/rocas

    [Header("Escala de decoración")]
    [Range(0.1f, 0.5f)] public float minScale = 0.8f; // Escala mínima
    [Range(0.1f, 0.5f)] public float maxScale = 1.5f; // Escala máxima

    [Header("Prefabs y configuración")]
    public GameObject tilePrefab;
    public GameObject treePrefab;
    public GameObject rockPrefab;
    public GameObject lakePrefab;
    public Transform player;

    [Header("Chunk Settings")]
    public int chunkSize = 10;
    public int visibleChunks = 2;

    private Dictionary<Vector2, Chunk> activeChunks = new Dictionary<Vector2, Chunk>();
    private HashSet<Vector2> loadedChunks = new HashSet<Vector2>();

    private Vector2 GetChunkCoord(Vector3 position)
    {
        return new Vector2(
            Mathf.Floor(position.x / chunkSize),
            Mathf.Floor(position.y / chunkSize)
        );
    }

    public class Chunk
    {
        public Vector2 ChunkCoord;
        public List<GameObject> Tiles = new List<GameObject>();
        public List<DecorationData> DecorationData = new List<DecorationData>();
        public List<GameObject> Decorations = new List<GameObject>();
    }

    public class DecorationData
    {
        public Vector3 Position;
        public string Type;
    }

    void Start()
    {
        // Opcional: validación de rangos
        if (minScale > maxScale)
        {
            Debug.LogWarning("minScale no puede ser mayor que maxScale. Ajustando valores.");
            minScale = maxScale;
        }
    }

    void Update()
    {
        Vector2 playerChunk = GetChunkCoord(player.position);

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

        foreach (var chunkCoord in new List<Vector2>(loadedChunks))
        {
            if (Mathf.Abs(chunkCoord.x - playerChunk.x) > visibleChunks ||
                Mathf.Abs(chunkCoord.y - playerChunk.y) > visibleChunks)
            {
                UnloadChunk(chunkCoord);
            }
        }
    }

    void LoadChunk(Vector2 chunkCoord)
    {
        if (loadedChunks.Contains(chunkCoord)) return;

        Chunk newChunk = new Chunk { ChunkCoord = chunkCoord };

        Vector3 chunkWorldPos = new Vector3(chunkCoord.x * chunkSize, chunkCoord.y * chunkSize, 0);

        // Crear tiles
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector3 tilePos = chunkWorldPos + new Vector3(x, y, 0);
                GameObject tile = Instantiate(tilePrefab, tilePos, Quaternion.identity);
                newChunk.Tiles.Add(tile);
            }
        }

        // Crear decoraciones
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector3 tilePos = chunkWorldPos + new Vector3(x, y, 0);
                GenerateDecoration(tilePos, newChunk);
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
            foreach (GameObject tile in chunk.Tiles)
            {
                Destroy(tile);
            }

            foreach (GameObject decoration in chunk.Decorations)
            {
                Destroy(decoration);
            }

            chunk.Tiles.Clear();
            chunk.Decorations.Clear();

            activeChunks.Remove(chunkCoord);
        }

        loadedChunks.Remove(chunkCoord);
    }



    void GenerateDecoration(Vector3 position, Chunk chunk)
    {
        float chance = Random.Range(0f, 1f); // Genera un número aleatorio entre 0 y 1
        string decorationType = null;

        // Asegúrate de que las probabilidades sumen 1
        if (chance < chanceOfTrees)
        {
            decorationType = "Tree"; // Genera un árbol
        }
        else if (chance < chanceOfTrees + chanceOfRocks)
        {
            decorationType = "Rock"; // Genera una roca
        }
        else
        {
            decorationType = "Lake"; // Genera un lago
        }

        if (decorationType != null)
        {
            // Verifica la distancia mínima (como antes)
            float minDistance = decorationType == "Lake" ? minDistanceForLakes : minDistanceBetweenDecorations;

            bool isTooClose = chunk.DecorationData.Any(data =>
            {
                float distance = Vector3.Distance(data.Position, position);

                // Usa distancia especial para lagos frente a otros tipos
                if (decorationType == "Lake" && (data.Type == "Tree" || data.Type == "Rock"))
                {
                    return distance < minLakeToTreeOrRock;
                }

                // Distancia mínima general
                return distance < minDistance;
            });

            if (isTooClose)
            {
                return; // Si está muy cerca de otra decoración, no generes nada
            }

            // Si pasó todas las verificaciones, agrega la decoración
            chunk.DecorationData.Add(new DecorationData
            {
                Position = position,
                Type = decorationType
            });

            InstantiateDecoration(position, decorationType, chunk);
        }
    }


    void InstantiateDecoration(Vector3 position, string decorationType, Chunk chunk)
    {
        GameObject prefab = null;

        switch (decorationType)
        {
            case "Tree": prefab = treePrefab; break;
            case "Rock": prefab = rockPrefab; break;
            case "Lake": prefab = lakePrefab; break;
        }

        if (prefab != null)
        {
            GameObject decoration = Instantiate(prefab, position + new Vector3(0, 0, -0.1f), Quaternion.identity);

            // Aplicar escala aleatoria
            float randomScale = Random.Range(minScale, maxScale);
            decoration.transform.localScale = Vector3.one * randomScale;

            chunk.Decorations.Add(decoration);
        }
    }
}
