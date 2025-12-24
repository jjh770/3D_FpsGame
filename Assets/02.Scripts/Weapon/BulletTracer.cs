// BulletTracer.cs (총알처럼 날아가는 버전)
using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private float _travelTimer = 0f;
    private float _bulletSpeed = 300f; // 총알 속도
    private float _bulletLength = 2f; // 총알 궤적 길이

    private Vector3 _startPoint;
    private Vector3 _endPoint;
    private Vector3 _direction;
    private float _totalDistance;

    private Color _startColor;
    private Color _endColor;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = true;

        _startColor = _lineRenderer.startColor;
        _endColor = _lineRenderer.endColor;
    }

    public void Initialize(Vector3 start, Vector3 end, float speed, float length, float width)
    {
        _startPoint = start;
        _endPoint = end;
        _direction = (end - start).normalized;
        _totalDistance = Vector3.Distance(start, end);

        _bulletSpeed = speed;
        _bulletLength = length;
        _travelTimer = 0f;

        _lineRenderer.startWidth = width;
        _lineRenderer.endWidth = width;

        _lineRenderer.startColor = _startColor;
        _lineRenderer.endColor = _endColor;

        _lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        _travelTimer += Time.deltaTime;

        // 현재 총알 위치 계산
        float travelDistance = _bulletSpeed * _travelTimer;

        // 총알 시작점과 끝점 계산
        Vector3 bulletFront = _startPoint + _direction * travelDistance;
        Vector3 bulletBack = _startPoint + _direction * Mathf.Max(0, travelDistance - _bulletLength);

        // LineRenderer 업데이트
        _lineRenderer.SetPosition(0, bulletBack);
        _lineRenderer.SetPosition(1, bulletFront);

        // 목표 지점 도달하면 풀로 반환
        if (travelDistance >= _totalDistance)
        {
            ObjectPool.Instance.Despawn(gameObject);
        }
    }
}
