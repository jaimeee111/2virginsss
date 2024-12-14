using UnityEngine;

public class Chunk : MonoBehaviour
{
    public GameObject tilePrefab; // Prefab del tile del suelo
    public int chunkSize = 10; // Tamaño del chunk (en tiles)
    public DecorationSpawner decorationSpawner;

    void Start()
    {
        GenerateTiles();
        decorationSpawner.SpawnDecorations(chunkSize);
    }

    private void GenerateTiles()
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector3 tilePosition = new Vector3(transform.position.x + x, transform.position.y + y, 0);
                Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
            }
        }
    }
}
