using UnityEngine;

public class DamageDealers : MonoBehaviour
{
    public float damage = 10f; // Cantidad de da�o que este objeto inflige

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Objeto {gameObject.name} interactuando con {other.name}");
    }
}
