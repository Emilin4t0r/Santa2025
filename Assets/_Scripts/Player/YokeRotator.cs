using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YokeRotator : MonoBehaviour
{
    public enum YokeType { Left, Right }
    public YokeType yokeType;
    public GameObject yokeRoot, gripRoot;
    public float tiltMultiplier;
    public float xOffset;

    private void Start()
    {
        yokeRoot = transform.Find("YokeRoot").gameObject;
    }
    void Update()
    {
        float tiltAmt = 0;
        switch (yokeType)
        {
            case YokeType.Left:
                tiltAmt = (-MouseAim.Ycoord + MouseAim.Xcoord) * tiltMultiplier;
                transform.localEulerAngles = new Vector3(xOffset + tiltAmt / 2, 0, 0);
                gripRoot.transform.localEulerAngles = new Vector3(0, 0, (Input.GetAxis("Horizontal") * 35) / 2);
                break;
            case YokeType.Right:
                tiltAmt = (-MouseAim.Ycoord + -MouseAim.Xcoord) * tiltMultiplier;
                transform.localEulerAngles = new Vector3(xOffset + tiltAmt / 2, 0, 0);
                gripRoot.transform.localEulerAngles = new Vector3(0, 0, (Input.GetAxis("Horizontal") * 35) / 2);
                break;
        }
    }
}
