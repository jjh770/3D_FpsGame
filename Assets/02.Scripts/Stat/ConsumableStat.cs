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
        _value = value;
        if (_value > _maxValue)
        {
            _value = _maxValue;
        }
    }
    public void RegenValue(float time)
    {
        _value += _regenValue * time;
        if (_value > _maxValue)
        {
            _value = _maxValue;
        }
    }
    public bool TryConsume(float amount)
    {
        if (_value < amount) return false;

        Consume(amount);
        return true;
    }
    public void Consume(float amount)
    {
        _value -= amount;
    }
}
