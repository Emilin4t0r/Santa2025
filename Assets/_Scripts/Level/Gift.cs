using DG.Tweening;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Gift : MonoBehaviour
{
    [SerializeField] float horizontalDamp = 0.9999f; // lower = stronger slowdown
    [SerializeField] float verticalDamp = 0.99f; // lower = stronger slowdown

    Rigidbody rb;
    GameObject parachute;

    bool touchedGround;

    public GameObject collectParticle;

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

        // Damp horizontal, 
        Vector3 horizontal = new Vector3(v.x, v.y, v.z);
        horizontal *= horizontalDamp;

        // Then damp vertical
        Vector3 vertical = new Vector3(v.x, v.y, v.z);
        vertical *= verticalDamp;

        rb.linearVelocity = new Vector3(horizontal.x, horizontal.y, horizontal.z);
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
            GameObject collect = Instantiate(collectParticle, transform.position, transform.rotation, null);
            Destroy(collect, 3f);
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
                Notifications.instance.ShowNotification("- 20mm guns rearmed! -");
                break;
            case "30MM":
                wpnsDupe.Find("SingleGuns").GetComponent<Guns>().ReloadGuns();
                Notifications.instance.ShowNotification("- 30mm guns rearmed! -");
                break;
            case "100MM":
                wpnsDupe.Find("AirBurst").GetComponent<Guns>().ReloadGuns();
                Notifications.instance.ShowNotification("- 100mm guns rearmed! -");
                break;
            // Missiles
            case "IR":
                wpn = Hardpoint.WeaponType.Pike_Double;
                WeaponsParent.instance.ReplaceWeaponGameobject(wpn);
                Notifications.instance.ShowNotification("- IR missiles rearmed! -");
                break;
            case "RADAR":
                wpn = Hardpoint.WeaponType.Longbow;
                WeaponsParent.instance.ReplaceWeaponGameobject(wpn);
                Notifications.instance.ShowNotification("- Radar missiles rearmed! -");
                break;
            case "SWARM":
                wpn = Hardpoint.WeaponType.Huracán_Pod;
                WeaponsParent.instance.ReplaceWeaponGameobject(wpn);
                Notifications.instance.ShowNotification("- Swarm missiles rearmed! -");
                break;
        }
        SoundSpawner.SpawnSound(transform.position, null, SoundLibrary.GetClip("pickup_gift"), 0, 0);
    }
}
