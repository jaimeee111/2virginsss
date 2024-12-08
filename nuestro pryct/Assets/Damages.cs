using UnityEngine;

public class DamageDealers : MonoBehaviour
{
    public float damage = 10f; // Cantidad de daño que este objeto inflige
    public GameObject bloodEffect; // Prefab del sistema de partículas (sangre)

    void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que colisiona es el jugador
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            // Aplica el daño al jugador
            player.TakeDamage(damage);

            // Instancia el efecto de sangre en la posición del jugador
            if (bloodEffect != null)
            {
                Instantiate(bloodEffect, other.transform.position, Quaternion.identity);
            }

            Debug.Log($"Objeto {gameObject.name} interactuando con {other.name}, daño infligido: {damage}");
        }
    }
}
