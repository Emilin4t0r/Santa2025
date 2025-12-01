using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMainMenuFromReplay : MonoBehaviour
{
    public static LoadMainMenuFromReplay instance;

    public Image black;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Unlock cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadMainMenu()
    {        
        StartCoroutine(MenuLoader());
    }

    IEnumerator MenuLoader()
    {
        black.DOFade(1, 5).SetEase(Ease.InExpo);
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("MainMenu");
    }
}
