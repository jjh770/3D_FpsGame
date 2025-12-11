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
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _coolTime = 0.1f;
    [SerializeField] private float _reloadTime = 1.6f;

    [Header("Rebound")]
    [SerializeField] private float _reboundAmount = 1f;
    [SerializeField] private float _reboundSpeed = 10f;
    [SerializeField] private float _reboundRecover = 10f;
    [SerializeField] private Vector2 _reboundRotation = new Vector2(-2f, 1f);
    [SerializeField] private float _knockbackAmount = 10f;

    [Header("Ammo Capacity")]
    [SerializeField] private int _maxBulletCount = 30;
    [SerializeField] private int _maxBulletClipCount = 120;

    public FireMode FireMode => _fireMode;
    public Sprite SpriteIcon => _spriteIcon;
    public float Damage => _damage;
    public float CoolTime => _coolTime;
    public float ReloadTime => _reloadTime;
    public int MaxBulletCount => _maxBulletCount;
    public int MaxBulletClipCount => _maxBulletClipCount;

    public float ReboundAmount => _reboundAmount;
    public float ReboundSpeed => _reboundSpeed;
    public float ReboundRecover => _reboundRecover;
    public Vector2 ReboundRotation => _reboundRotation;
    public float KnockbackAmount => _knockbackAmount;

    public Vector3 CalculateRebound()
    {
        float reboundX = _reboundRotation.x * _reboundAmount;
        float reboundY = Random.Range(-_reboundRotation.y, _reboundRotation.y) * _reboundAmount;
        return new Vector3(reboundX, reboundY, 0f);
    }
}
