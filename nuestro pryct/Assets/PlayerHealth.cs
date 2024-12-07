using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Water and Food Settings")]
    public float maxWater = 100f;
    private float currentWater;

    public float maxFood = 100f;
    private float currentFood;

    [Header("UI Elements")]
    public Slider healthBar;
    public Slider waterBar;
    public Slider foodBar;

    [Header("Damage Settings")]
    public string damageTag = "Enemy"; // Tag de los objetos que hacen daño
    public float damageAmount = 10f;  // Cantidad de daño recibido por colisión

    void Start()
    {
        // Inicializa las barras y valores
        currentHealth = maxHealth;
        currentWater = maxWater;
        currentFood = maxFood;

        UpdateUI();
    }

    void Update()
    {
        // Reduce agua y comida con el tiempo
        currentWater -= Time.deltaTime; // Reduce agua
        currentFood -= Time.deltaTime * 0.5f; // Reduce comida más lento

        // Verifica si agua o comida son 0
        if (currentWater <= 0 || currentFood <= 0)
        {
            TakeDamage(Time.deltaTime * 10); // Recibe daño continuo por falta de agua/comida
        }

        // Si la salud es 0, mata al personaje
        if (currentHealth <= 0)
        {
            Die();
        }

        // Actualiza las barras de UI
        UpdateUI();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(damageTag))
        {
            Debug.Log($"Colisión con {collision.gameObject.name}");
            TakeDamage(damageAmount);
        }
    }


    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Recibió daño: {amount}, Salud actual: {currentHealth}");
    }


    private void UpdateUI()
    {
        if (healthBar)
        {
            healthBar.value = currentHealth / maxHealth;
            Debug.Log($"Barra de salud actualizada: {healthBar.value}");
        }

        if (waterBar)
        {
            waterBar.value = currentWater / maxWater;
        }

        if (foodBar)
        {
            foodBar.value = currentFood / maxFood;
        }
    }


    private void Die()
    {
        Debug.Log("El personaje ha muerto.");
        // Puedes añadir lógica adicional como cargar otra escena, animaciones, etc.
        Destroy(gameObject);
    }
}
