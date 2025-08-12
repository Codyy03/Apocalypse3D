using UnityEngine;

[RequireComponent(typeof(LimitTheCameraRange))]
public class MapDragController : MonoBehaviour
{
    [SerializeField] float dragSpeed;
    Vector3 lastMousePosition;
    LimitTheCameraRange limitThecameraRange;
    Camera cam;


    private void Start()
    {
        limitThecameraRange = GetComponent<LimitTheCameraRange>();
        cam = GetComponent<Camera>();
    }
    private void LateUpdate()
    {
        limitThecameraRange.LimitCameraRange();
    }
    // Update is called once per frame
    void Update()
    {
        // Pocz¹tek przeci¹gania
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        // Podczas przeci¹gania
        if (Input.GetMouseButton(0))
        {
            Vector3 currentPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = lastMousePosition - currentPos;

            // Przesuñ kamerê w przeciwn¹ stronê ni¿ porusza siê mysz
            transform.position += new Vector3(direction.x, 0, direction.z);
        }
    }
}
