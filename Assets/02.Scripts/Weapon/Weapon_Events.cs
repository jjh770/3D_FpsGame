using System;
using UnityEngine;

public static class WeaponEvents
{
    public static event Action<int, int> OnAmmoChanged;
    public static event Action<int> OnConsumableChanged;
    public static event Action<float> OnReload;
    public static event Action<Sprite> OnChangeWeapon;
    public static event Action<Vector3> OnReboundCamera;
    public static void TriggerAmmoChanged(int current, int reserve)
    {
        OnAmmoChanged?.Invoke(current, reserve);
    }

    public static void TriggerConsumableChanged(int current)
    {
        OnConsumableChanged?.Invoke(current);
    }

    public static void TriggerReload(float reloadTime)
    {
        OnReload?.Invoke(reloadTime);
    }

    public static void TriggerChangeIcon(Sprite weaponIcon)
    {
        OnChangeWeapon?.Invoke(weaponIcon);
    }

    public static void TriggerRebound(Vector3 weaponRebound)
    {
        OnReboundCamera?.Invoke(weaponRebound);
    }
}
