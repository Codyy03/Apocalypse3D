using System;
using System.Collections;
using UnityEngine;

public class BlackScreenController : MonoBehaviour 
{
    public static BlackScreenController instance;

    public bool blackScreenIsActive;
    public Action onBlackScreenActivate;
    
    [SerializeField] GameObject blackScrren;

    [Tooltip("Wszystkie elementy UI")]
    [SerializeField] GameObject allUI;
    
    Animator animator;
    private void Awake()
    {
        animator = blackScrren.GetComponent<Animator>();
        instance = this;
    }
    /// <summary>
    /// ustawia na okreœlony czas czarny okran gry
    /// </summary>
    /// <param name="time">czas, w którym ma byæ aktywne t³o</param>
    public void ActivateBlackScreen(float time)
    {
        blackScreenIsActive = true;

        blackScrren.SetActive(true);

        allUI.SetActive(false);

        animator.Play("OnEnableAniamtion");

        StartCoroutine(ShowBlackScrren(time));
    }
    IEnumerator ShowBlackScrren(float time)
    {
        yield return new WaitForSeconds(time);
        blackScreenIsActive = false;

        allUI.SetActive(true);

        blackScrren.SetActive(false);
        onBlackScreenActivate?.Invoke();

        onBlackScreenActivate = null;
    }
}
