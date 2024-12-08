using UnityEngine;

public class DecorationCollision : MonoBehaviour
{
    public bool isPassable = false; // Set true for passable objects

    private void OnCollisionEnter(Collision collision)
    {
        if (!isPassable && collision.gameObject.CompareTag("Obstacle")) // Assuming obstacles are tagged "Obstacle"
        {
            Rigidbody rb = GetComponent<Rigidbody>(); // Get the Rigidbody of the player
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero; // Stop the player's movement in 3D space
            }
        }
    }
}

