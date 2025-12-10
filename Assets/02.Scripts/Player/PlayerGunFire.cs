using UnityEngine;

public class PlayerGunFire : MonoBehaviour
{
    // 마우스의 왼쪽 버튼을 누르면 바라보는 방향으로 총을 발사하고 싶다. (총을 발사하고 싶다)
    [Header("Weapons")]
    [SerializeField] private Weapon_Gun _weaponGun;
    [SerializeField] private Weapon_Rifle _weaponRifle;

    private IWeapon _currentWeapon;

    private void Start()
    {
        EquipWeapon(_weaponGun);
    }

    private void Update()
    {
        if (_currentWeapon == null) return;

        // 1. 마우스 왼쪽 버튼이 눌린다면
        if (_currentWeapon == _weaponGun)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _currentWeapon.TryShoot();
            }
        }
        else if (_currentWeapon == _weaponRifle)
        {
            if (Input.GetMouseButton(0))
            {
                _currentWeapon.TryShoot();
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            _currentWeapon.TryReload();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipWeapon(_weaponGun);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipWeapon(_weaponRifle);
        }
    }

    private void EquipWeapon(IWeapon weapon)
    {
        _currentWeapon = weapon;

        WeaponEvents.TriggerChangeIcon(weapon.GetIcon());

        // 무기 오브젝트 활성화/비활성화
        _weaponGun.gameObject.SetActive(_weaponGun == weapon);
        _weaponRifle.gameObject.SetActive(_weaponRifle == weapon);
    }
}
