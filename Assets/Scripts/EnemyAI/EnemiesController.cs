using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesController : MonoBehaviour
{
    public static List<GameObject> enemiesAttacking;

    private void Start()
    {
        enemiesAttacking = new List<GameObject>();
    }
}
