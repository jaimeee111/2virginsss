using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target; // Referencia al personaje
    public float smoothSpeed = 0.125f; // Velocidad de suavizado
    public Vector3 offset; // Offset para ajustar la posici�n de la c�mara

    private Vector3 velocity = Vector3.zero; // Velocidad usada por SmoothDamp

    void LateUpdate()
    {
        if (target == null) return; // Aseg�rate de que hay un objetivo asignado

        // Calcula la posici�n deseada
        Vector3 desiredPosition = target.position + offset;

        // Suaviza la transici�n hacia la posici�n deseada
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);

        // Actualiza la posici�n de la c�mara
        transform.position = smoothedPosition;
    }
}
