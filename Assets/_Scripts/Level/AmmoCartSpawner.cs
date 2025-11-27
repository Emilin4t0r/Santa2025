using UnityEngine;

public class AmmoCartSpawner : MonoBehaviour
{
    public GameObject ammoCartPrefab;

    public float spawnInterval;
    float timeToSpawnNextCart;

    private void Start()
    {
        timeToSpawnNextCart = Time.time + spawnInterval;
    }

    private void Update()
    {
        if (Time.time > timeToSpawnNextCart)
        {
            SpawnCart();
            timeToSpawnNextCart = Time.time + spawnInterval;
        }
    }

    void SpawnCart()
    {        
        Quaternion q = transform.rotation;
        Quaternion r = Random.rotation;
        Quaternion randomYRot = new Quaternion(q.x, r.y, q.z, q.w);
        var cart = Instantiate(ammoCartPrefab, transform.position, randomYRot, transform);
        SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip("gift_announce"));
        Destroy(cart.gameObject, 30);

        var tts = TooltipSpawner.instance;
        tts.ShowTooltip(tts.tt_gift);
    }
}
