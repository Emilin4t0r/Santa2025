using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DeathEvents());
    }

    IEnumerator DeathEvents()
    {
        SoundSpawner.SpawnSound(transform.position, transform.parent, SoundLibrary.GetClip("enemy_explode"), 0, 0.1f, 1f);
        Helpers.ExplosionSound(transform.position);
        yield return new WaitForSeconds(2);
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Scoreboard");
    }
}
