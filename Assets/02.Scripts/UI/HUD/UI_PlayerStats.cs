using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerStats _stats;

    [Header("Health UI")]
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Image _healthDelayFill;

    [Header("Stamina UI")]
    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private Image _staminaDelayFill;

    [Header("Animation Settings")]
    [SerializeField] private float _mainSliderDuration = 0.2f;
    [SerializeField] private float _delayFillDuration = 0.5f;
    [SerializeField] private Ease _mainEase = Ease.OutQuad;
    [SerializeField] private Ease _delayEase = Ease.OutQuad;

    private Tweener _healthTween;
    private Tweener _healthDelayTween;
    private Tweener _staminaTween;
    private Tweener _staminaDelayTween;

    private void OnEnable()
    {
        if (_stats != null)
        {
            _stats.Health.OnValueChanged += UpdateHealthSlider;
            _stats.Stamina.OnValueChanged += UpdateStaminaSlider;

            float healthPercent = _stats.Health.GetPercentage();
            _healthSlider.value = healthPercent;
            _healthDelayFill.fillAmount = healthPercent;

            _staminaSlider.value = _stats.Stamina.GetPercentage();
        }
    }

    private void OnDisable()
    {
        if (_stats != null)
        {
            _stats.Health.OnValueChanged -= UpdateHealthSlider;
            _stats.Stamina.OnValueChanged -= UpdateStaminaSlider;
        }

        _healthTween?.Kill();
        _healthDelayTween?.Kill();
        _staminaTween?.Kill();
        _staminaDelayTween?.Kill();
    }

    private void UpdateSlider(float current, float max, Slider mainSlider, Image delayFill, ref Tweener mainTween, ref Tweener delayTween)
    {
        float targetValue = max > 0 ? current / max : 0;

        mainTween?.Kill();
        mainTween = mainSlider.DOValue(targetValue, _mainSliderDuration)
            .SetEase(_mainEase);

        if (delayFill != null)
        {
            delayTween?.Kill();
            delayTween = delayFill.DOFillAmount(targetValue, _delayFillDuration)
                .SetEase(_delayEase);
        }
    }

    private void UpdateHealthSlider(float current, float max)
    {
        UpdateSlider(current, max, _healthSlider, _healthDelayFill, ref _healthTween, ref _healthDelayTween);
    }

    private void UpdateStaminaSlider(float current, float max)
    {
        UpdateSlider(current, max, _staminaSlider, _staminaDelayFill, ref _staminaTween, ref _staminaDelayTween);
    }
}
