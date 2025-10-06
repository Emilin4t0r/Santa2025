using UnityEngine;

public class MenuTurntable : MonoBehaviour
{
    public float spd;
    void Update()
    {
        transform.Rotate(0, spd * Time.deltaTime, 0);
    }
}
