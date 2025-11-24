using UnityEngine;

public class TooltipSpawner : MonoBehaviour
{
    public static TooltipSpawner instance;

    public GameObject tt_firemsl, tt_flaps, tt_gift, tt_radarmsl;
    bool firemslShowed, flapsShowed, giftShowed, radarmslShowed;

    private void Awake()
    {
        instance = this;
    }

    public void ShowTooltip(GameObject tooltipPrefab)
    {
        foreach (Transform child in transform)
        {
            Destroy(child);
        }

        if (tooltipPrefab == tt_firemsl)
        {
            if (firemslShowed)
                return;
            else
                firemslShowed = true;
        }
        if (tooltipPrefab == tt_flaps)
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

        var tt = Instantiate(tooltipPrefab, transform);
        tt.GetComponent<RectTransform>().localPosition = Vector3.zero;
        //Do animations
        Destroy(tt, 5);
    }
}
