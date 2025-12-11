using System.Collections;
using UnityEngine;

public class CoroutineExample_3 : MonoBehaviour
{
    private void Update()
    {
        // 1. 코루틴도 함수이므로 인자를 넘겨줄 수 있다.
        StartCoroutine(Sequence_Coroutine());
    }

    private IEnumerator Sequence_Coroutine()
    {
        // 여러 코루틴을 연속해서 실행할 경우 중첩 코루틴을 사용하지 말고 시퀀스 형식으로 해결
        yield return StartCoroutine(Ready_Coroutine(1f));
        yield return StartCoroutine(Start_Coroutine(1f));
        yield return StartCoroutine(End_Coroutine(1f));
    }

    private IEnumerator Ready_Coroutine(float second)
    {
        Debug.Log($"{second}초 대기");
        yield return new WaitForSeconds(second);
        // 2. 코루틴 내부에서도 코루틴을 호출할 수 있다.
        StartCoroutine(Start_Coroutine(1f));
    }

    private IEnumerator Start_Coroutine(float second)
    {
        Debug.Log($"{second}초 대기");
        yield return new WaitForSeconds(second);
    }
    private IEnumerator End_Coroutine(float second)
    {
        Debug.Log($"{second}초 대기");
        yield return new WaitForSeconds(second);
    }
}
