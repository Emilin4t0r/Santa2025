using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlightInfo : MonoBehaviour
{
    AirplaneController ac;
    AircraftUtils au;

    public TextMeshProUGUI spd, alt, thr, hp, flp;
    public Image hpImg;
    public Slider thrSlider, hpSlider;

    private void Start()
    {
        ac = AirplaneController.instance;
        au = AircraftUtils.instance;
    }

    private void FixedUpdate()
    {
        float speed = ac.rb.linearVelocity.magnitude * 3.6f;
        int health = (int)au.health;
        spd.text = "SPD: " + ((int)speed).ToString("D4") + " km/h";
        alt.text = "ALT: " + ((int)ac.transform.position.y).ToString("D4") + " m";
        thr.text = "THR: " + (int)(ac.thrustPercent * 100) + "%";
        hp.text = "HP: " + health + "%";
        flp.text = ac.flap > 0 ? " FLAPS: ON" : " FLAPS: OFF";        

        thrSlider.value = ac.thrustPercent;
        hpSlider.value = au.health;

        float healthCol = health / 100f;
        hpImg.color = new Color(1 - healthCol, healthCol, healthCol, 0.75f);
    }
}
