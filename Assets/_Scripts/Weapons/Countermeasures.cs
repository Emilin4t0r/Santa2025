using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Countermeasures : MonoBehaviour
{
    AircraftUtils au;
    public List<Flare> flares;
    float nextTimeToFire;
    public float fireRate, launchForce = 10;
    public GameObject launchParticle;
    bool inGameScene;

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }
    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    void OnSceneChanged(Scene old, Scene now)
    {
        if (now.name == "Gameplay")
        {
            InitializeWeapon();
        }
        else
        {
            inGameScene = false;
        }
    }

    public void InitializeWeapon()
    {
        GetBoxesFromChildren();
        au = AircraftUtils.instance;
        inGameScene = true;
    }

    void GetBoxesFromChildren()
    {
        flares = new List<Flare>();
        Flare[] _flares = GetComponentsInChildren<Flare>();
        foreach (Flare f in _flares)
        {
            flares.Add(f);
        }
    }

    private void Update()
    {
        if (!inGameScene)
            return;

        if (!au.turnedOn)
            return;

        if (SettingsToggler.gamePaused) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (flares.Count <= 0)
            {
                SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip("empty_weapon"), 0, 0);
                return;
            }

            if (Time.time > nextTimeToFire)
            {
                var p = Instantiate(launchParticle, flares[0].transform.position, Quaternion.identity, null);
                p.transform.eulerAngles = new Vector3(0, 0, 0);
                FireFlare();
                FireFlare();
            }
        }
    }

    void FireFlare()
    {
        nextTimeToFire = Time.time + fireRate;

        Flare fl = flares[0];
        fl.TurnOn(launchForce);
        fl.transform.parent = null;
        flares.Remove(fl);
        Destroy(fl.gameObject, 10);

        EZCameraShake.CameraShaker.Instance.ShakeOnce(0.4f, 15, 0, 0.5f);
    }
}
