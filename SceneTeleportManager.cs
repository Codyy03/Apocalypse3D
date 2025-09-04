using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleportManager : MonoBehaviour
{
    [SerializeField] int sceneIndex;
    [SerializeField] GameObject notification;
    bool enterTriggerPoint;

    void Update()
    {
        if (!enterTriggerPoint) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(2);

            SceneLoadData.sceneToLoadIndex = sceneIndex;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            enterTriggerPoint = true;
            notification.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        notification.SetActive(false);
        enterTriggerPoint = false;
    }
}
