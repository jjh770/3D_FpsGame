using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Monster))]
public class MonsterHealthBar : MonoBehaviour
{
    private Monster _monster;
    private MonsterStats _stats;
    private MonsterCombat _combat;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Image _delayImage;
    [SerializeField] private Transform _healthBarTransform;

    [SerializeField] private float _fillDuration = 0.2f;
    [SerializeField] private float _delayDuration = 0.5f;
    private Tween _fillTween;
    private Tween _delayTween;

    private Camera _mainCamera;

    private void Awake()
    {
        _monster = GetComponent<Monster>();
        _stats = GetComponent<MonsterStats>();
        _combat = GetComponent<MonsterCombat>();
        _mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        if (_combat != null)
        {
            _combat.OnHit += OnHitMonster;
        }

        if (_monster != null)
        {
            _monster.OnMonsterSpawned += RefreshHealthBar;
        }
    }

    private void OnDisable()
    {
        if (_combat != null)
        {
            _combat.OnHit -= OnHitMonster;
        }

        if (_monster != null)
        {
            _monster.OnMonsterSpawned -= RefreshHealthBar;
        }
    }
    private void OnHitMonster()
    {
        float targetValue = _stats.GetHealthPercentage();

        _fillTween?.Kill();
        _fillTween = _fillImage.DOFillAmount(targetValue, _fillDuration);

        if (_delayImage != null)
        {
            _delayTween?.Kill();
            _delayTween = _delayImage.DOFillAmount(targetValue, _delayDuration);
        }
    }

    /// <summary>
    /// 스폰 시 체력바를 즉시 풀로 리셋합니다.
    /// </summary>
    private void RefreshHealthBar()
    {
        float healthPercentage = _stats.GetHealthPercentage();

        // 트윈 중단
        _fillTween?.Kill();
        _delayTween?.Kill();

        // 즉시 풀로 설정 (애니메이션 없이)
        if (_fillImage != null)
        {
            _fillImage.fillAmount = healthPercentage;
        }

        if (_delayImage != null)
        {
            _delayImage.fillAmount = healthPercentage;
        }
    }

    // LateUpdate() 유니티 내부 계산이 끝난 뒤 '화면 갱신'을 위한 코드만 모여있을 때 주로 사용
    private void LateUpdate()
    {
        // UI는 변경사항이 있을 경우에 계속 다시 처음부터 그리기 때문에 예외처리
        // UI 가 알고있는 몬스터 체력값과 다를 때만 fillAmount를 수정한다.
        // 빌보드 기법 : 카메라의 위치와 회전에 상관없이 항상 정면을 바라보게 하는 기법
        _healthBarTransform.forward = _mainCamera.transform.forward;
    }

    private void OnDestroy()
    {
        // OnDisable에서 이미 구독 해제됨
        _fillTween?.Kill();
        _delayTween?.Kill();
    }
}
