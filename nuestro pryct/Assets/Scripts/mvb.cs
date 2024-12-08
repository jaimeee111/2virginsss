using UnityEngine;

public class SimpleMover : MonoBehaviour
{
    void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime);
    }
}
