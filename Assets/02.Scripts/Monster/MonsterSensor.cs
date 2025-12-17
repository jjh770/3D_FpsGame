using System;
using UnityEngine;

public class MonsterSensor : MonoBehaviour
{
    [SerializeField] private PlayerReferenceSO _playerReference;
    [SerializeField] private LayerMask _playerLayer;

    private Player _player;
    private Vector3 _spawnPosition;

    public Player Player => _player;
    public Vector3 SpawnPosition => _spawnPosition;
    public event Action OnPlayerDeath;

    private void Awake()
    {
        _spawnPosition = transform.position;
    }

    private void Start()
    {
        // PlayerReferenceSO에서 Player 가져오기
        if (_playerReference != null)
        {
            _player = _playerReference.Current;
            if (_player != null)
            {
                _player.OnPlayerDeath += HandlePlayerDeath;
            }
        }
    }

    // 범위 내 플레이어 감지
    public bool IsPlayerInRange(float range)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, _playerLayer);
        return colliders.Length > 0;
    }

    // 스폰 위치 근처인지 체크
    public bool IsNearSpawn(float threshold = 0.5f)
    {
        return Vector3.Distance(transform.position, _spawnPosition) <= threshold;
    }

    private void HandlePlayerDeath()
    {
        OnPlayerDeath?.Invoke();
    }

    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.OnPlayerDeath -= HandlePlayerDeath;
        }
    }

    // Gizmo로 감지 범위 시각화
    private void OnDrawGizmosSelected()
    {
        // 감지 범위 (노란색)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 10f);

        // 공격 범위 (빨간색)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}
