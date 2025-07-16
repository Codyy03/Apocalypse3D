using System;
using System.Collections.Generic;
using UnityEngine;
using FPSControllerLPFP;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Wszystkie bronie gracza")]
    public List<UsedWeapon> usedWeapons;
    public UsedWeapon lastSelectedWeapon;

    public bool noWeapons;
    [Serializable]
    public class UsedWeapon
    {
        public Weapon currentWeapon;
        public KeyCode button;
        public ShowItemDescription showItemDescription;
    }

    [Serializable]
    public class Weapon
    {
        public GameObject weapon;
        public Item weaponItem;
    }
    public List<Weapon> weapons;
    private FpsControllerLPFP fpsController;

    int currentWeaponIndex = 0;

    void Awake()
    {
        fpsController = GetComponent<FpsControllerLPFP>();
        lastSelectedWeapon = usedWeapons[0];
        usedWeapons[0].currentWeapon = weapons[0];
    }

    void Update()
    {
        if (GameManager.UIElementIsOpen || InteractionController.lootIsOpen) return;

        if (usedWeapons[0].currentWeapon == null && usedWeapons[1].currentWeapon == null)
            noWeapons = true;

        HandleWeaponSelection();
        SelectWeaponByScroll();
    }

    void SelectWeaponByScroll()
    {
        if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0f)
        {
            currentWeaponIndex = (currentWeaponIndex == 0) ? 1 : 0;
            
            if (usedWeapons[currentWeaponIndex].showItemDescription.itemInSlot != null)
                UpdateWeaponSelection();

            else currentWeaponIndex = (currentWeaponIndex == 0) ? 1 : 0;
        }
    }
    void UpdateWeaponSelection()
    {
        for (int i = 0; i < usedWeapons.Count; i++) 
        {
            bool isActive = (i == currentWeaponIndex);

            usedWeapons[i].currentWeapon.weapon.SetActive(isActive);

            if (isActive)
            {
                lastSelectedWeapon = usedWeapons[i];
                fpsController.arms = usedWeapons[i].currentWeapon.weapon.transform;
            }
        }
    }
    void HandleWeaponSelection()
    {
        for (int i = 0; i < usedWeapons.Count; i++)
        {
            if (Input.GetKeyDown(usedWeapons[i].button) && usedWeapons[i].showItemDescription.itemInSlot != null)
            {
                currentWeaponIndex = i;
                UpdateWeaponSelection();
                break;
            }
        }
    }
    public void ChangeWeapon(int weaponType, Item weapon)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].weaponItem == weapon)
            {
                
                usedWeapons[currentWeaponIndex].currentWeapon.weapon.SetActive(false);
                usedWeapons[weaponType].currentWeapon = weapons[i];
                usedWeapons[weaponType].currentWeapon.weapon.GetComponentInChildren<Gun>().damage = weapon.damage;
                usedWeapons[weaponType].currentWeapon.weapon.SetActive(true);
                lastSelectedWeapon = usedWeapons[weaponType];
                currentWeaponIndex = weaponType;
                fpsController.arms = usedWeapons[weaponType].currentWeapon.weapon.transform;
                return;
            }
        }
       
    }
}