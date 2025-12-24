using System;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    private static ClickManager _instance;
    public static ClickManager Instance => _instance;

    private int _leftClickCount = 0;
    private int _rightClickCount = 0;

    public int LeftClickCount => _leftClickCount;
    public int RightClickCount => _rightClickCount;

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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _leftClickCount += 1;
            OnDataChanged?.Invoke();
        }
        if (Input.GetMouseButtonDown(1))
        {
            _rightClickCount += 1;
            OnDataChanged?.Invoke();
        }
    }
}
