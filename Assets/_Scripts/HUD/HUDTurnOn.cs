using UnityEngine;
using UnityEngine.UI;

public class HUDTurnOn : MonoBehaviour
{
    public static HUDTurnOn instance;

    public Image mainHudMask;
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
        anim.SetTrigger("TurnOn");
        mainHudMask.color = new Color(1, 1, 1, 0);
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
                mainHudMask.color = new Color(1, 1, 1, 1);
                waitingForBootup = false;
            }
        }
    }
}
