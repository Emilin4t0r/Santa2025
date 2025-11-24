using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Image fillImg;

    private void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        int buildIndex = SceneUtility.GetBuildIndexByScenePath("Gameplay"); //this returns -1 if scene doesn't exist

        if (buildIndex >= 0)
        {
            //The operation that will control the Async loading using the global LoadingData script
            AsyncOperation operation = SceneManager.LoadSceneAsync("Gameplay");
            //Stop next scene from loading
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                //display loading bar, tips, etc. here
                //->
                fillImg.fillAmount = operation.progress;

                if (operation.progress >= 0.8f)
                {
                    //allow next scene to load
                    operation.allowSceneActivation = true;
                }
                yield return null;
            }
        }
        else
        {
            print("Scene is invalid!");
        }
    }
}
