
using UnityEngine;

public interface IWeapon
{
    void TryShoot();
    void TryReload();
    Sprite GetIcon();
}
