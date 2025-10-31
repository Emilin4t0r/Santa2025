using System.Collections;
using UnityEngine;

public class EntranceRamp : MonoBehaviour
{
    public float timeBeforeGateOpen, gateOpenSpeed;
    public Transform gatePivot;
    bool readyToOpen = false;

    private void Start()
    {
        StartCoroutine(OpenGateAfter(timeBeforeGateOpen));
    }

    private void Update()
    {
        if (readyToOpen)
        {
            gatePivot.Rotate(new Vector3(0, 0, gateOpenSpeed * Time.deltaTime));
            if (gatePivot.localEulerAngles.z <= 2.5f)
            {
                enabled = false;
            }
        }
    }

    IEnumerator OpenGateAfter(float openDelay)
    {
        yield return new WaitForSeconds(openDelay);
        readyToOpen = true;
    }


}
