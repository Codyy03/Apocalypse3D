using UnityEngine;

public class BaseAnimationsController : MonoBehaviour
{
    protected string currentAnimation;

    protected Animator animator;

    [Tooltip("Czas przejœcia miêdzy animacjami (w sekundach)")]
    public float transitionTime = 0.1f;

    /// <summary>
    /// Zmienia animacjê, jeœli nie jest ju¿ odtwarzana. U¿ywa CrossFade dla p³ynnych przejœæ.
    /// </summary>
    /// <param name="animation">Nazwa animacji do odtworzenia</param>
    public void ChangeAnimation(string animation)
    {
        if (string.IsNullOrEmpty(animation)) return;

        if (currentAnimation == animation) return;

        animator.CrossFade(animation, transitionTime);
        currentAnimation = animation;
    }
}
