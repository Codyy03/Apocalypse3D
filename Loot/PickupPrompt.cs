using TMPro;
using UnityEngine;

public class PickupPrompt : MonoBehaviour
{
    public Transform targetWorld; // obiekt w œwiecie
    public TextMeshProUGUI uiText; // referencja do TMP w Canvasie

    // Update is called once per frame
    void Update()
    {
        if (targetWorld == null) return;

        // Pozycja na ekranie
        Vector3 screenPos = Camera.main.WorldToScreenPoint(targetWorld.position);

        // Jeœli obiekt jest przed kamer¹
        if (screenPos.z > 0)
        {
            uiText.transform.position = screenPos;
            uiText.enabled = true;
        }
        else
        {
            uiText.enabled = false; // za kamer¹ = ukryj
        }

    }
}
