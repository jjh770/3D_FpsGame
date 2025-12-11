using UnityEngine;

// 유니티에서 제공하는 어트리뷰트 (강제로 해당 타입의 컴포넌트를 붙여버림, 지울수도 없음)
[RequireComponent(typeof(Rigidbody2D))]
public class ErrorTest_1 : MonoBehaviour
{
    private void Start()
    {
        // MissingComponentException
        // 사용하고자 하는 컴포넌트가 null일 때 발생
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();

        // 초기화 시 null 검사하는 방어코드
        // 방어코드 -> null 검사
        if (rigidbody2D == null)
        {
            // 적절한 처리
            // - AddComponent
            // - 오류를 로깅 (나타냄)
            // - GetComponent 대신 TryGetComponent 사용
        }

        Debug.Log(rigidbody2D.linearVelocity);

        // NullReferenceException
        // 사용하고자 하는 객체가 null 값일 때 그 객체의 필드나 메서드에 접근하려고 하면 발생
        Rigidbody2D rigidbody2D2 = null;
        Debug.Log(rigidbody2D2.linearVelocity);
    }
}
