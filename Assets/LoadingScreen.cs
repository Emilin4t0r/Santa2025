using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Image fillImg;
    public float fillSpeed = 3f;
    float currentFill = 0f;

    private void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        int buildIndex = SceneUtility.GetBuildIndexByScenePath("Gameplay");

        if (buildIndex >= 0)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync("Gameplay");
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);
                // ^ Normalizes the 0–0.9 Unity progress into 0–1

                // Smoothly move toward target progress
                currentFill = Mathf.Lerp(currentFill, targetProgress, Time.deltaTime * fillSpeed);
                fillImg.fillAmount = currentFill;

                // Allow activation when Unity hits 0.9 (meaning loaded)
                if (operation.progress >= 0.9f && currentFill >= 0.995f)
                {
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }
        else
        {
            Debug.LogError("Scene is invalid!");
        }
    }
}
