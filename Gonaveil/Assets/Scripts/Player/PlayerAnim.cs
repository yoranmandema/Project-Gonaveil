using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    public PlayerController playerMovement;

    private Animator animator;

    void Start () {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetBool("IsCrouching", playerMovement.isCrouching);
        animator.SetFloat("MoveSpeed", playerMovement.velocity.sqrMagnitude);
        if (playerMovement.isSliding) {
            animator.SetTrigger("Slide");
        }
    }
}
