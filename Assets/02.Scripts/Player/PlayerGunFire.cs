using UnityEngine;

public class PlayerGunFire : MonoBehaviour
{
    // 마우스의 왼쪽 버튼을 누르면 바라보는 방향으로 총을 발사하고 싶다. (총을 발사하고 싶다)
    [SerializeField] private Transform _fireTransform;
    [SerializeField] private ParticleSystem _hitEffectVFX;

    private void Update()
    {
        // 1. 마우스 왼쪽 버튼이 눌린다면
        if (Input.GetMouseButton(0))
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
                // ParticleSystem hitEffect = Instantiate(_hitEffectPrefab, hitInfo.point, Quaternion.identity);

                // 2. 하나를 캐싱해두고 Play    -> 단점 : 재실행이므로 기존 Play 중인 파티클 삭제 (Hierachy에 두기)
                // Play방식은 이미 같은 파티클을 Play하면 이전에 Play중인 파티클은 사라짐
                // hitInfo에는 point이라는 정보를 제공함 -> point는 부딫힌 위치 반환
                // _hitEffectVFX.transform.position = hitInfo.point;
                // hitInfo에는 normal이라는 정보를 제공함 -> normal은 법선벡터(부딫히면 튕겨져 나오는 방향)
                // _hitEffectVFX.transform.forward = hitInfo.normal;
                // _hitEffectVFX.Play();

                // 3. 하나를 캐싱해두고 Emit
                ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
                emitParams.position = hitInfo.point;
                emitParams.rotation3D = Quaternion.LookRotation(hitInfo.normal).eulerAngles;
                _hitEffectVFX.Emit(emitParams, 1);
            }
        }
    }
}
