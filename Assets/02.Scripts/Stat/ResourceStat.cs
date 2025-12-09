using System;
using UnityEngine;

[Serializable]
public class ResourceStat
{
    [SerializeField] private int _maxCount;
    [SerializeField] private int _currentCount;

    public int MaxCount => _maxCount;
    public int CurrentCount => _currentCount;

    public void Initialize()
    {
        _currentCount = _maxCount;
    }

    public bool TryConsume(int amount = 1)
    {
        if (_currentCount < amount)
        {
            return false;
        }

        _currentCount -= amount;
        return true;
    }

    public void Add(int amount = 1)
    {
        _currentCount += amount;
        if (_currentCount > _maxCount)
            _currentCount = _maxCount;
    }

    public bool IsFull() => _currentCount >= _maxCount;
    public bool IsEmpty() => _currentCount <= 0;
}
