using UnityEngine;
using TMPro;
public class InteractionText : MonoBehaviour
{
    [Tooltip("Obiekt, przy którym wyœwietlany jest napis")]
    public Transform targetObject;

    [Tooltip("Offset od pozycji targetObject (np. aby napis by³ wy¿ej nad obiektem)")]
    public Vector3 offset = new Vector3(0f, 1.5f, 0f);

    [Tooltip("Szybkoœæ interpolacji pozycji napisu")]
    public float lerpSpeed = 10f;

    [Tooltip("Odleg³oœæ, w której gracz widzi etykietê interakcji")]
    public float displayDistance = 3f;

    [Tooltip("Transform gracza (np. jego g³owy lub œrodka cia³a)")]
    public Transform playerTransform;

    private RectTransform rectTransform;          // RectTransform etykiety UI
    private RectTransform canvasRectTransform;    // RectTransform Canvasu
    public Camera mainCamera;
    private TextMeshProUGUI labelText;           // Tekst wyœwietlany w UI

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        labelText = GetComponent<TextMeshProUGUI>();

        canvasRectTransform = transform.parent.GetComponent<RectTransform>();

    }

    void Update()
    {
        if (targetObject == null || mainCamera == null || canvasRectTransform == null || playerTransform == null)
            return;

        // Oblicz pozycjê nad obiektem
        Vector3 worldPosition = targetObject.position + offset;
        transform.position = worldPosition;

        // Obracanie UI w stronê kamery (billboarding)
        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);



    }

}

