using System.Collections.Generic;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    [SerializeField] float distanceToEneable = 100;
    [SerializeField] float checkInterval = 0.5f;
    [SerializeField] List<GameObject> locations = new();
    
    Transform player;

    float sqrDistanceToEnable;
    float nextCheckTime;
    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>().transform;
        sqrDistanceToEnable = distanceToEneable * distanceToEneable;
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;
            CheckLocations();
        }
    }

    void CheckLocations()
    {
        Vector3 playerPos = player.position;

        foreach (GameObject l in locations)
        {
            float sqrDist = (l.transform.position - playerPos).sqrMagnitude;
            bool shouldBeActive = sqrDist <= sqrDistanceToEnable;

            if (l.activeSelf != shouldBeActive)
                l.SetActive(shouldBeActive);
        }
    }
}
