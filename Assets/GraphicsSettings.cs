using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GraphicsSettings : MonoBehaviour
{
    public UniversalRenderPipelineAsset urpAsset;
    public GraphicsPreset[] presets;

    public void ApplyPreset(int index)
    {
        GraphicsPreset p = presets[index];

        // Render scale
        urpAsset.renderScale = p.renderScale;

        if (p.shadows)
        {
            // Shadow Distance
            urpAsset.shadowDistance = 100f;

            // Shadow Resolution
            urpAsset.mainLightShadowmapResolution = (int)p.shadowResolution;

            // Cascade Count
            urpAsset.shadowCascadeCount = Mathf.Clamp(p.cascadeCount, 1, 4);
        }
        else
        {
            // Shadows Off -> force lightweight settings
            urpAsset.shadowDistance = 0f;
            urpAsset.shadowCascadeCount = 1;
            urpAsset.mainLightShadowmapResolution = 0;
        }

        // Texture Quality
        QualitySettings.globalTextureMipmapLimit = p.textureQuality;

        // Anti-Aliasing
        urpAsset.msaaSampleCount = p.antiAliasing;
    }
}
