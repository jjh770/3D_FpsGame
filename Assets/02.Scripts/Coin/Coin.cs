using UnityEngine;

public class Coin : MonoBehaviour, IPoolable
{
    private Rigidbody _rigidbody;

    [Header("Spread Settings")]
    [SerializeField] private float _spreadForce;      // 흩뿌리는 힘
    [SerializeField] private float _upwardForce;      // 위로 뜨는 힘
    [SerializeField] private float _randomTorque;     // 회전력

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void OnSpawn()
    {
        ResetPosition();
        SpreadPosition();
    }

    public void OnDespawn()
    {
        // 물리력 초기화 (재사용 시 깔끔하게)
        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
        transform.localScale = Vector3.one;  // 스케일 초기화 추가
    }

    private void ResetPosition()
    {
        //transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    private void SpreadPosition()
    {
        // 1. 랜덤 방향 벡터 생성 (수평 원형)
        Vector2 randomCircle = Random.insideUnitCircle.normalized;
        Vector3 spreadDirection = new Vector3(randomCircle.x, 0, randomCircle.y);

        // 2. 위로 + 랜덤 방향 힘 적용
        Vector3 totalForce = spreadDirection * _spreadForce + Vector3.up * _upwardForce;
        _rigidbody.AddForce(totalForce, ForceMode.Impulse);

        // 3. 랜덤 회전 추가 (동전이 빙글빙글)
        Vector3 randomTorqueForce = new Vector3(
            Random.Range(-_randomTorque, _randomTorque),
            Random.Range(-_randomTorque, _randomTorque),
            Random.Range(-_randomTorque, _randomTorque)
        );
        _rigidbody.AddTorque(randomTorqueForce, ForceMode.Impulse);
    }
}
