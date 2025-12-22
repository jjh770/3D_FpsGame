using System.Collections.Generic;
using UnityEngine;

public class CoinGain : MonoBehaviour
{
    [Header("Coin Settings")]
    [Tooltip("이 컴포넌트가 붙은 오브젝트에 SphereCollider(isTrigger=true)를 추가하고 radius를 이 값으로 설정하세요")]
    [SerializeField] private float _gainRadius = 2f;
    [SerializeField] private LayerMask _coinLayer = 1 << 6;  // Coin Layer 설정 필요
    [SerializeField] private Transform _playerTransform;

    [Header("Bezier Curve")]
    [SerializeField] private float _bezierHeight = 1.5f;     // 곡선 높이
    [SerializeField] private float _duration = 0.8f;         // 이동 소요 시간 (초)
    [SerializeField] private float _fadeSpeed = 2f;          // 투명도 사라짐 속도
    [SerializeField] private Transform _targetTransform;

    private readonly List<Coin> collectedCoins = new();
    private readonly Dictionary<Coin, CoinCollectionData> _coinDataMap = new();
    private SphereCollider _gainCollider;

    /// <summary>
    /// 코인 수집 시작 시점의 데이터
    /// </summary>
    private struct CoinCollectionData
    {
        public Vector3 StartPosition;
        public float StartTime;

        public CoinCollectionData(Vector3 startPosition, float startTime)
        {
            StartPosition = startPosition;
            StartTime = startTime;
        }
    }

    private void Awake()
    {
        // SphereCollider 자동 설정 (없으면 추가)
        _gainCollider = GetComponent<SphereCollider>();
        if (_gainCollider == null)
        {
            _gainCollider = gameObject.AddComponent<SphereCollider>();
        }

        _gainCollider.isTrigger = true;
        _gainCollider.radius = _gainRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 레이어 체크
        if (((1 << other.gameObject.layer) & _coinLayer) == 0)
            return;

        // Coin 컴포넌트 체크
        if (other.TryGetComponent<Coin>(out Coin coin) && !collectedCoins.Contains(coin))
        {
            CollectCoin(coin);
        }
    }

    private void Update()
    {
        // 수집된 동전들 이동
        UpdateCollectedCoins();
    }

    private void CollectCoin(Coin coin)
    {
        collectedCoins.Add(coin);

        // 수집 시작 데이터 저장
        _coinDataMap[coin] = new CoinCollectionData(
            coin.transform.position,  // 현재 위치를 시작점으로 고정
            Time.time                 // 수집 시작 시간
        );

        coin.gameObject.SetActive(true);  // Pool에서 활성화
    }

    private void UpdateCollectedCoins()
    {
        for (int i = collectedCoins.Count - 1; i >= 0; i--)
        {
            Coin coin = collectedCoins[i];
            coin.GetComponent<CapsuleCollider>().isTrigger = true;
            // 동전 despawn 처리
            if (!coin.gameObject.activeInHierarchy)
            {
                collectedCoins.RemoveAt(i);
                _coinDataMap.Remove(coin);  // Dictionary에서도 제거
                continue;
            }

            // Dictionary에 데이터가 없으면 스킵 (비정상 상태)
            if (!_coinDataMap.TryGetValue(coin, out CoinCollectionData data))
            {
                collectedCoins.RemoveAt(i);
                continue;
            }

            Vector3 targetPos = _targetTransform.position;
            MoveCoinWithBezier(coin, data, targetPos);
        }
    }

    private void MoveCoinWithBezier(Coin coin, CoinCollectionData data, Vector3 targetPos)
    {
        Transform coinTransform = coin.transform;

        // 경과 시간 기반 t 계산 (0 → 1, 한 방향)
        float elapsed = Time.time - data.StartTime;
        float t = Mathf.Clamp01(elapsed / _duration);

        // 고정된 시작 위치 사용
        Vector3 startPos = data.StartPosition;
        Vector3 controlPoint = (startPos + targetPos) / 2 + Vector3.up * _bezierHeight;
        Vector3 currentPos = EvaluateQuadraticBezier(startPos, controlPoint, targetPos, t);

        coinTransform.position = currentPos;

        // 3D 코인: 크기 줄이기 + 회전 가속
        float scale = Mathf.Lerp(1f, 0.1f, t);
        coinTransform.localScale = Vector3.one * scale;

        // 추가 회전 (수집될 때 빙글빙글)
        coinTransform.Rotate(Vector3.up * 360f * Time.deltaTime);

        // 도착 체크 (t가 1에 도달하면 제거)
        if (t >= 1f)
        {
            coin.OnDespawn();
            coin.gameObject.SetActive(false);
            _coinDataMap.Remove(coin);  // Dictionary에서 제거
        }
    }

    private Vector3 EvaluateQuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }

    // OverlapSphere 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _gainRadius);
    }
}
