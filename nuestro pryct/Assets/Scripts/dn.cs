using UnityEngine;

public class CollisionTester : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Colisión detectada con: {collision.gameObject.name}");
    }
}
