using UnityEngine;

[ExecuteAlways]
public class LootIDAssignerZ : MonoBehaviour
{
    private static int nextAvailableID = 1;

    [SerializeField] private Loot lootComponent;

    [Tooltip("Prefix stylu lootu — zombie, wrak, skrzynia itd.")]
    [SerializeField] private string apocalypsePrefix = "loot";

    private void Awake()
    {
        if (lootComponent == null)
            lootComponent = GetComponent<Loot>();

        if (lootComponent != null && string.IsNullOrEmpty(lootComponent.lootID))
        {
            lootComponent.lootID = apocalypsePrefix + "_" + nextAvailableID.ToString("D3"); // np. loot_024
            nextAvailableID++;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(lootComponent); // zapisuj w prefabie
#endif
        }
    }
}