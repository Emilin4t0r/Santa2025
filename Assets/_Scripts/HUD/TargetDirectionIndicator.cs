using UnityEngine;

public class TargetDirectionIndicator : MonoBehaviour
{
    public Transform arrowU, arrowD, arrowL, arrowR;
    public static Transform currentThreat;
    public GameObject threatText;
    public float threatFlashFrequency;

    [Range(0f, 1f)]
    public float threshold = 0.5f; // Minimum component ratio to activate an arrow

    AircraftUtils au;

    private void Start()
    {
        au = AircraftUtils.instance;
        DisableAllArrows();
        ToggleThreatText(false);
    }

    private void Update()
    {
        if (!au.turnedOn) return;

        if (EnemiesController.enemiesAttacking.Count > 0)
        {
            currentThreat = EnemiesController.enemiesAttacking[0].transform;
        } else
        {
            currentThreat = null;
        }

        if (currentThreat != null)
        {
            UpdateArrows();
        }
        else
        {
            DisableAllArrows();
            ToggleThreatText(false);
        }
    }

    void UpdateArrows()
    {
        // World-space vector to the threat
        Vector3 toThreat = currentThreat.position - transform.position;

        // Convert to local space relative to this object
        Vector3 localDir = transform.InverseTransformDirection(toThreat);

        // Zero out minor components
        float absX = Mathf.Abs(localDir.x);
        float absZ = Mathf.Abs(localDir.z);

        // Reset all arrows first
        DisableAllArrows();

        // Determine dominant direction
        if (absZ >= absX * threshold)
        {
            if (localDir.z > 0)
                arrowU.gameObject.SetActive(true);
            else if (localDir.z < 0)
                arrowD.gameObject.SetActive(true);
        }

        if (absX >= absZ * threshold)
        {
            if (localDir.x > 0)
                arrowR.gameObject.SetActive(true);
            else if (localDir.x < 0)
                arrowL.gameObject.SetActive(true);
        }

        FlashThreatText();
    }

    float nextThreatFlash;
    void FlashThreatText()
    {
        if (Time.time > nextThreatFlash)
        {
            ToggleThreatText(!threatText.activeSelf);
            nextThreatFlash = Time.time + threatFlashFrequency;
        }
    }

    void ToggleThreatText(bool state)
    {
        threatText.SetActive(state);
        if (state)
            SoundSpawner.SpawnSound(transform.position, transform.parent, SoundLibrary.GetClip("rwr_target"), 0, 0f, 0.7f);
    }

    void DisableAllArrows()
    {
        arrowU.gameObject.SetActive(false);
        arrowD.gameObject.SetActive(false);
        arrowL.gameObject.SetActive(false);
        arrowR.gameObject.SetActive(false);            
    }
}
