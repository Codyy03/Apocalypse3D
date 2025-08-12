using UnityEngine;

public class LimitTheCameraRange : MonoBehaviour
{
    [SerializeField] Vector2 minBounds;
    [SerializeField] Vector2 maxBounds;

    public void LimitCameraRange()
    {
        Vector3 clampedPosition = transform.localPosition;

        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, minBounds.y, maxBounds.y);

        transform.localPosition = clampedPosition;
    }
}
