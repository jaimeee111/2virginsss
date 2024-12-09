using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class ChunkManager : MonoBehaviour
{
    [Header("Probabilidades de decoración (porcentaje)")]
    [Range(0, 100)] public float chanceOfTrees = 10; // Porcentaje de árboles (10%)
    [Range(0, 100)] public float chanceOfRocks = 5;  // Porcentaje de rocas (5%)
    [Range(0, 100)] public float chanceOfWater = 2;  // Porcentaje de agua (2%)

    [Header("Distancias mínimas")]
    public float minDistanceBetweenDecorations = 1.0f; // Mínima entre decoraciones generales
    public float minDistanceForLakes = 1.5f;           // Mínima para lagos
    public float minLakeToTreeOrRock = 2.0f;           // Lago respecto a árboles/rocas

    [Header("Prefabs y configuración")]
    public GameObject tilePrefab;
    public GameObject treePrefab;
    public GameObject rockPrefab;
    public GameObject lakePrefab;
    public Transform player;

    [Header("Chunk Settings")]
    public int chunkSize = 10;
    public int visibleChunks = 2;

    private float normalizedChanceOfTrees;
    private float normalizedChanceOfRocks;
    private float normalizedChanceOfWater;

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
        normalizedChanceOfTrees = chanceOfTrees / 100f;
        normalizedChanceOfRocks = chanceOfRocks / 100f;
        normalizedChanceOfWater = chanceOfWater / 100f;
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
        float chance = Random.Range(0f, 1f);
        string decorationType = null;

        if (chance < normalizedChanceOfWater)
        {
            decorationType = "Lake";
        }
        else if (chance < normalizedChanceOfWater + normalizedChanceOfRocks)
        {
            decorationType = "Rock";
        }
        else if (chance < normalizedChanceOfWater + normalizedChanceOfRocks + normalizedChanceOfTrees)
        {
            decorationType = "Tree";
        }

        if (decorationType != null)
        {
            float minDistance = decorationType == "Lake" ? minDistanceForLakes : minDistanceBetweenDecorations;

            float prefabSize = Mathf.Max(
                lakePrefab.GetComponent<Renderer>().bounds.extents.magnitude,
                treePrefab.GetComponent<Renderer>().bounds.extents.magnitude
            );

            minDistance = Mathf.Max(minDistance, prefabSize);

            foreach (var data in chunk.DecorationData)
            {
                if (decorationType == "Lake" && (data.Type == "Tree" || data.Type == "Rock"))
                {
                    if (Vector3.Distance(data.Position, position) < minLakeToTreeOrRock)
                        return;
                }
                else if (Vector3.Distance(data.Position, position) < minDistance)
                    return;
            }

            chunk.DecorationData.Add(new DecorationData { Position = position, Type = decorationType });
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
            GameObject decoration = Instantiate(prefab, position, Quaternion.identity);
            chunk.Decorations.Add(decoration);
        }
    }

    private void OnDrawGizmos()
    {
        if (activeChunks == null) return;

        foreach (var chunk in activeChunks.Values)
        {
            foreach (var decoration in chunk.DecorationData)
            {
                Gizmos.color = decoration.Type == "Lake" ? Color.blue : Color.green;
                Gizmos.DrawWireSphere(decoration.Position, minLakeToTreeOrRock);
            }
        }
    }
}