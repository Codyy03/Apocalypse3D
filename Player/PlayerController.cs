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

        HandleWeaponSelection();
        SelectWeaponByScroll();
    }
    // za pomoca scrolla zmien broñ
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
    // zaktualizauj wyci¹gniêt¹ bron przez gracza
    void UpdateWeaponSelection()
    {
        for (int i = 0; i < usedWeapons.Count; i++) 
        {
            bool isActive = (i == currentWeaponIndex);

            if (usedWeapons[i].currentWeapon != null) 
                usedWeapons[i].currentWeapon.weapon.SetActive(isActive);

            if (isActive)
            {
                lastSelectedWeapon = usedWeapons[i];
                fpsController.arms = usedWeapons[i].currentWeapon.weapon.transform;
            }
        }
    }
    // wyciaga tylko te bron, która jest dostepna
    public void UpdateWeaponAfterInventoryChange(int weaponType)
    {
        if(!noWeapons && usedWeapons[weaponType].showItemDescription.itemInSlot != null)
        {
            currentWeaponIndex = weaponType;
            UpdateWeaponSelection();
        }
    }
    // wybiera za pomoca klawiszy bron gracza
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
    // usuwa bron
    public void RemoveWeapon(int weponType)
    {

        if (weponType == 0 && usedWeapons[1].currentWeapon.weapon == null)
        {
            noWeapons = true;
            usedWeapons[0].currentWeapon.weapon.GetComponentInChildren<Gun>().HideWeapon();
            return;
        }
        if (weponType == 1 && usedWeapons[0].currentWeapon.weapon == null)
        {
            noWeapons = true;
            usedWeapons[1].currentWeapon.weapon.GetComponentInChildren<Gun>().HideWeapon();
            return;
        }
        usedWeapons[weponType].currentWeapon.weapon.SetActive(false);
        usedWeapons[weponType].currentWeapon = null;

    }
    void DisableWeapon(int weponType)
    {
        if (weponType == 0 && usedWeapons[1].currentWeapon.weapon == null)
        {
            noWeapons = true;
            usedWeapons[0].currentWeapon.weapon.GetComponentInChildren<Gun>().HideWeapon();
            return;
        }
        if (weponType == 1 && usedWeapons[0].currentWeapon.weapon == null)
        {
            noWeapons = true;
            usedWeapons[1].currentWeapon.weapon.GetComponentInChildren<Gun>().HideWeapon();
            return;
        }
    }
    // zmienia bron i wyciaga nowo za³o¿on¹
    public void ChangeWeapon(int weaponType, Item weapon)
    {
        Gun gun;
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].weaponItem == weapon)
            {
                
                usedWeapons[currentWeaponIndex].currentWeapon.weapon.SetActive(false);
                usedWeapons[weaponType].currentWeapon = weapons[i];

                gun = usedWeapons[weaponType].currentWeapon.weapon.GetComponentInChildren<Gun>();
                gun.damage = weapon.damage;

                usedWeapons[weaponType].currentWeapon.weapon.SetActive(true);

                lastSelectedWeapon = usedWeapons[weaponType];
                currentWeaponIndex = weaponType;
                fpsController.arms = usedWeapons[weaponType].currentWeapon.weapon.transform;

                noWeapons = false;
                gun.ShowWeapon();

                return;
            }
        }
       
    }
}