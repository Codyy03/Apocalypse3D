using UnityEngine;
using UnityEngine.SceneManagement;

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
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "CityRoad": SetBorderValues(new Vector2(-150f, -140f ), new Vector2(300f, 200f)); break;
            case "Beginning": SetBorderValues(new Vector2(- 143.8696f, -96.69377f), new Vector2(-106.3538f, -91.37058f)); break;
        }
    }

    void SetBorderValues(Vector2 min, Vector2 max)
    {
          minBounds = min;
          maxBounds = max;
    }
}
