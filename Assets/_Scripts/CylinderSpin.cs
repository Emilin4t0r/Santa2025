using UnityEngine;

public class CylinderSpin : MonoBehaviour
{
    public float spd;
    void Update()
    {
        transform.Rotate(0, spd * Time.deltaTime, 0);
    }
}
