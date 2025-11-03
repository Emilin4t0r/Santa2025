using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAim : MonoBehaviour
{
    private int[] center = new int[2];
    private float blockX;
    private float mouseX;
    private float blockY;
    private float mouseY;
    public static float Xcoord;
    public static float Ycoord;

    public float mouseSensitivity;
    void Start()
    {
        center[0] = Screen.width / 2;
        center[1] = Screen.height / 2;
    }

    void Update()
    {
        blockX = Screen.width / 100f;
        mouseX = Input.mousePosition.x - center[0];
        Xcoord = (mouseX / blockX) * mouseSensitivity;
        blockY = Screen.height / 100f;
        mouseY = Input.mousePosition.y - center[1];
        Ycoord = (mouseY / blockY) * mouseSensitivity;
    }
}
