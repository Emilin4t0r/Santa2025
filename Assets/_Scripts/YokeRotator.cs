using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YokeRotator : MonoBehaviour
{
    public enum YokeType { Left, Right }
    public YokeType yokeType;
    public GameObject yokeRoot, gripRoot;
    public float tiltMultiplier;
    AirplaneController ac;

    private void Start()
    {
        ac = AirplaneController.instance;
        yokeRoot = transform.Find("YokeRoot").gameObject;
    }
    void Update()
    {
        float tiltAmt = 0;
        switch (yokeType)
        {
            /*
            case YokeType.Left:
                transform.localEulerAngles = new Vector3(-90 + (-MouseAim.Ycoord / 2), 0, 0);
                yokeRoot.transform.localEulerAngles = new Vector3(0, MouseAim.Xcoord / 2, 0);
                gripRoot.transform.localEulerAngles = new Vector3(0, 0, (Input.GetAxis("Horizontal") * 35) / 2);
                break;
            case YokeType.Right:
                transform.localEulerAngles = new Vector3(-90 + (-MouseAim.Ycoord / 2), 0, 0);
                yokeRoot.transform.localEulerAngles = new Vector3(0, MouseAim.Xcoord / 2, 0);
                gripRoot.transform.localEulerAngles = new Vector3(0, 0, (Input.GetAxis("Horizontal") * 35) / 2);
                break;
            */
            case YokeType.Left:
                tiltAmt = (-MouseAim.Ycoord + MouseAim.Xcoord) * tiltMultiplier;
                transform.localEulerAngles = new Vector3(-90 + tiltAmt / 2, 0, 0);
                gripRoot.transform.localEulerAngles = new Vector3(0, 0, (Input.GetAxis("Horizontal") * 35) / 2);
                break;
            case YokeType.Right:
                tiltAmt = (-MouseAim.Ycoord + -MouseAim.Xcoord) * tiltMultiplier;
                transform.localEulerAngles = new Vector3(-90 + tiltAmt / 2, 0, 0);
                gripRoot.transform.localEulerAngles = new Vector3(0, 0, (Input.GetAxis("Horizontal") * 35) / 2);
                break;
        }
    }
}
