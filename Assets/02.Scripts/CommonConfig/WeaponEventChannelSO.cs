using System;
using UnityEngine;

/// <summary>
/// 무기 관련 이벤트를 관리하는 ScriptableObject
/// static 이벤트 대신 SO를 사용하여 메모리 누수를 방지하고 테스트 가능성을 높입니다.
/// Resources 폴더에 있는 인스턴스를 자동으로 로드합니다.
/// </summary>
[CreateAssetMenu(fileName = "WeaponEventChannel", menuName = "Events/Weapon Event Channel")]
public class WeaponEventChannelSO : ScriptableObject
{
    private static WeaponEventChannelSO _instance;

    /// <summary>
    /// Resources/WeaponEventChannel에서 자동으로 로드되는 싱글톤 인스턴스
    /// </summary>
    public static WeaponEventChannelSO Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<WeaponEventChannelSO>("WeaponEventChannel");

                if (_instance == null)
                {
                    Debug.LogError("WeaponEventChannel을 Resources 폴더에서 찾을 수 없습니다! " +
                        "Assets/Resources/WeaponEventChannel.asset 파일을 생성해주세요.");
                }
            }
            return _instance;
        }
    }

    // 이벤트 선언
    public event Action<int, int> OnAmmoChanged;
    public event Action<int> OnConsumableChanged;
    public event Action<float> OnReload;
    public event Action<Sprite> OnChangeWeapon;
    public event Action<Vector3> OnReboundCamera;

    // 이벤트 발생 메서드들
    public void RaiseAmmoChanged(int current, int reserve)
    {
        OnAmmoChanged?.Invoke(current, reserve);
    }

    public void RaiseConsumableChanged(int current)
    {
        OnConsumableChanged?.Invoke(current);
    }

    public void RaiseReload(float reloadTime)
    {
        OnReload?.Invoke(reloadTime);
    }

    public void RaiseChangeWeapon(Sprite weaponIcon)
    {
        OnChangeWeapon?.Invoke(weaponIcon);
    }

    public void RaiseReboundCamera(Vector3 weaponRebound)
    {
        OnReboundCamera?.Invoke(weaponRebound);
    }

    /// <summary>
    /// 에디터나 씬 전환 시 모든 이벤트 구독을 초기화합니다.
    /// </summary>
    private void OnDisable()
    {
        OnAmmoChanged = null;
        OnConsumableChanged = null;
        OnReload = null;
        OnChangeWeapon = null;
        OnReboundCamera = null;
    }
}
