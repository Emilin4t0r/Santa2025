using UnityEngine;

public class JetExhaust : MonoBehaviour
{
    public Light exhaustLight;
    public ParticleSystem flameParticle;
    public AudioSource flyingSound;
    float flyingSoundBaseVol;

    AirplaneController ac;

    private void Start()
    {
        ac = AirplaneController.instance;
        flyingSoundBaseVol = flyingSound.volume;
    }

    private void FixedUpdate()
    {
        float thr = ac.thrustPercent;

        exhaustLight.intensity = thr;

        var main = flameParticle.main;
        main.startColor = new Color(1f, 0.8431373f, 0.1411765f, thr / 3);

        flyingSound.pitch = 0.8f + (thr / 3);
        flyingSound.volume = flyingSoundBaseVol * thr;
    }
}
