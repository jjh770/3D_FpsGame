using System;
using UnityEngine;

[Serializable]
// 무기의 '스탯'을 관리하는 컴포넌트
public class WeaponStats
{
    [SerializeField] private Sprite _spriteIcon;
    [SerializeField] private ResourceStat _bulletCount;
    [SerializeField] private ResourceStat _bulletClipCount;
    [SerializeField] private float _reloadTime = 1.6f;
    [SerializeField] private float _coolTime = 0.1f;

    public Sprite SpriteIcon => _spriteIcon;
    public ResourceStat BulletCount => _bulletCount;
    public ResourceStat BulletClipCount => _bulletClipCount;
    public float ReloadTime => _reloadTime;
    public float CoolTime => _coolTime;
    public bool IsReloading { get; set; }

    public void Initialize()
    {
        _bulletCount.Initialize();
        _bulletClipCount.Initialize();
    }
}
