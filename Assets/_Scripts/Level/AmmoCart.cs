using DG.Tweening;
using UnityEngine;

public class AmmoCart : MonoBehaviour
{
    public float moveSpeed;
    public GameObject gift;
    public GameObject parachute;

    private void Start()
    {
        
    }

    void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AmmoGiftSpawner"))
        {
            SpawnGift();
        }
    }

    void SpawnGift()
    {
        gift.transform.parent = null;
        gift.transform.localEulerAngles = new Vector3(270, 0, 0);
        gift.GetComponent<Gift>().Launch();

        Rigidbody grb = gift.GetComponent<Rigidbody>();
        grb.isKinematic = false;
        grb.AddForce(transform.forward * moveSpeed * 100, ForceMode.Impulse);
        
        parachute.transform.DOScale(1, 5).SetEase(Ease.InExpo);
    }
}
