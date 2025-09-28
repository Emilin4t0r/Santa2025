using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public static HUD instance;
    public enum HUDMode { AirToAir, AirToGround }
    public HUDMode hudMode;
    public GameObject airToAir, airToGround;

    public static float hudOffset = -0.25f;
    public RectTransform[] offsetElements;

    private void Awake()
    {
        instance = this;        
    }

    private void Start()
    {
        foreach(var e in offsetElements)
        {
            e.localPosition += new Vector3(0, hudOffset, 0);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleHUDMode();
        }
    }

    void ToggleHUDMode()
    {        
        if (hudMode == HUDMode.AirToAir)
        {
            airToAir.SetActive(false);
            airToGround.SetActive(true);
            hudMode = HUDMode.AirToGround;
        } else if (hudMode == HUDMode.AirToGround)
        {
            airToAir.SetActive(true);
            airToGround.SetActive(false);
            hudMode = HUDMode.AirToAir;
        }
    }
}
