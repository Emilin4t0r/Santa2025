using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CinematicCamsController : MonoBehaviour
{
    [SerializeField] private List<GameObject> cameras = new List<GameObject>();
    [SerializeField] private List<GameObject> onboardCams = new List<GameObject>();
    public Transform player;
    public Image fakeCursor;

    float t;

    void Update()
    {       
        if (Time.time > t)
        {
            ActivateClosest();
            t = Time.time + 1f;
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            fakeCursor.enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            int r = Random.Range(0, 3);
            for(int i= 0; i < onboardCams.Count; ++i)
            {
                if (i == r)
                    onboardCams[i].SetActive(true);
                else
                    onboardCams[i].SetActive(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            for (int i = 0; i < onboardCams.Count; ++i)
            {
                onboardCams[i].SetActive(false);
            }
        }
    }

    private void ActivateClosest()
    {
        if (player == null || cameras.Count == 0)
            return;

        GameObject closest = null;
        float closestDistanceSqr = float.MaxValue;

        foreach (var obj in cameras)
        {
            if (obj == null)
                continue;

            float distSqr = (obj.transform.position - player.position).sqrMagnitude;

            if (distSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distSqr;
                closest = obj;
            }
        }

        foreach (var obj in cameras)
        {
            if (obj != null)
                obj.SetActive(obj == closest);
        }
    }
}
