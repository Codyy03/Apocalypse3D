using System;
using UnityEngine;

public static class AmmunitionStorage
{
    public static int handgunAmmo = 30;

    public static int rifleAmmo = 200;

    public static int sniperAmmo = 50;

    public static Action onHandganAmmoChange;
    public static Action onRifleAmmoChange;

    public static Action onSniperAmmoChange;
    public static void ChangeHandgunAmmo(int value)
    {
        handgunAmmo = Mathf.Clamp(handgunAmmo + value, 0, 10000);

        onHandganAmmoChange?.Invoke();
    }
    public static void ChangeRifleAmmo(int value)
    {
        rifleAmmo = Mathf.Clamp(rifleAmmo + value, 0, 10000);

        onRifleAmmoChange?.Invoke();
    }

    public static void ChangeSniperAmmo(int value)
    {
        sniperAmmo = Mathf.Clamp(sniperAmmo + value, 0, 10000);

        onSniperAmmoChange?.Invoke();
    }
}
