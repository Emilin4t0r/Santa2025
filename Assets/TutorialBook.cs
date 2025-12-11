using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TutorialBook : MonoBehaviour
{
    public List<GameObject> pages;
    int currentPage;
    public UnityEvent closeBook;

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
                closeBook.Invoke();
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
        SoundSpawner.SpawnSound(transform.position, null, SoundLibrary.GetClip("menu_button_select"), 0, 0.05f);
        foreach (var p in pages)
        {
            p.SetActive(false);
        }
        pages[index].SetActive(true);        
    }
}
