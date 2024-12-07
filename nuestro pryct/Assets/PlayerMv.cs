using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidad de movimiento
    private Rigidbody2D rb;
    private Vector2 movement;

    void Awake()
    {
        // Obtiene el componente Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Captura la entrada del teclado (Horizontal y Vertical)
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        // Aplica el movimiento al Rigidbody2D
        rb.linearVelocity = movement * moveSpeed;
    }
}
