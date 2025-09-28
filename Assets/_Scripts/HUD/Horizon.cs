using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horizon : MonoBehaviour
{
    AirplaneController ac;
    private void Start()
    {
        ac = AirplaneController.instance;
    }
    private void FixedUpdate()
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, -ac.transform.localEulerAngles.z);
    }
}
