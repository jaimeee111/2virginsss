using UnityEngine;

public class ResourcePickup : MonoBehaviour
{
    public enum ResourceType { Water, Food } // Tipos de recursos
    public ResourceType resourceType;
    public float recoveryAmount = 20f; // Cantidad que se recupera

    public GameObject pickupEffect;

    void OnTriggerEnter(Collider other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            if (resourceType == ResourceType.Water)
            {
                player.RecoverWater(recoveryAmount);
            }
            else if (resourceType == ResourceType.Food)
            {
                player.RecoverFood(recoveryAmount);
                Destroy(gameObject);
            }
        }
    }

}

