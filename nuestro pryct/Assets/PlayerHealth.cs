using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI Elements")]
    public Slider healthBar;

    void Start()
    {
        // Inicializa la salud
        currentHealth = maxHealth;

        // Configura la barra de salud
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth; // Actualiza la barra de salud
        }
    }

    public void TakeDamage(float damage)
    {
        // Reduce la salud
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Actualiza la barra de salud
        UpdateHealthBar();

        // Comprueba si el personaje muere
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("El personaje ha muerto.");
        // Aquí puedes añadir animaciones, reiniciar el nivel, etc.
        Destroy(gameObject); // Elimina al personaje de la escena
    }

    void OnTriggerEnter(Collider other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (damageDealer != null)
        {
            Debug.Log($"Colisión (Trigger) con {other.gameObject.name}, aplicando daño: {damageDealer.damage}");
            TakeDamage(damageDealer.damage);
        }
    }
}
