
using UnityEngine;

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
}
