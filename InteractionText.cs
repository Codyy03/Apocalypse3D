using UnityEngine;
using TMPro;
public class InteractionText : MonoBehaviour
{
    [Tooltip("Obiekt, przy kt�rym wy�wietlany jest napis")]
    public Transform targetObject;

    [Tooltip("Offset od pozycji targetObject (np. aby napis by� wy�ej nad obiektem)")]
    public Vector3 offset = new Vector3(0f, 1.5f, 0f);

    [Tooltip("Szybko�� interpolacji pozycji napisu")]
    public float lerpSpeed = 10f;

    [Tooltip("Odleg�o��, w kt�rej gracz widzi etykiet� interakcji")]
    public float displayDistance = 3f;

    [Tooltip("Transform gracza (np. jego g�owy lub �rodka cia�a)")]
    public Transform playerTransform;

    private RectTransform rectTransform;          // RectTransform etykiety UI
    private RectTransform canvasRectTransform;    // RectTransform Canvasu
    public Camera mainCamera;
    private TextMeshProUGUI labelText;           // Tekst wy�wietlany w UI

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

        // Oblicz pozycj� nad obiektem
        Vector3 worldPosition = targetObject.position + offset;
        transform.position = worldPosition;

        // Obracanie UI w stron� kamery (billboarding)
        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);



    }

}

