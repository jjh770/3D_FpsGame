using UnityEngine;

/// <summary>
/// 몬스터의 VFX(시각 효과)를 관리하는 컴포넌트
/// Attack, Hit, Death 등 모든 VFX를 ObjectPool을 통해 재생합니다.
/// </summary>
public class MonsterVFXController : MonoBehaviour
{
    private GameObject _attackVFXPrefab;
    private GameObject _hitVFXPrefab;
    private Transform _attackTransform;

    /// <summary>
    /// MonsterDataSO로 VFX 초기화
    /// </summary>
    public void Initialize(MonsterDataSO data, Transform attackTransform)
    {
        if (data != null)
        {
            _attackVFXPrefab = data.AttackVFXPrefab;
            _hitVFXPrefab = data.HitVFXPrefab;
        }

        _attackTransform = attackTransform;
    }

    /// <summary>
    /// 공격 VFX를 재생합니다.
    /// </summary>
    public void MonsterAttackVFX(Vector3 direction)
    {
        if (_attackVFXPrefab == null)
        {
            Debug.LogWarning($"[MonsterVFXController] Attack VFX prefab is not set for {gameObject.name}");
            return;
        }

        Vector3 spawnPosition = _attackTransform != null ? _attackTransform.position : transform.position;
        Quaternion spawnRotation = Quaternion.LookRotation(direction);

        // ObjectPool에서 VFX 가져오기
        GameObject vfx = ObjectPool.Instance.Spawn(_attackVFXPrefab, spawnPosition, spawnRotation);

        // ParticleSystem 재생
        ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();

            // VFX 재생 후 풀로 반환 (duration만큼 후)
            float duration = ps.main.duration + ps.main.startLifetime.constantMax;
            ObjectPool.Instance.Despawn(vfx, duration);
        }
        else
        {
            // ParticleSystem이 없으면 1초 후 반환
            ObjectPool.Instance.Despawn(vfx, 1f);
        }
    }

    public void MonsterHitVFX(Vector3 hitPoint)
    {
        if (_hitVFXPrefab == null) return;

        // ObjectPool에서 VFX 가져오기
        GameObject vfx = ObjectPool.Instance.Spawn(_hitVFXPrefab, hitPoint, Quaternion.identity);
        vfx.transform.SetParent(transform);

        // ParticleSystem 재생
        ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();

            // VFX 재생 후 풀로 반환 (duration만큼 후)
            float duration = ps.main.duration + ps.main.startLifetime.constantMax;
            ObjectPool.Instance.Despawn(vfx, duration);
        }
        else
        {
            // ParticleSystem이 없으면 1초 후 반환
            ObjectPool.Instance.Despawn(vfx, 3f);
        }
    }
    // public void PlayDeathVFX() { }
}
