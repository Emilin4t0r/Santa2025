using UnityEngine;

public class TVStatic : MonoBehaviour
{

    Transform img;
    bool doStatic;

    private void Start()
    {
        img = transform.GetChild(0);
        ToggleStatic(false);
    }

    public void ToggleStatic(bool yesno)
    {
        doStatic = yesno;
        img.gameObject.SetActive(yesno);
    }

    void JitterImage()
    {
        img.transform.localPosition = Vector3.zero;
        float x = Random.Range(-0.2f, 0.2f);
        float y = Random.Range(-0.2f, 0.2f);
        img.transform.localPosition = new Vector3(x, y, 0);
    }

    private void Update()
    {
        if (doStatic)
            JitterImage();
    }
}
