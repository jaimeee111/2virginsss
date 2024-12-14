using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public GameObject[] decorationPrefabs; // Prefabs para árboles, rocas, lagos, etc.
    public GameObject groundTilePrefab; // Prefab para los tiles del suelo

    public float[] decorationWeights; // Pesos de probabilidad para decoraciones
    public int chunkSize = 10; // Tamaño del chunk en tiles
    public int visibleChunks = 2; // Cantidad de chunks visibles alrededor del jugador
    public int groundTileSize = 1; // Tamaño de cada tile de suelo

    private Transform player; // Referencia al jugador
    private Dictionary<Vector2, GameObject> activeChunks = new Dictionary<Vector2, GameObject>();

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (decorationPrefabs.Length != decorationWeights.Length)
        {
            Debug.LogError("La cantidad de decoraciones y pesos no coinciden.");
            return;
        }

        UpdateChunks();
    }

    void Update()
    {
        UpdateChunks();
    }

    private void UpdateChunks()
    {
        Vector2 currentChunkCoord = GetChunkCoord(player.position);

        HashSet<Vector2> chunksToKeep = new HashSet<Vector2>();

        for (int xOffset = -visibleChunks; xOffset <= visibleChunks; xOffset++)
        {
            for (int yOffset = -visibleChunks; yOffset <= visibleChunks; yOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoord.x + xOffset, currentChunkCoord.y + yOffset);
                chunksToKeep.Add(viewedChunkCoord);

                if (!activeChunks.ContainsKey(viewedChunkCoord))
                {
                    CreateChunk(viewedChunkCoord);
                }
            }
        }

        List<Vector2> chunksToRemove = new List<Vector2>();
        foreach (Vector2 coord in activeChunks.Keys)
        {
            if (!chunksToKeep.Contains(coord))
            {
                chunksToRemove.Add(coord);
            }
        }

        foreach (Vector2 coord in chunksToRemove)
        {
            Destroy(activeChunks[coord]);
            activeChunks.Remove(coord);
        }
    }

    private void CreateChunk(Vector2 chunkCoord)
    {
        GameObject chunk = new GameObject($"Chunk_{chunkCoord}");
        chunk.transform.position = new Vector3(chunkCoord.x * chunkSize, chunkCoord.y * chunkSize, 0);
        activeChunks[chunkCoord] = chunk;

        GenerateGroundTiles(chunk, chunkCoord);
        GenerateDecorations(chunk, chunkCoord);
    }

    private void GenerateGroundTiles(GameObject chunk, Vector2 chunkCoord)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector3 tilePosition = new Vector3(
                    chunkCoord.x * chunkSize + x * groundTileSize,
                    chunkCoord.y * chunkSize + y * groundTileSize,
                    0
                );

                GameObject tile = Instantiate(groundTilePrefab, tilePosition, Quaternion.identity);
                tile.transform.SetParent(chunk.transform);
            }
        }
    }

    private void GenerateDecorations(GameObject chunk, Vector2 chunkCoord)
    {
        HashSet<Vector2> occupiedPositions = new HashSet<Vector2>();

        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                Vector2 gridPosition = new Vector2(i, j);

                // Verifica si la posición ya está ocupada
                if (occupiedPositions.Contains(gridPosition)) continue;

                if (Random.Range(0f, 1f) > 0.8f) // Genera decoraciones en un 20% de las posiciones
                {
                    Vector3 decorationPosition = new Vector3(
                        chunkCoord.x * chunkSize + i * groundTileSize + Random.Range(-0.4f, 0.4f),
                        chunkCoord.y * chunkSize + j * groundTileSize + Random.Range(-0.4f, 0.4f),
                        0
                    );

                    GameObject selectedDecoration = GetWeightedRandomDecoration();

                    // Si el objeto tiene una base amplia, reserva las posiciones cercanas
                    float baseSize = 1f; // Ajusta según el tamaño de la base del objeto
                    ReservePositions(occupiedPositions, gridPosition, baseSize);

                    GameObject decoration = Instantiate(selectedDecoration, decorationPosition, Quaternion.identity);
                    decoration.transform.SetParent(chunk.transform);
                }
            }
        }
    }

    private void ReservePositions(HashSet<Vector2> occupiedPositions, Vector2 center, float baseSize)
    {
        int range = Mathf.CeilToInt(baseSize);

        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector2 offset = new Vector2(x, y);
                occupiedPositions.Add(center + offset);
            }
        }
    }

    private Vector2 GetChunkCoord(Vector3 position)
    {
        return new Vector2(
            Mathf.Floor(position.x / chunkSize),
            Mathf.Floor(position.y / chunkSize)
        );
    }

    private GameObject GetWeightedRandomDecoration()
    {
        float totalWeight = 0f;

        foreach (float weight in decorationWeights)
        {
            totalWeight += weight;
        }

        float randomValue = Random.Range(0, totalWeight);

        float cumulativeWeight = 0f;
        for (int i = 0; i < decorationPrefabs.Length; i++)
        {
            cumulativeWeight += decorationWeights[i];
            if (randomValue <= cumulativeWeight)
            {
                return decorationPrefabs[i];
            }
        }

        return decorationPrefabs[decorationPrefabs.Length - 1];
    }
}
