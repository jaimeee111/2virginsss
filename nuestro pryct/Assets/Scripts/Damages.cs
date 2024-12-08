using UnityEngine;

public class DamageDealers : MonoBehaviour
{
    public float damage = 10f; // Cantidad de da�o que este objeto inflige
    public GameObject bloodEffect; // Prefab del sistema de part�culas (sangre)

    void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que colisiona es el jugador
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            // Aplica el da�o al jugador
            player.TakeDamage(damage);

            // Instancia el efecto de sangre en la posici�n del jugador
            if (bloodEffect != null)
            {
                Instantiate(bloodEffect, other.transform.position, Quaternion.identity);
            }

            Debug.Log($"Objeto {gameObject.name} interactuando con {other.name}, da�o infligido: {damage}");
        }
    }
}
