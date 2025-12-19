using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerGunFire : PlayerComponent
{
    // 마우스의 왼쪽 버튼을 누르면 바라보는 방향으로 총을 발사하고 싶다. (총을 발사하고 싶다)
    [Header("Weapons")]
    [SerializeField] private List<Weapon> _weapons = new List<Weapon>();
    private Animator _animator;
    private Weapon _currentWeapon;

    protected override void Awake()
    {
        base.Awake();
        // 시작 시 모든 무기 비활성화
        foreach (var weapon in _weapons)
        {
            weapon.gameObject.SetActive(false);
        }
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (_weapons.Count > 0)
        {
            EquipWeapon(0);
        }
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return; // UI 클릭이므로 총을 쏘지 않음
        }
        if (!CanExecute()) return;

        if (_currentWeapon == null) return;

        // 1. 마우스 왼쪽 버튼이 눌린다면
        switch (_currentWeapon.FireMode)
        {
            case FireMode.SemiAuto:
                if (Input.GetMouseButtonDown(0))
                {
                    if (_currentWeapon.TryShoot())
                    {
                        _animator.SetTrigger("GunFire");
                    }
                }
                break;

            case FireMode.FullAuto:
                if (Input.GetMouseButton(0))
                {
                    if (_currentWeapon.TryShoot())
                    {
                        _animator.SetTrigger("GunFire");
                    }
                }
                break;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            _currentWeapon.TryReload();
        }
        for (int i = 0; i < _weapons.Count && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                EquipWeapon(i);
                break;
            }
        }
    }

    private void EquipWeapon(int index)
    {
        if (index < 0 || index >= _weapons.Count) return;

        _currentWeapon = _weapons[index];

        // 모든 무기 비활성화 후 현재 무기만 활성화
        for (int i = 0; i < _weapons.Count; i++)
        {
            _weapons[i].gameObject.SetActive(i == index);
        }

        WeaponEventChannelSO.Instance?.RaiseChangeWeapon(_currentWeapon.GetIcon());
    }
}
