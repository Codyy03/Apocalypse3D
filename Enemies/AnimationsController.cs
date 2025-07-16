using UnityEngine;
using UnityEngine.InputSystem.HID;

public class AnimationsController : MonoBehaviour
{
    [Header("Nazwy animacji")]
    public string idle;
    public string walk;
    public string run;
    public string dead;
    public string attack;
    [SerializeField] string[] hitsAniamtions;
    public string hit;


    string currentAnimation;

    private Animator animator;

    [Tooltip("Czas przej�cia mi�dzy animacjami (w sekundach)")]
    public float transitionTime = 0.1f;

    [SerializeField] GameObject attackPoint;

    private void Awake()
    {
        hit = GetRandomHitAniamtion();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Zmienia animacj�, je�li nie jest ju� odtwarzana. U�ywa CrossFade dla p�ynnych przej��.
    /// </summary>
    /// <param name="animation">Nazwa animacji do odtworzenia</param>
    public void ChangeAnimation(string animation)
    {
        if (string.IsNullOrEmpty(animation)) return;

        if (currentAnimation == animation) return;

        animator.CrossFade(animation, transitionTime);
        currentAnimation = animation;
    }
    string GetRandomHitAniamtion()
    {
        return hitsAniamtions[Random.Range(0, hitsAniamtions.Length)];
    }
    public Animator GetAnimator()
    {
        return animator;
    }

    public void EnableAttack()
    {
        attackPoint.SetActive(true);

    }
    public void DisableAttack()
    {
        attackPoint.SetActive(false);

    }
}

