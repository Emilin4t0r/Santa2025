using UnityEngine;

public class CinemaCamMover : MonoBehaviour
{
    public float speed;

    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);        
    }
}
