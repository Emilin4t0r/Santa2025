using UnityEngine;

public class AmmoCartSpawner : MonoBehaviour
{
    public GameObject ammoCartPrefab;

    public float spawnInterval;
    float timeToSpawnNextCart;

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
        Destroy(cart.gameObject, 30);
    }
}
