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

    public TextMeshProUGUI spd, alt, thr, hp, flp, flares;
    public Image hpImg;
    public Slider thrSlider, hpSlider;

    bool showFlares;
    Countermeasures countermeasures;
    int flaresLeft;

    private void Start()
    {
        ac = AirplaneController.instance;
        au = AircraftUtils.instance;
        countermeasures = GameObject.Find("WeaponsDupe").transform.Find("CountermeasuresParent").GetComponent<Countermeasures>();
        flaresLeft = countermeasures.flares.Count;
        if (flaresLeft == 0)
        {
            flares.transform.parent.gameObject.SetActive(false);
            showFlares = false;
        }
        else { 
            showFlares = true; 
        }
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

        if (showFlares)
        {
            flaresLeft = countermeasures.flares.Count;
            flares.text = flaresLeft.ToString("0");
        }

        thrSlider.value = ac.thrustPercent;
        hpSlider.value = au.health;

        float healthCol = health / 100f;
        hpImg.color = new Color(1 - healthCol, healthCol, healthCol, 0.75f);
    }
}
