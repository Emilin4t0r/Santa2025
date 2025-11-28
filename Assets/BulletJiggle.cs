using UnityEngine;

public class BulletJiggle : MonoBehaviour
{
    public Transform bullet3d;
    public float magnitude, frequency;

    float nextTimeToJiggle;

    private void Update()
    {
        if (Time.time > nextTimeToJiggle)
        {
            bullet3d.localPosition = Vector3.zero;
            bullet3d.localPosition = Random.insideUnitSphere * magnitude;
            nextTimeToJiggle = Time.time + frequency;
        }        
    }
}
