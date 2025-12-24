using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private TextMeshProUGUI _progressText;

    private void Start()
    {
        StartCoroutine(LoadScene_Coroutine());
    }

    private IEnumerator LoadScene_Coroutine()
    {
        // LoadSceneAsync는 씬 로드 상황에 대한 데이터를 가지고 있는 객체를 반환한다.
        AsyncOperation ao = SceneManager.LoadSceneAsync("DecorateFPSScene");

        // 로드되는 씬의 모습이 화면에 안보이게 하기
        ao.allowSceneActivation = false;

        // 로드가 완료될 때까지 계속 진행
        while (!ao.isDone)
        {
            // ao는 진행률도 가지고 있음.
            _progressSlider.value = ao.progress;
            _progressText.text = $"{ao.progress * 100}%";

            // 90%에서 더이상 안차오르는 이유는 모든 씬 로드를 완료한 뒤 넘어가기전 준비상태이기 때문
            if (ao.progress >= 0.9f)
            {
                ao.allowSceneActivation = true;
            }
            yield return null;
        }

    }
}
