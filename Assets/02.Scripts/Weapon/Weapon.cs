using System.Collections;
using UnityEngine;

// 모든 총기류 무기를 처리하는 통합 클래스
// ScriptableObject 기반으로 코드 중복 제거 및 OCP 준수
public class Weapon : MonoBehaviour, IWeapon
{
    [Header("Weapon Configuration")]
    [SerializeField] private WeaponDataSO _weaponData;

    [Header("Fire Components")]
    [SerializeField] private Transform _fireTransform;
    [SerializeField] private ParticleSystem _hitEffectVFX;

    // 런타임 상태 (각 인스턴스마다 독립적)
    private ResourceStat _bulletCount;
    private ResourceStat _bulletClipCount;
    private float _timer = 0f;
    private bool _isReloading = false;
    private Coroutine _reloadCoroutine;
    public FireMode FireMode => _weaponData.FireMode;
    private Camera _mainCamera;

    private void Awake()
    {
        InitializeWeapon();
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        BulletUIChange();
    }

    private void OnEnable()
    {
        BulletUIChange();
    }

    private void OnDisable()
    {
        // 무기 비활성화 시 재장전 취소
        if (_reloadCoroutine != null)
        {
            StopCoroutine(_reloadCoroutine);
            _reloadCoroutine = null;
        }
        _isReloading = false;
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_bulletCount.IsEmpty() && !_isReloading)
        {
            TryReload();
        }
    }


    // ScriptableObject의 데이터를 기반으로 ResourceStat 생성
    private void InitializeWeapon()
    {
        _bulletCount = new ResourceStat(_weaponData.MaxBulletCount);
        _bulletClipCount = new ResourceStat(_weaponData.MaxBulletClipCount);
    }

    public void TryShoot()
    {
        if (_timer < _weaponData.CoolTime) return;
        if (_isReloading) return;
        if (_bulletCount.IsEmpty()) return;

        _bulletCount.TryConsume();
        BulletUIChange();
        Fire();
        TriggerRebound();
        _timer = 0f;
    }

    public void TryReload()
    {
        if (_isReloading || _bulletClipCount.IsEmpty() || _bulletCount.IsFull())
            return;

        _reloadCoroutine = StartCoroutine(ReloadBullet());
    }

    public Sprite GetIcon()
    {
        return _weaponData.SpriteIcon;
    }

    private IEnumerator ReloadBullet()
    {
        _isReloading = true;
        WeaponEvents.TriggerReload(_weaponData.ReloadTime);
        yield return new WaitForSeconds(_weaponData.ReloadTime);

        int currentBullet = _bulletCount.CurrentCount;
        int maxBullet = _bulletCount.MaxCount;
        int reserveBullet = _bulletClipCount.CurrentCount;

        int bulletNeeded = maxBullet - currentBullet;
        int bulletToReload = Mathf.Min(bulletNeeded, reserveBullet);

        _bulletCount.Add(bulletToReload);
        _bulletClipCount.TryConsume(bulletToReload);
        BulletUIChange();

        _isReloading = false;
        _reloadCoroutine = null;
    }


    private void Fire()
    {
        // Ray를 생성하고 [발사할 위치], [방향]을 설정
        Ray ray = new Ray(_fireTransform.position, _mainCamera.transform.forward);

        // RayCastHit(충돌한 대상의 정보)를 저장할 변수
        RaycastHit hitInfo;

        // 발사하고 충돌 여부 확인
        bool isHit = Physics.Raycast(ray, out hitInfo);
        if (isHit)
        {
            // 충돌했다면 피격 이펙트 표시
            Debug.Log(hitInfo.transform.name);

            // hitInfo.point: 부딪힌 위치
            _hitEffectVFX.transform.position = hitInfo.point;
            // hitInfo.normal: 법선벡터 (튕겨져 나오는 방향)
            _hitEffectVFX.transform.forward = hitInfo.normal;
            _hitEffectVFX.Play();
        }
    }
    private void TriggerRebound()
    {
        Vector3 rebound = _weaponData.CalculateRebound();

        // 이벤트로 반동 전달 (CameraRotate에서 받음)
        WeaponEvents.TriggerRebound(rebound);
    }

    private void BulletUIChange()
    {
        WeaponEvents.TriggerAmmoChanged(_bulletCount.CurrentCount, _bulletClipCount.CurrentCount);
    }
}
