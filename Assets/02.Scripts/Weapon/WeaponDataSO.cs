using UnityEngine;
public enum FireMode
{
    SemiAuto,
    FullAuto,
}

/// <summary>
/// 무기의 불변 설정 데이터 (템플릿)
/// 여러 무기 인스턴스가 이 데이터를 공유함
/// </summary>
[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon System/Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    [Header("Visual")]
    [SerializeField] private FireMode _fireMode = FireMode.FullAuto;
    [SerializeField] private Sprite _spriteIcon;

    [Header("Fire Settings")]
    [SerializeField] private float _coolTime = 0.1f;
    [SerializeField] private float _reloadTime = 1.6f;

    [Header("Ammo Capacity")]
    [SerializeField] private int _maxBulletCount = 30;
    [SerializeField] private int _maxBulletClipCount = 120;

    public Sprite SpriteIcon => _spriteIcon;
    public FireMode FireMode => _fireMode;
    public float CoolTime => _coolTime;
    public float ReloadTime => _reloadTime;
    public int MaxBulletCount => _maxBulletCount;
    public int MaxBulletClipCount => _maxBulletClipCount;
}
