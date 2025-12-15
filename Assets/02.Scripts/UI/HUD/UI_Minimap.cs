using UnityEngine;
using UnityEngine.UI;

public class UI_Minimap : MonoBehaviour
{
    [SerializeField] private Button _mapButtonReduce;
    [SerializeField] private Button _mapButtonExpand;
    [SerializeField] private MinimapCamera _minimapCamera;
    [SerializeField] private float _cameraAdjustAmount = 3f;

    private void Start()
    {
        _mapButtonExpand.onClick.AddListener(() => HandleMapSize(_cameraAdjustAmount));
        _mapButtonReduce.onClick.AddListener(() => HandleMapSize(-_cameraAdjustAmount));
    }

    private void HandleMapSize(float amount)
    {
        if (_minimapCamera == null)
        {
            return;
        }
        _minimapCamera.SetMapSize(amount);
    }
}
