using DG.Tweening;
using UnityEngine;

public class Gift : MonoBehaviour
{
    [SerializeField] float horizontalDamp = 0.95f; // lower = stronger slowdown

    Rigidbody rb;
    GameObject parachute;

    bool touchedGround;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        parachute = transform.Find("Parachute").gameObject;
    }

    void Update()
    {
        if (touchedGround) return;

        transform.localEulerAngles = rb.linearVelocity;
    }

    void FixedUpdate()
    {
        if (touchedGround || rb.isKinematic) return;
        
        Vector3 v = rb.linearVelocity;

        // Zero out vertical part, damp horizontal, then restore Y
        Vector3 horizontal = new Vector3(v.x, 0f, v.z);
        horizontal *= horizontalDamp;

        rb.linearVelocity = new Vector3(horizontal.x, v.y, horizontal.z);
    }

    public void Launch()
    {
        DOTween.To(() => horizontalDamp, x => horizontalDamp = x, 0.99f, 10f).SetEase(Ease.InExpo);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(parachute);
            horizontalDamp = 0f;
            touchedGround = true;
        }        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Bullet"))
        {
            GrantRearm();
            Destroy(gameObject);
        }
    }

    void GrantRearm()
    {
        Hardpoint.WeaponType wpn = 0;
        Transform wpnsDupe = GameObject.Find("WeaponsDupe").transform;
        string activeWpn = wpnsDupe.GetComponent<WeaponsSelector>().currentWeaponName;        
        print("dupe: " + wpnsDupe.name + " active weapon: " + activeWpn);
        switch (activeWpn)
        {
            // Guns
            case "20MM":
                wpnsDupe.Find("ChainGuns").GetComponent<Guns>().ReloadGuns();
                break;
            case "30MM":
                wpnsDupe.Find("SingleGuns").GetComponent<Guns>().ReloadGuns();
                break;
            case "100MM":
                wpnsDupe.Find("AirBurst").GetComponent<Guns>().ReloadGuns();
                break;
            // Missiles
            case "IR":
                wpn = Hardpoint.WeaponType.Pike_Double;
                WeaponsParent.instance.ReplaceWeaponGameobject(wpn);
                break;
            case "RADAR":
                wpn = Hardpoint.WeaponType.Longbow;
                WeaponsParent.instance.ReplaceWeaponGameobject(wpn);
                break;
            case "SWARM":
                wpn = Hardpoint.WeaponType.Huracán_Pod;
                WeaponsParent.instance.ReplaceWeaponGameobject(wpn);
                break;
        }
        SoundSpawner.SpawnSound(transform.position, null, SoundLibrary.GetClip("pickup_gift"), 0, 0);
    }
}
