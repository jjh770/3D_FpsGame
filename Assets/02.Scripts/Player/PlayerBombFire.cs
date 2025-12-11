using UnityEngine;

public class PlayerBombFire : MonoBehaviour
{
    // 마우스 오른쪽 버튼을 누르면 카메라(플레이어)가 바라보는 방향으로 폭탄을 던지고 싶음

    // 필요 속성
    // 발사 위치
    // 발사할 폭탄
    // 던질 힘
    [SerializeField] private Transform _fireTransform;
    [SerializeField] private Bomb _bombPrefab;
    [SerializeField] private float _throwPower = 15f;
    private PlayerBombs _playerBombs;
    private Camera _mainCamera;

    private void Awake()
    {
        _playerBombs = GetComponent<PlayerBombs>();
        _mainCamera = Camera.main;
    }
    private void Start()
    {
        BombUIChange();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (_playerBombs.BombCount.IsEmpty()) return;

            _playerBombs.BombCount.TryConsume();
            BombUIChange();
            GameObject bomb = ObjectPool.Instance.Spawn(_bombPrefab.gameObject, _fireTransform.position, Quaternion.identity);
            Rigidbody bombRigidbody = bomb.GetComponent<Rigidbody>();
            bombRigidbody.AddForce(_mainCamera.transform.forward * _throwPower, ForceMode.Impulse);
        }
    }

    private void BombUIChange()
    {
        WeaponEvents.TriggerConsumableChanged(_playerBombs.BombCount.CurrentCount);
    }
}
