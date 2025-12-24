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

    private EZoomMode _zoomMode = EZoomMode.Normal;
    [SerializeField] private GameObject _normalCrosshair;
    [SerializeField] private GameObject _zoomInCrosshair;
    [SerializeField] private float _zoomInFOV = 10f;
    [SerializeField] private float _normalFOV = 60f;

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
        CanShoot();
        Shooting();
        ZoomModeCheck();
        Reloading();
        WeaponSwitching();
    }

    private void Shooting()
    {
        bool shouldFire = false;

        // FireMode에 따라 다른 입력 처리
        if (_currentWeapon.FireMode == FireMode.SemiAuto)
        {
            // 반자동: 클릭마다 한 발
            shouldFire = Input.GetMouseButtonDown(0);
        }
        else if (_currentWeapon.FireMode == FireMode.FullAuto)
        {
            // 완전 자동: 누르고 있는 동안 연사
            shouldFire = Input.GetMouseButton(0);
        }

        if (shouldFire && _currentWeapon.TryShoot())
        {
            _animator.SetTrigger("GunFire");
        }
    }
    private void WeaponSwitching()
    {
        for (int i = 0; i < _weapons.Count && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                WeaponEventChannelSO.Instance.RaiseReloadCancel();
                EquipWeapon(i);
                break;
            }
        }
    }

    private void Reloading()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _currentWeapon.TryReload();
        }
    }

    private bool CanShoot()
    {
        return !EventSystem.current.IsPointerOverGameObject()
            && CanExecute()
            && _currentWeapon != null;
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

    private void ZoomModeCheck()
    {
        if (Input.GetMouseButton(1))
        {
            _zoomMode = EZoomMode.ZoomIn;
            _normalCrosshair.SetActive(false);
            _zoomInCrosshair.SetActive(true);
            Camera.main.fieldOfView = _zoomInFOV;
        }
        else
        {
            _zoomMode = EZoomMode.Normal;
            _normalCrosshair.SetActive(true);
            _zoomInCrosshair.SetActive(false);
            Camera.main.fieldOfView = _normalFOV;
        }
    }
}
