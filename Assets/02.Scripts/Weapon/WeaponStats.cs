using UnityEngine;

// 무기의 '스탯'을 관리하는 컴포넌트
public class WeaponStats : MonoBehaviour
{
    [SerializeField] private ResourceStat _bullet;
    [SerializeField] private ResourceStat _bulletClipCount;
    [SerializeField] private ResourceStat _bombCount;
    [SerializeField] private float _reloadTime = 1.6f;
    [SerializeField] private float _coolTime = 0.1f;

    public ResourceStat BulletCount => _bullet;
    public ResourceStat BulletClipCount => _bulletClipCount;
    public ResourceStat BombCount => _bombCount;
    public float ReloadTime => _reloadTime;
    public float CoolTime => _coolTime;
    public bool IsReloading { get; set; }

    private void Start()
    {
        _bullet.Initialize();
        _bulletClipCount.Initialize();
        _bombCount.Initialize();
    }
}
