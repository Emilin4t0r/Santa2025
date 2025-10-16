using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour
{
    public List<AudioClip> sounds;
    public static List<AudioClip> staticSounds;

    private void Awake()
    {
        staticSounds = sounds;
    }

    public static AudioClip GetClip(string clipName)
    {
        return staticSounds.Find(item => item.name == clipName);        
    }
}
