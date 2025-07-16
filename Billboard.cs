using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform target;                  // Kamera gracza
    public float smoothSpeed = 8f;            // Szybko�� wyg�adzania
    public bool onlyRotateAroundY = true;     // Czy obraca� tylko w poziomie?

    void Start()
    {
        if (target == null && Camera.main != null)
            target = Camera.main.transform;
    }

    void Update()
    {
        if (target == null) return;

        // Kierunek od obiektu do kamery
        Vector3 direction = transform.position - target.position;

        if (onlyRotateAroundY)
            direction.y = 0f; // Ignoruj pion (Zachowuje poziom)

        if (direction == Vector3.zero) return;

        // Wyznacz docelow� rotacj�
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // P�ynne przej�cie do rotacji
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
    }
}
