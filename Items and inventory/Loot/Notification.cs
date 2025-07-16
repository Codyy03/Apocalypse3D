using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notification : MonoBehaviour
{

    public void SwitchNotification(GameObject notification, bool status)
    {
        notification.SetActive(status);
    }
}
