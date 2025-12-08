using UnityEngine;

public class ErrorTest_2 : MonoBehaviour
{
    private void Start()
    {
        // MissingReferenceException : 보통 삭제한 게임 오브젝트를 참조/접근(필드, 메서드) 하려고 할 때 뜸
        Destroy(gameObject);
        Debug.Log(gameObject.name);

        // IndexOutOfRangeException : 배열 (리스트)에서 유효하지 않은 인덱스에 접근할 때 뜸
        int[] numbers = new int[10];
        Debug.Log(numbers[13]);

        // DevideByZeroException : 정수 타입의 변수를 0으로 나눌 때 뜸
        int number1 = 13;
        int number2 = 0;
        Debug.Log(number1 / number2);
    }
}
