
using UnityEngine;

public interface IWeapon
{
    bool TryShoot();
    void TryReload();
    Sprite GetIcon();
}
