using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Settings/Graphics Preset")]
public class GraphicsPreset : ScriptableObject
{
    [Header("URP Quality")]
    public float renderScale = 1;           // Render scale in URP

    [Header("Shadows")]
    public bool shadows = true;
    public ShadowResolution shadowResolution = ShadowResolution.VeryHigh;
    public int cascadeCount = 2;     // 1,2,3 allowed

    public int textureQuality = 0;        // 0 = Full Res, 1 = Half Res...
    public int antiAliasing = 2;          // 0,2,4,8 in URP Quality
}
