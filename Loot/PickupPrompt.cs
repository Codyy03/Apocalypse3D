using TMPro;
using UnityEngine;

public class PickupPrompt : MonoBehaviour
{
    public Transform targetWorld; // obiekt w �wiecie
    public TextMeshProUGUI uiText; // referencja do TMP w Canvasie

    // Update is called once per frame
    void Update()
    {
        if (targetWorld == null) return;

        // Pozycja na ekranie
        Vector3 screenPos = Camera.main.WorldToScreenPoint(targetWorld.position);

        // Je�li obiekt jest przed kamer�
        if (screenPos.z > 0)
        {
            uiText.transform.position = screenPos;
            uiText.enabled = true;
        }
        else
        {
            uiText.enabled = false; // za kamer� = ukryj
        }

    }
}
