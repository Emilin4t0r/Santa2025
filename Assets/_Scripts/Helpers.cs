
using System;
using UnityEngine;
using UnityEngine.Rendering;

public static class Helpers
{
    /// <summary>
    /// Adds spaces to float values of thousands for neat display.
    /// Example: 10000f -> "10 000"
    /// </summary>
    public static string FormatSpaceIntoThousands(float value)
    {
        int rounded = Mathf.RoundToInt(value);
        return rounded.ToString("#,0", System.Globalization.CultureInfo.InvariantCulture).Replace(",", " ");
    }

    /// <summary>
    /// Finds radar blip closest to origin in HUD
    /// </summary>
    /// <param name="origin">The RectTransform to compare from.</param>
    /// <param name="radarParent">Parent containing radar blip children.</param>
    public static RectTransform GetClosestRadarBlip(RectTransform origin, Transform radarParent)
    {
        if (radarParent.childCount == 0)
            return null;

        RectTransform closest = null;
        float bestDist = 1000f;

        for (int i = 0; i < radarParent.childCount; i++)
        {
            RectTransform child = radarParent.GetChild(i) as RectTransform;
            float dist = Vector2.Distance(origin.anchoredPosition, child.anchoredPosition);

            Debug.Log("Distance to " + child.name + ": " + dist);

            if (dist < bestDist)
            {
                Debug.Log("found better distance: " + dist + " " + child.name + ".\n choosing " + child.name);
                bestDist = dist;
                closest = child;
                
            }
        }

        Debug.Log("Closest was " + closest.name);

        return closest;
    }
}
