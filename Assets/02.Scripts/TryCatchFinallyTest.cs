using System;
using UnityEngine;

public class TryCatchFinallyTest : MonoBehaviour
{
    public int Age;

    // 예외 : 런타임에 발생하는 오류 (참조, 나누기, 인덱스 범위 벗어나기 등등)

    // try-catch 문법은 예외를 처리하는 기본 문법

    private void Start()
    {
        if (Age < 0)
        {
            Debug.Log("0보다 작을 수 없음");
            throw new Exception("0보다 작을 수 없음");
        }

        // 아래 문법은 인덱스 범위를 벗어나므로 오류가 발생함.
        // -> 다른 컴포넌트나 게임 오브젝트에도 영향을 줌으로써 프로그램이 정상적으로 동작 안할 수 있다.
        int[] numbers = new int[32];
        //int index = 75; // 실제로는 내가 문제를 해결하기 위해 반복문이나 수식을 통해 얻은 인덱스
        //numbers[index] = 1;

        // 베스트 : 알고리즘을 잘 처리하는 것

        try
        {
            // 예외가 발생할만한 코드 작성
            int index = 75; // 실제로는 내가 문제를 해결하기 위해 반복문이나 수식을 통해 얻은 인덱스
            numbers[index] = 1;
        }
        catch (Exception e)
        {
            Debug.Log(e); // 무슨 오류인지 확인

            // 예외가 발생했을 때 처리해야할 코드 작성
            int index = numbers.Length - 1;
            numbers[index] = 1;
            Debug.Log("IndexOutOfRange 오류 일어남");
        }
        finally
        {
            // 옵션 : 정상, 오류 구분하지 않고 실행할 코드 작성
        }
    }
}

//Try - Catch 구문은 되도록이면 안쓰는 것이 좋음
//- 성능 저하
//- 잘못된 알고리즘 방치

//하지만 써야하는 경우가 있음 
//- 네트워크 접근
//    - 로그인, 로그아웃, 서버, DB, 아이템 저장/불러오기 등등
//- 파일 접근
//    - 모든 예외 상황을 처리할 수 없기 때문 (파일명 확인, 용량 확인, 권한 확인 등등)
//- DB 접근
