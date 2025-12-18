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

    [Header("Fire Effects")]
    [SerializeField] private GameObject[] _muzzleFlashPrefabs; // 5개 할당

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
        PrewarmMuzzleFlash(); // 추가
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

    private void PrewarmMuzzleFlash()
    {
        if (_muzzleFlashPrefabs.Length == 0) return;

        foreach (var prefab in _muzzleFlashPrefabs)
        {
            ObjectPool.Instance.Prewarm(prefab, 3);
        }
    }

    // ScriptableObject의 데이터를 기반으로 ResourceStat 생성
    private void InitializeWeapon()
    {
        _bulletCount = new ResourceStat(_weaponData.MaxBulletCount);
        _bulletClipCount = new ResourceStat(_weaponData.MaxBulletClipCount);
    }

    public bool TryShoot()
    {
        if (_timer < _weaponData.CoolTime) return false;
        if (_isReloading) return false;
        if (_bulletCount.IsEmpty()) return false;

        _bulletCount.TryConsume();
        BulletUIChange();
        Fire();
        FireEffect();
        TriggerRebound();
        _timer = 0f;
        return true;
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
        WeaponEventChannelSO.Instance?.RaiseReload(_weaponData.ReloadTime);
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

    private void FireEffect()
    {
        // 랜덤으로 하나 선택
        int randomIndex = Random.Range(0, _muzzleFlashPrefabs.Length);
        GameObject selectedFlash = _muzzleFlashPrefabs[randomIndex];

        GameObject flash = ObjectPool.Instance.Spawn(
            selectedFlash,
            _fireTransform.position,
            _fireTransform.rotation
        );
        // 부모 설정 추가
        flash.transform.SetParent(_fireTransform);
        flash.transform.localPosition = Vector3.zero; // 로컬 위치 0으로
        flash.transform.localRotation = Quaternion.identity;

        ObjectPool.Instance.Despawn(flash, 0.05f);
    }

    private void Fire()
    {
        // 화면 중앙에서 Ray 생성 (FPS/TPS 모두 지원)
        Ray ray = _mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

        // RayCastHit(충돌한 대상의 정보)를 저장할 변수
        RaycastHit hitInfo;

        // 발사하고 충돌 여부 확인
        bool isHit = Physics.Raycast(ray, out hitInfo);
        if (isHit)
        {
            // 충돌했다면 피격 이펙트 표시
            _hitEffectVFX.transform.position = hitInfo.point;
            _hitEffectVFX.transform.forward = hitInfo.normal;
            _hitEffectVFX.Play();

            // hitInfo의 태그와 레이어 비교하지 않은 이유
            // -> 어차피 Monster 컴포넌트를 가지고 와야하기 때문 (2번 일 안함)
            if (hitInfo.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TryTakeDamage(_weaponData.Damage);
                if (damageable is IKnockbackable knockbackable)
                {
                    knockbackable.TakeKnockback(ray.direction, _weaponData.KnockbackAmount);
                }
            }
        }
    }
    private void TriggerRebound()
    {
        Vector3 rebound = _weaponData.CalculateRebound();

        // 이벤트로 반동 전달 (CameraRotate에서 받음)
        WeaponEventChannelSO.Instance?.RaiseReboundCamera(rebound);
    }

    private void BulletUIChange()
    {
        WeaponEventChannelSO.Instance?.RaiseAmmoChanged(_bulletCount.CurrentCount, _bulletClipCount.CurrentCount);
    }
}
