using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class URPFeatureToggler : MonoBehaviour
{
    public string featureName;
    public bool enableFeature = true;

    void Start()
    {
        ToggleFeature(featureName, enableFeature);
    }

    void ToggleFeature(string name, bool state)
    {
        var pipeline = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        if (pipeline == null)
        {
            Debug.LogWarning("Not using URP.");
            return;
        }

        // Get the renderer data list (internal) via reflection
        var field = typeof(UniversalRenderPipelineAsset).GetField("m_RendererDataList",
            BindingFlags.NonPublic | BindingFlags.Instance);

        var rendererDataList = field.GetValue(pipeline) as ScriptableRendererData[];

        if (rendererDataList == null || rendererDataList.Length == 0)
        {
            Debug.LogWarning("RendererDataList is empty.");
            return;
        }

        // Use default renderer
        var rendererData = rendererDataList[pipeline.defaultRendererIndex];

        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature != null && feature.name == name)
            {
                feature.SetActive(state);
                rendererData.SetDirty();
                Debug.Log($"[URP] Feature '{name}' set to {state}");
                return;
            }
        }

        Debug.LogWarning($"Render feature '{name}' not found in default renderer.");
    }
}
