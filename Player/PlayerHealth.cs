using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerHealth : MonoBehaviour
{
    [Tooltip("Maksymalne �ycie gracza")]
    public float maxHealth = 100;

    [Tooltip("Reprezentacja graficzna poziomu �ycia gracza")]
    public Image healthBar;
    public Image healthBarInInventory;
    public TextMeshProUGUI healthText;

    [SerializeField] SetArmorFromInventory setArmorFromInventory;
    [SerializeField] GameObject vest;
    TextMeshProUGUI durabilityText;
    
    float currentHealth;

    Inventory inventory;
    private void Awake()
    {
        inventory = FindFirstObjectByType<Inventory>();
        durabilityText = vest.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        currentHealth = maxHealth;

        healthText.text = $"{currentHealth} / {maxHealth}";
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.H))
        {
            vest.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.H))
        {
            vest.SetActive(false);
        }
    }
    /// <summary>
    /// zmie� poziom zycia gracza
    /// </summary>
    /// <param name="value"></param>
    public void ChangePlayerHealth(float value)
    {
        if (value < 0)
            value = ApplyArmorProtection(value);
        
        currentHealth = Mathf.Clamp(currentHealth + value, 0f, maxHealth);
        healthBar.fillAmount = currentHealth / maxHealth;

        healthBarInInventory.fillAmount = currentHealth / maxHealth;

        healthText.text = $"{currentHealth} / {maxHealth}";
    }

    public void RegeneratePlayerHealth()
    {
        ChangePlayerHealth(maxHealth);
    }
    /// <summary>
    /// zastosuj pancerz 
    /// </summary>
    /// <param name="damage">obra�enia</param>
    /// <returns></returns>
    float ApplyArmorProtection(float damage)
    {
        if (setArmorFromInventory == null || setArmorFromInventory.durability <= 1f)
            return damage;

        // Pobierz maksymaln� trwa�o�� pancerza na podstawie danych przedmiotu z ekwipunku
        float maxDurability = inventory.GetItem(setArmorFromInventory.actualUseID).durability;

        // Oblicz aktualny procent trwa�o�ci (0.0 � 1.0), uwzgl�dniaj�c maksymaln� warto��
        float durabilityRatio = Mathf.Clamp01(setArmorFromInventory.durability / maxDurability);

        // Pobierz efektywno�� pancerza (np. 0.4 oznacza redukcj� 40% obra�e� przy pe�nej trwa�o�ci)
        float armorEfficiency = inventory.GetItem(setArmorFromInventory.actualUseID).armor;

        // Po��cz trwa�o�� i efektywno�� pancerza w jeden wsp�czynnik redukcji
        float reductionRatio = durabilityRatio * armorEfficiency;

        // Oblicz ostateczne obra�enia po uwzgl�dnieniu redukcji pancerza
        float reducedDamage = damage * (1f - reductionRatio);

        // Oblicz ile trwa�o�ci straci kamizelka w wyniku przyj�cia ciosu
        float durabilityLoss = Mathf.Abs(damage) * armorEfficiency;

        // Zmniejsz trwa�o�� pancerza o obliczon� warto��
        setArmorFromInventory.ChangeDurablility(-durabilityLoss);

        // Zaktualizuj interfejs gracza � poka� aktualny poziom trwa�o�ci kamizelki
        SetVestDurabilityTextInUI(setArmorFromInventory.durability);

        return reducedDamage;
    }
    /// <summary>
    /// ustawia warto�� pancerza w UI
    /// </summary>
    /// <param name="value"></param>
    public void SetVestDurabilityTextInUI(float value)
    {
        durabilityText.text = $"Trwa�o��: {value}";
    }
    public float GetHealth() => currentHealth;

    public void SetHealth(float value)
    {
        currentHealth = Mathf.Clamp(value, 0f, maxHealth);
        healthBar.fillAmount = currentHealth / maxHealth;
    }
}