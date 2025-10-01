using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwarmMissiles : MonoBehaviour
{
    public List<SwarmMissile> missiles;

    private void Awake()
    {
        missiles = new List<SwarmMissile>();
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }
    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
    void OnSceneChanged(Scene old, Scene now)
    {
        if (now.name == "Gameplay Test")
        {
            GetMissilesFromChildren();
        }
    }

    void GetMissilesFromChildren()
    {
        missiles = new List<SwarmMissile>();
        SwarmMissile[] _missiles = GetComponentsInChildren<SwarmMissile>();

        foreach (SwarmMissile msl in _missiles)
        {
            missiles.Add(msl);
        }
    }

    private void Update()
    {
        if (missiles.Count <= 0)
            return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireMissile();
        }
    }

    void FireMissile()
    {
        SwarmMissile msl = missiles[0];
        msl.enabled = true;
        msl.transform.parent = null;
        missiles.Remove(msl);
    }
}
