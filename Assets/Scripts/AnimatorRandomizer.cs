using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorRandomizer : MonoBehaviour
{
    public RuntimeAnimatorController[] animationControllerArray;
    private Animator animator;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        if (!animator) {
            throw new UnityException("Animator randomizer needs an Animator");
        }

		animator.runtimeAnimatorController = animationControllerArray[Random.Range(0, animationControllerArray.Length)];
    }
}
