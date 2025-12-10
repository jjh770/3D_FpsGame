using System.Collections;
using UnityEngine;

// 무기의 동작을 정의하는 컴포넌트
public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponStats _weaponStats;
    [SerializeField] private Transform _fireTransform;
    [SerializeField] private ParticleSystem _hitEffectVFX;

    private float _timer = 0;
    private void Awake()
    {
        _weaponStats = GetComponent<WeaponStats>();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
    }
    public void TryShoot()
    {
        if (_timer < _weaponStats.CoolTime) return;

        if (_weaponStats.IsReloading) return;


        if (_weaponStats.BulletCount.IsEmpty())
        {
            TryReload();
            return;
        }

        _weaponStats.BulletCount.TryConsume();
        Fire();
        _timer = 0f;
    }

    public void TryReload()
    {
        if (_weaponStats.IsReloading || _weaponStats.BulletClipCount.IsEmpty() || _weaponStats.BulletCount.IsFull())
            return;

        StartCoroutine(ReloadBullet());
    }

    private IEnumerator ReloadBullet()
    {
        _weaponStats.IsReloading = true;

        yield return new WaitForSeconds(_weaponStats.ReloadTime);

        int currentBullet = _weaponStats.BulletCount.CurrentCount;
        int maxBullet = _weaponStats.BulletCount.MaxCount;
        int reserveBullet = _weaponStats.BulletClipCount.CurrentCount;

        int bulletNeeded = maxBullet - currentBullet;
        int bulletToReload = Mathf.Min(bulletNeeded, reserveBullet);

        _weaponStats.BulletCount.Add(bulletToReload);
        _weaponStats.BulletClipCount.TryConsume(bulletToReload);

        _weaponStats.IsReloading = false;
    }

    private void Fire()
    {
        // 2. Ray를 생성하고 [발사할 위치], [방향], [거리(생략가능)]를 설정한다 (쏜다.)
        Ray ray = new Ray(_fireTransform.position, Camera.main.transform.forward);

        // 3. RayCastHit(충돌한 대상의 정보)를 저장할 변수를 생성한다.
        RaycastHit hitInfo = new RaycastHit();

        // 4. [발사하고], [충돌했다면] -> 맞았는가 안맞았는가 Bool 제공
        bool isHit = Physics.Raycast(ray, out hitInfo);
        if (isHit)
        {
            // 5. 충돌했다면 피격 이펙트 표시
            Debug.Log(hitInfo.transform.name);

            // 파티클생성과 플레이 방식
            // 1. Instantiate 방식 (+ 풀링) -> 새로 생성(메모리, CPU 측면으로는 좋음)
            // -> 한 화면에 여러가지 수정 후 여러 개 그릴 경우

            // ParticleSystem hitEffect = Instantiate(_hitEffectPrefab, hitInfo.point, Quaternion.identity);


            // 2. 하나를 캐싱해두고 Play    -> 단점 : 재실행이므로 기존 Play 중인 파티클 삭제 (Hierachy에 두기)
            // 인스펙터 설정 그대로 그릴 경우 (내부적으로는 Emit과 동일함)

            // hitInfo에는 point이라는 정보를 제공함 -> point는 부딫힌 위치 반환
            _hitEffectVFX.transform.position = hitInfo.point;
            // hitInfo에는 normal이라는 정보를 제공함 -> normal은 법선벡터(부딫히면 튕겨져 나오는 방향)
            _hitEffectVFX.transform.forward = hitInfo.normal;
            _hitEffectVFX.Play();

            // 3. 하나를 캐싱해두고 Emit
            // 인스펙터 설정을 수정 후 그릴 경우 (transform을 제외한 다른 설정을 바꾸고 싶을 때 emitParams 세팅)
            // 파티클 시스템 local vs world
            // 빌보드란?
        }
    }
}
