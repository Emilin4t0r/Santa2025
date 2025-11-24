using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    public Transform camPivot, santaModel;

    void Start()
    {
        StartCoroutine(DeathEvents());
    }

    private void Update()
    {
        camPivot.LookAt(santaModel);
    }

    IEnumerator DeathEvents()
    {
        SoundSpawner.SpawnSound(transform.position, transform.parent, SoundLibrary.GetClip("enemy_explode"), 0, 0.1f, 1f);
        Helpers.ExplosionSound(transform.position);
        GameObject.Find("ToBlack").GetComponent<FadeBlack>().DoFade();
        yield return new WaitForSeconds(3);
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Scoreboard");
    }
}
