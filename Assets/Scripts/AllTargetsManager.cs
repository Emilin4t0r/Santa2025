using System.Collections.Generic;
using UnityEngine;

public class AllTargetsManager : MonoBehaviour
{
    public static AllTargetsManager instance;
    
    public List<Transform> targets = new List<Transform>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        
    }

    public Transform GetRandomTarget(Transform exclude = null)
    {
        if (targets == null || targets.Count == 0)
            return null;

        // If there's only one target and it's the excluded one → nothing valid
        if (exclude != null && targets.Count == 1 && targets[0] == exclude)
            return null;

        Transform target;
        do
        {
            int index = Random.Range(0, targets.Count);
            target = targets[index];
        }
        while (target == exclude);

        return target;
    }
}
