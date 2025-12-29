using DG.Tweening;
using UnityEngine;

public class MenuTurntable : MonoBehaviour
{
    public static MenuTurntable instance;
    Transform mainCam;

    public float spd;
    public float mouseRotateSpd;
    [HideInInspector] public bool spin;

    bool inLowerPos;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        spin = true;
        mainCam = Camera.main.transform;
    }

    void Update()
    {
        if (spin)
        {
            transform.Rotate(0, spd * Time.deltaTime, 0);
        }
    }

    private void OnMouseDrag()
    {
        var x = Input.GetAxis("Mouse X");
        transform.Rotate(0, x * spd * mouseRotateSpd * Time.deltaTime, 0);
    }

    public void TogglePosition()
    {
        if (!inLowerPos)
        {
            transform.DOLocalMoveY(-4, 1.5f).SetEase(Ease.InOutSine);
            transform.DORotate(new Vector3(0, 0, 0), 1.5f);
            spin = false;
            inLowerPos = true;
        }
        else
        {
            transform.DOLocalMoveY(-3, 1).SetEase(Ease.InOutSine);
            spin = true;
            inLowerPos = false;
        }
    }
}
