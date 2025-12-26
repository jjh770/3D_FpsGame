using System;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    private static CoinManager _instance;
    public static CoinManager Instance => _instance;

    private int _coinCount = 0;

    public int CoinCount => _coinCount;

    public event Action OnDataChanged;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    public void AddCoin(int amount = 1)
    {
        _coinCount += amount;
        OnDataChanged?.Invoke();
    }
}
