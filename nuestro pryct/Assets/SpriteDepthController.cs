using UnityEngine;

public class SpriteDepthController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Header("Ajuste de Escala de Profundidad")]
    public float depthMultiplier = 100f; // Ajustable en el inspector

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Calcula el sortingOrder con el multiplicador ajustado
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * depthMultiplier);
    }
}
