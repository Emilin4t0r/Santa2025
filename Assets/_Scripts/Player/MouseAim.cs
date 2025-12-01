using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAim : MonoBehaviour
{
    public RectTransform fakeCursor;
    public float baseSensitivity;
    public static float Xcoord;
    public static float Ycoord;

    private Vector2 fakePos;
    public RectTransform canvasRect;

    public float steerForce = 2f;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        fakePos = Vector2.zero;
    }

    void Update()
    {
        if (SettingsToggler.gamePaused) return;

        // Get mouse movement (not position)
        float dx = Input.GetAxisRaw("Mouse X");
        float dy = Input.GetAxisRaw("Mouse Y");

        // Apply your own sensitivity
        fakePos += new Vector2(dx, dy) * baseSensitivity * Settings.mouseSensitivity;

        // Clamp to screen/canvas edges
        float halfW = canvasRect.sizeDelta.x * 0.5f;
        float halfH = canvasRect.sizeDelta.y * 0.5f;

        fakePos.x = Mathf.Clamp(fakePos.x, -halfW, halfW);
        fakePos.y = Mathf.Clamp(fakePos.y, -halfH, halfH);

        // Move fake cursor
        fakeCursor.anchoredPosition = fakePos;

        // Convert to steering (-1 to 1)
        Xcoord = (fakePos.x / halfW) * steerForce;
        Ycoord = (fakePos.y / halfH) * steerForce;
    }
}
