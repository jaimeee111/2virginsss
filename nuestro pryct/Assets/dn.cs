using UnityEngine;

public class CollisionTester : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Colisi�n detectada con: {collision.gameObject.name}");
    }
}
