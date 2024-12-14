using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement2D : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidad de movimiento
    private Rigidbody rb;
    private Vector2 movement;
    private Animator animator;

    void Awake()
    {
        // Obtiene el componente Rigidbody
        rb = GetComponent<Rigidbody>();

        // Obtiene el componente Animator
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Captura la entrada del teclado (Horizontal y Vertical)
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");

        // Actualiza los parámetros del Animator
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude); // Magnitud al cuadrado (0 si no hay movimiento)
    }

    void FixedUpdate()
    {
        // Aplica el movimiento al Rigidbody
        rb.linearVelocity = movement * moveSpeed;
    }
}
