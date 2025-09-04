using System;
using UnityEngine;

public class PlayerEnterPlace : MonoBehaviour
{
    public Action playerEnterField;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEnterField?.Invoke();

            playerEnterField = null;
        }
    }
}
