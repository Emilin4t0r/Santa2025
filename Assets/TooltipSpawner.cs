using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TooltipSpawner : MonoBehaviour
{
    public static TooltipSpawner instance;
    public float lifetime = 3f;

    public GameObject tt_firemsl, tt_flaps, tt_gift, tt_radarmsl, tt_lead;
    bool firemslShowed, flapsShowed, giftShowed, radarmslShowed, leadShowed;

    private void Awake()
    {
        instance = this;
    }

    public void ShowTooltip(GameObject tooltipPrefab)
    {        
        if (tooltipPrefab == tt_firemsl)
        {
            if (firemslShowed)
                return;
            else
                firemslShowed = true;
        }
        if (tooltipPrefab == tt_flaps) // Todo, activation - activate after first 20 seconds?
        {
            if (flapsShowed)
                return;
            else
                flapsShowed = true;
        }
        if (tooltipPrefab == tt_gift)
        {
            if (giftShowed)
                return;
            else
                giftShowed = true;
        }
        if (tooltipPrefab == tt_radarmsl)
        {
            if (radarmslShowed)
                return;
            else
                radarmslShowed = true;
        }
        if (tooltipPrefab == tt_lead)
        {
            if (leadShowed)
                return;
            else
                leadShowed = true;
        }

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        var tt = Instantiate(tooltipPrefab, transform);
        var content = tt.GetComponentInChildren<Mask>().transform.GetChild(0); // "Content" -object
        StartCoroutine(AnimateTooltip(content));
        //Do animations
        Destroy(tt, lifetime + 1); // +1 second to allow tweens to die out
    }

    IEnumerator AnimateTooltip(Transform tooltip)
    {
        Vector3 l = tooltip.transform.localPosition;
        tooltip.transform.localPosition = new Vector3(-350, l.y, l.z);
        float moveTime = 0.5f;
        tooltip.transform.DOLocalMoveX(0, moveTime);
        yield return new WaitForSeconds(lifetime - moveTime);
        if (tooltip)
            tooltip.transform.DOLocalMoveX(-350, moveTime);
    }
}
