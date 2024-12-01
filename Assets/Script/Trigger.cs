using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public PlayerController controller;
    public Animator animator;
    public IEnumerator EndAnimation(int seconds)
    {
        //move camera back
        //cam.transform.DOMove(new Vector3(0, 12, 0), 1);
        //cam.transform.DORotate(new Vector3(90, 0, 0), 1);
        yield return new WaitForSeconds(seconds);
        controller._canThrow = true;
        controller._isAnimPlaying = false;
        controller.playerAnimation.SetTrigger("Reset");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!animator && !other.CompareTag("Player")) return;
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(2.5f);
        animator.SetTrigger("Play");
    }
}
