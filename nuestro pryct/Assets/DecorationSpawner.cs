using System.Collections.Generic;
using UnityEngine;

public class DecorationSpawner : MonoBehaviour
{
    public List<GameObject> decorations; // Lista de prefabs de decoraciones (árboles, rocas, ríos)
    public int maxDecorations = 20; // Número máximo de decoraciones por chunk
    private List<Vector2> usedPositions = new List<Vector2>(); // Posiciones ocupadas para evitar solapamientos

    public void SpawnDecorations(int chunkSize)
    {
        for (int i = 0; i < maxDecorations; i++)
        {
            Vector2 randomPosition;
            int attempts = 0;

            // Encuentra una posición válida dentro del chunk
            do
            {
                randomPosition = new Vector2(
                    Random.Range(0, chunkSize),
                    Random.Range(0, chunkSize)
                );
                attempts++;
            } while (IsPositionUsed(randomPosition) && attempts < 10);

            if (attempts >= 10) continue; // Si no encuentra posición válida, omite este objeto

            usedPositions.Add(randomPosition);

            // Instancia una decoración aleatoria
            GameObject decorationPrefab = decorations[Random.Range(0, decorations.Count)];
            Vector3 decorationPosition = new Vector3(
                transform.position.x + randomPosition.x,
                transform.position.y + randomPosition.y,
                0
            );
            Instantiate(decorationPrefab, decorationPosition, Quaternion.identity, transform);
        }
    }

    private bool IsPositionUsed(Vector2 position)
    {
        foreach (Vector2 usedPosition in usedPositions)
        {
            if (Vector2.Distance(position, usedPosition) < 1.5f) // Distancia mínima para evitar solapamientos
            {
                return true;
            }
        }
        return false;
    }
}

