using UnityEngine;

public class HealthRegeneration : MonoBehaviour
{
    [Header("Regeneration Settings")]
    public float healthRegenRate = 5f; // Tasa de regeneración de salud por segundo
    private PlayerHealth playerHealth;

    void Start()
    {
        // Obtener referencia al componente PlayerHealth en el mismo GameObject
        playerHealth = GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogError("No se encontró un componente PlayerHealth en el GameObject.");
        }
    }

    void Update()
    {
        if (playerHealth != null)
        {
            // Verificar si los niveles de comida y agua son mayores al 50%
            bool canRegenerate = playerHealth.currentWater > playerHealth.maxWater * 0.5f &&
                                 playerHealth.currentFood > playerHealth.maxFood * 0.5f;

            if (canRegenerate && playerHealth.currentHealth < playerHealth.maxHealth)
            {
                // Regenerar salud lentamente
                playerHealth.currentHealth += healthRegenRate * Time.deltaTime;
                playerHealth.currentHealth = Mathf.Clamp(playerHealth.currentHealth, 0, playerHealth.maxHealth);

                // Actualizar la UI de salud
                playerHealth.UpdateUI();
            }
        }
    }
}
