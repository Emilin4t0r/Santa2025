using UnityEngine;

public class MenuButtonSounds : MonoBehaviour
{
    public void PlayHoverSound()
    {
        SoundSpawner.SpawnSound(transform.position, null, SoundLibrary.GetClip("menu_button_hover"), 0, 0, 1f);
    }
    public void PlaySelectSound()
    {
        SoundSpawner.SpawnSound(transform.position, null, SoundLibrary.GetClip("menu_button_select"), 0, 0, 1f);
    }
}
