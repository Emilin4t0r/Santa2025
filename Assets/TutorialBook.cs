using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialBook : MonoBehaviour
{
    public List<GameObject> pages;
    int currentPage;
    public FadeBlack black;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (currentPage < pages.Count - 1)
            {
                currentPage++;
                ChangePage(currentPage);
            }
            else
            {
                black.DoFade();
                GameObject.Find("MainMenuController").GetComponent<MainMenuController>().StartGame();
            }            
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (currentPage > 0)
            {
                currentPage--;
                ChangePage(currentPage);
            }
        }
    }

    void ChangePage(int index)
    {
        foreach(var p in pages)
        {
            p.SetActive(false);
        }
        pages[index].SetActive(true);        
    }
}
