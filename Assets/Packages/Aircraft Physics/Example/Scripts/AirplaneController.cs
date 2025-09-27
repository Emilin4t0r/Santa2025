using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AirplaneController : MonoBehaviour
{
    public static AirplaneController instance;

    public bool mouseHorizontalAsRoll;

    [SerializeField]
    List<AeroSurface> controlSurfaces = null;
    [SerializeField]
    List<WheelCollider> wheels = null;
    [SerializeField]
    float rollControlSensitivity = 0.2f;
    [SerializeField]
    float pitchControlSensitivity = 0.2f;
    [SerializeField]
    float yawControlSensitivity = 0.2f;

    [Range(-1, 1)]
    public float Pitch;
    [Range(-1, 1)]
    public float Yaw;
    [Range(-1, 1)]
    public float Roll;
    [Range(0, 1)]
    public float flap;
    [SerializeField]

    public float thrustPercent;
    public float brakesTorque;

    AircraftPhysics aircraftPhysics;
    public Rigidbody rb;

    // AUDIO
    AudioSource flyingSound;
    GameObject enemyLockSound;
    private void Awake()
    {
        instance = this;
        aircraftPhysics = GetComponent<AircraftPhysics>();
        rb = GetComponent<Rigidbody>();
        flyingSound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (mouseHorizontalAsRoll)
        {
            if (!CameraController.freeLooking)
            {
                Pitch = Mathf.Clamp(-MouseAim.Ycoord / 75 + Input.GetAxis("Vertical"), -1, 1);
                Roll = Mathf.Clamp(MouseAim.Xcoord / 75, -1, 1);
            }
            Yaw = -Input.GetAxis("Horizontal");
        } else
        {
            if (!CameraController.freeLooking)
            {
                Pitch = Mathf.Clamp(-MouseAim.Ycoord / 75 + Input.GetAxis("Vertical"), -1, 1);
                Yaw = Mathf.Clamp(-MouseAim.Xcoord / 75, -1, 1);
            }
            Roll = Input.GetAxis("Horizontal");
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            thrustPercent += Time.deltaTime;
            if (thrustPercent > 1)
                thrustPercent = 1;
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            thrustPercent -= Time.deltaTime;
            if (thrustPercent < 0)
                thrustPercent = 0;            
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            flap = flap > 0 ? 0 : 0.3f;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            brakesTorque = brakesTorque > 0 ? 0 : 100f;
        }

        //Camera shaking
        if (!CameraController.freeLooking)
        {
            float spd = rb.linearVelocity.magnitude;
            if (spd > 100 && Mathf.Abs(Pitch) > 0.75f)
            {
                EZCameraShake.CameraShaker.Instance.ShakeOnce(Pitch / 15 * (spd / 100), Pitch * 15 * (spd / 100), 0, 1f);
            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            Destroy(enemyLockSound);
            SoundSpawner.SpawnSoundLoop(transform.position, transform, SoundLibrary.GetClip("rwr_missile"));
        }
    }

    private void FixedUpdate()
    {
        SetControlSurfacesAngles(Pitch, Roll, Yaw, flap);
        aircraftPhysics.SetThrustPercent(thrustPercent);
        foreach (var wheel in wheels)
        {
            wheel.brakeTorque = brakesTorque;
            // small torque to wake up wheel collider
            wheel.motorTorque = 0.01f;
        }

        flyingSound.pitch = 0.8f + (thrustPercent / 3);
        /*if (EnemiesController.enemiesAttacking.Count > 0)
        {
            if (enemyLockSound == null)
                enemyLockSound = SoundSpawner.SpawnSoundLoop(transform.position, transform, SoundLibrary.GetClip("rwr_lock"));
        } else
        {
            if (enemyLockSound != null)
                SoundSpawner.EndLoop(enemyLockSound);
        }*/
    }

    public void SetControlSurfacesAngles(float pitch, float roll, float yaw, float flap)
    {
        foreach (var surface in controlSurfaces)
        {
            if (surface == null || !surface.IsControlSurface) continue;
            switch (surface.InputType)
            {
                case ControlInputType.Pitch:
                    surface.SetFlapAngle(pitch * pitchControlSensitivity * surface.InputMultiplyer);
                    break;
                case ControlInputType.Roll:
                    surface.SetFlapAngle(roll * rollControlSensitivity * surface.InputMultiplyer);
                    break;
                case ControlInputType.Yaw:
                    surface.SetFlapAngle(yaw * yawControlSensitivity * surface.InputMultiplyer);
                    break;
                case ControlInputType.Flap:
                    surface.SetFlapAngle(this.flap * surface.InputMultiplyer);
                    break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            SetControlSurfacesAngles(Pitch, Roll, Yaw, flap);
    }
}
