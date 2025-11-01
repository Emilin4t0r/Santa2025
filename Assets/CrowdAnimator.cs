using System.Collections;
using UnityEngine;

public class CrowdAnimator : MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(AnimStarter());
    }

    IEnumerator AnimStarter()
    {
        yield return new WaitForSeconds(Random.Range(0.0f, 1.0f));
        anim.SetTrigger("Play");
        anim.speed = Random.Range(1.2f, 1.4f);
    }
}
