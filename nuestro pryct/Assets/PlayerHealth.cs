using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f; // Salud máxima
    public float currentHealth;

    [Header("Water and Food Settings")]
    public float maxWater = 100f; // Máximo de agua
    public float currentWater;
    public float waterConsumptionRate = 1f; // Consumo de agua por segundo

    public float maxFood = 100f; // Máximo de comida
    public float currentFood;
    public float foodConsumptionRate = 0.5f; // Consumo de comida por segundo

    [Header("UI Settings")]
    public Slider healthBar; // Barra de salud
    public Slider waterBar;  // Barra de agua
    public Slider foodBar;   // Barra de comida

    void Start()
    {
        // Inicializa la salud, agua y comida
        currentHealth = maxHealth;
        currentWater = maxWater;
        currentFood = maxFood;

        UpdateUI();
    }

    void Update()
    {
        // Reducir agua y comida con el tiempo
        currentWater -= waterConsumptionRate * Time.deltaTime;
        currentFood -= foodConsumptionRate * Time.deltaTime;

        // Clampea los valores para evitar que sean negativos
        currentWater = Mathf.Clamp(currentWater, 0, maxWater);
        currentFood = Mathf.Clamp(currentFood, 0, maxFood);

        // Actualizar la UI
        UpdateUI();

        // Si agua o comida llegan a 0, causa daño continuo
        if (currentWater <= 0 || currentFood <= 0)
        {
            TakeDamage(Time.deltaTime * 10); // Daño por falta de recursos
        }

        // Si la salud llega a 0, muere
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void UpdateUI()
    {
        // Actualiza las barras de UI
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
        }
        if (waterBar != null)
        {
            waterBar.value = currentWater / maxWater;
        }
        if (foodBar != null)
        {
            foodBar.value = currentFood / maxFood;
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"Recibiendo daño: {damage}");
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("El personaje ha muerto.");
        // Aquí puedes añadir lógica como reiniciar el nivel o mostrar un mensaje de Game Over
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        // Detecta si el objeto tiene el componente DamageDealer
        DamageDealers damageDealer = other.GetComponent<DamageDealers>();
        if (damageDealer != null)
        {
            Debug.Log($"Trigger detectado con: {other.gameObject.name}, daño recibido: {damageDealer.damage}");
            TakeDamage(damageDealer.damage);
        }

       

    }

    public void RecoverWater(float amount)
    {
        currentWater += amount;
        currentWater = Mathf.Clamp(currentWater, 0, maxWater); // Limita el agua al máximo permitido
        UpdateUI();
        Debug.Log($"Agua recuperada: {amount}. Agua actual: {currentWater}");
    }

    public void RecoverFood(float amount)
    {
        currentFood += amount;
        currentFood = Mathf.Clamp(currentFood, 0, maxFood); // Limita la comida al máximo permitido
        UpdateUI();
        Debug.Log($"Comida recuperada: {amount}. Comida actual: {currentFood}");
    }


}
