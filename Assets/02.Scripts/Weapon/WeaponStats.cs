using UnityEngine;

// 플레이어의 '스탯'을 관리하는 컴포넌트
public class WeaponStats : MonoBehaviour
{
    [SerializeField] private ResourceStat _bullet;
    [SerializeField] private ResourceStat _bulletClipCount;
    [SerializeField] private ResourceStat _bombCount;

    public ResourceStat BulletCount => _bullet;
    public ResourceStat BulletClipCount => _bulletClipCount;
    public ResourceStat BombCount => _bombCount;

    private void Start()
    {
        _bullet.Initialize();
        _bulletClipCount.Initialize();
        _bombCount.Initialize();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
    }
}
