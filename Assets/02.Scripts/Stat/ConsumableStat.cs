using System;
using UnityEngine;

// 도메인화
// -> 응집도가 높아진다.
// -> 무결성도 해치면 안된다. (value가 maxValue보다 크면 안된다 등)
[Serializable]
public class ConsumableStat
{
    [SerializeField] private float _maxValue;
    [SerializeField] private float _value;
    [SerializeField] private float _regenValue;

    public float MaxValue => _maxValue;
    public float Value => _value;

    // 현재 체력과 최대 체력을 전달
    public event Action<float, float> OnValueChanged;

    public void Initialize()
    {
        SetValue(_maxValue);
    }

    public void IncreaseMax(float amount)
    {
        _maxValue += amount;
    }
    public void Increase(float amount)
    {
        SetValue(_value + amount);
    }
    public void DecreaseMax(float amount)
    {
        _maxValue -= amount;
    }
    public void Decrease(float amount)
    {
        SetValue(_value - amount);
    }
    public void SetMaxValue(float value)
    {
        _maxValue = value;
    }
    public void SetValue(float value)
    {
        _value = Mathf.Clamp(value, 0, _maxValue);
        OnValueChanged?.Invoke(_value, _maxValue);
    }
    public void RegenValue(float time)
    {
        if (_value >= _maxValue) return;

        _value = Mathf.Min(_value + _regenValue * time, _maxValue);
        OnValueChanged?.Invoke(_value, _maxValue);
    }

    public float GetPercentage()
    {
        return _maxValue > 0 ? _value / _maxValue : 0f;
    }

    public bool TryConsume(float amount)
    {
        if (_value < amount)
        {
            // 체력 부족: 남은 체력만큼만 소모하고 false 반환
            SetValue(0);
            return false;
        }

        // 체력 충분: 요청한 만큼 소모하고 true 반환
        SetValue(_value - amount);
        return true;
    }

    public void Consume(float amount)
    {
        SetValue(_value - amount);
    }
}
