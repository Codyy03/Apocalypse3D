using UnityEngine;
using TMPro;

public class OverlayNameplate : MonoBehaviour
{
    [Header("Referencje")]
    public Transform anchor;
    public Camera mainCamera;
    public Canvas canvas;
    public TextMeshProUGUI nameText;

    [Header("Offset w UI")]
    public Vector2 pixelOffset = new Vector2(0, 40f);

    [Header("Stabilizacja")]
    [SerializeField] private float smoothSpeed = 10f; // wi�ksze = szybciej dogania

    private RectTransform rect;
    private RectTransform canvasRect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (!canvas) canvas = GetComponentInParent<Canvas>();
        if (!mainCamera) mainCamera = Camera.main;
        if (canvas) canvasRect = canvas.GetComponent<RectTransform>();

        if (canvas && canvas.renderMode == RenderMode.ScreenSpaceCamera)
            canvas.worldCamera = mainCamera;
    }

    void LateUpdate()
    {
        if (!anchor || !canvasRect || !mainCamera) return;

        // je�li kamera pad�a � podmie� na Camera.main
        if (!mainCamera.gameObject.activeInHierarchy)
        {
            mainCamera = Camera.main;
            if (canvas && canvas.renderMode == RenderMode.ScreenSpaceCamera)
                canvas.worldCamera = mainCamera;
        }

        // oblicz pozycj� w ekranie
        Vector3 screenPos = mainCamera.WorldToScreenPoint(anchor.position);

        bool visible = screenPos.z > 0f;
        if (nameText) nameText.enabled = visible;
        if (!visible) return;

        // zamie� na pozycj� w Canvasie
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            mainCamera,
            out localPoint
        );

        Vector2 targetPos = localPoint + pixelOffset;

        // p�ynne doganianie (LERP od aktualnej pozycji do targetu)
        rect.anchoredPosition = Vector2.Lerp(
            rect.anchoredPosition,
            targetPos,
            Time.deltaTime * smoothSpeed
        );
    }
}
