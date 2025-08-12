using UnityEngine;
using UnityEngine.InputSystem.HID;

public class ZombieAnimationsController : BaseAnimationsController
{
    [Header("Nazwy animacji")]
    public string idle;
    public string walk;
    public string run;
    public string dead;
    public string attack;

    [SerializeField] string[] hitsAniamtions;

    public string hit;

    [SerializeField] GameObject attackPoint;

    private void Awake()
    {
        hit = GetRandomHitAniamtion();
        animator = GetComponent<Animator>();
    }
    string GetRandomHitAniamtion()
    {
        return hitsAniamtions[Random.Range(0, hitsAniamtions.Length)];
    }
    public Animator GetAnimator()
    {
        return animator;
    }

    public void EnableAttack() => attackPoint.SetActive(true);

    public void DisableAttack() => attackPoint.SetActive(false);

}

