using UnityEngine;

public class BasicMotionAnimations : BaseAnimationsController
{
    [Header("Animacje")]
    public string idle;
    public string walk;
    public string death;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
}
