using UnityEngine;

public class MapCameraMovement : MonoBehaviour
{
    [SerializeField] float cameraSpeed;

    void Update()
    {
        float vertivalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        Vector3 movement = new Vector3(vertivalInput, 0 , horizontalInput);

        transform.Translate(-movement * cameraSpeed * Time.unscaledDeltaTime, Space.World);

    }
}
