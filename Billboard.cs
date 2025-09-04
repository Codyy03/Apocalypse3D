using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void LateUpdate()
    {
        if (mainCamera == null || !mainCamera.gameObject.activeInHierarchy)
        {
            mainCamera = Camera.main;
        }

        if (!mainCamera) return;

        // obróæ do kamery (tylko w osi Y)
        Vector3 direction = mainCamera.transform.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(-direction);
    }
}
