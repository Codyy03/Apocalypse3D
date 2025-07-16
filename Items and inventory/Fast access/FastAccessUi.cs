using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FastAccessUi : MonoBehaviour
{
    public TextMeshProUGUI medicineName;
    public TextMeshProUGUI medicineQuantity;
    public TextMeshProUGUI medicineHp;

    public Image icon;
    public Sprite nativeIcon;
    public void UpdateFastAccess(string medicineQuantity)
    {
        this.medicineQuantity.text = "x" + medicineQuantity;
    }
    public void ResetValues()
    {
        medicineHp.text = "";
        medicineQuantity.text = "";
        medicineName.text = "Leki";
        icon.sprite = nativeIcon;


    }
}
