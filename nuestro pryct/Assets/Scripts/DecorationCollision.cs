using UnityEngine;

[RequireComponent(typeof(BoxCollider))] // Asegura que cada objeto tenga un BoxCollider
public class DecorationCollision : MonoBehaviour
{
    public bool isPassable = false; // Si es true, el objeto será atravesable

    private void Start()
    {
        // Configura el BoxCollider según si es atravesable o no
        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.isTrigger = isPassable; // Si es pasable, el collider será un trigger
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Si el objeto no es atravesable y el jugador colisiona con él
        if (!isPassable && collision.gameObject.CompareTag("Player"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero; // Detiene el movimiento del jugador
                Debug.Log($"{gameObject.name} bloquea al jugador.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si el objeto es atravesable y el jugador entra al trigger
        if (isPassable && other.CompareTag("Player"))
        {
            Debug.Log($"El jugador atraviesa {gameObject.name}.");
        }
    }
}
