using NUnit.Framework;
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
    public string lookAround;

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

    void AssertNotNullOrEmpty(string value, string fieldName)
    {
        if (string.IsNullOrEmpty(value))
            Debug.LogError($"{fieldName} nie jest ustawione w {gameObject.name}");
    }

    private void OnValidate()
    {
        AssertNotNullOrEmpty(idle, nameof(idle));
        AssertNotNullOrEmpty(walk, nameof(walk));
        AssertNotNullOrEmpty(run, nameof(run));
        AssertNotNullOrEmpty(dead, nameof(dead));
        AssertNotNullOrEmpty(attack, nameof(attack));
        AssertNotNullOrEmpty(lookAround, nameof(lookAround));
    }
}

