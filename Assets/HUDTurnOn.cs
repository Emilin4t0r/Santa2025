using UnityEngine;

public class HUDTurnOn : MonoBehaviour
{
    public static HUDTurnOn instance;

    public Canvas canvas;
    Animator anim;
    public GameObject loadHUD;
    public bool waitingForBootup;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        waitingForBootup = true;
        anim = GetComponent<Animator>();
        Invoke("Bootup", 1);
    }

    void Bootup()
    {
        canvas.enabled = false;
        anim.SetTrigger("TurnOn");        
    }

    private void Update()
    {
        if (waitingForBootup)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0); // layer 0
            if (stateInfo.IsName("HUDTurnOn") && stateInfo.normalizedTime >= 1f)
            {
                print("Boot anim finished!");
                loadHUD.SetActive(false);
                waitingForBootup = false;
                TargetInfo.instance.LoadHUDAfterBootup();
            }
        }
    }
}
