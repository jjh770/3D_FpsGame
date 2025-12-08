using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _staminaSlider;

    private PlayerMove _playerMove;
    private float _currentHealth;

    private void Start()
    {
        _playerMove = FindObjectOfType<PlayerMove>();

        if (_staminaSlider != null)
        {
            _staminaSlider.maxValue = _playerMove.MaxStamina;
            _staminaSlider.value = _playerMove.CurrentStamina;
        }
    }

    private void Update()
    {
        UpdateStaminaUI();
    }

    private void UpdateStaminaUI()
    {
        if (_staminaSlider != null && _playerMove != null)
        {
            _staminaSlider.value = _playerMove.CurrentStamina;
        }
    }
}
