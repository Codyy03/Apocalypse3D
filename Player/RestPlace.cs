using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class RestPlace : MonoBehaviour
{
    public Action onRestEvent;
    [SerializeField] float restTime = 1.5f;
    [SerializeField] GameObject notification;
    
    PlayerHealth playerHealth;

    bool canRest;

    bool isResting;
    void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!canRest) return;
        if (isResting) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            BlackScreenController.instance.ActivateBlackScreen(restTime);
            
            // wywo³uje event
            onRestEvent?.Invoke();

            // resetuje event
            onRestEvent = null;

            playerHealth.RegeneratePlayerHealth();
            
            isResting = true;
            
            StartCoroutine(IsResting());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canRest = true;

            if (enabled)
                notification.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        canRest = false;
        notification.SetActive(false);
    }

    IEnumerator IsResting()
    {
        yield return new WaitForSeconds(restTime);
        isResting = false;
    }
}
