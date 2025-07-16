using UnityEngine;

public static class AmmunitionStorage
{
    public static int handgunAmmo = 30;

    public static int rifleAmmo = 200;


    public static void ChangeHandgunAmmo(int value)
    {
        handgunAmmo = Mathf.Clamp(handgunAmmo + value, 0, 10000);
    }
    public static void ChangeRifleAmmo(int value)
    {
        rifleAmmo = Mathf.Clamp(rifleAmmo + value, 0, 10000);
    }
}
