using UnityEngine;
using UnityEngine.UI;

public class UI_BombStats : MonoBehaviour
{
    [SerializeField] private Text _bombCountText;

    private void OnEnable()
    {
        WeaponEvents.OnConsumableChanged += OnConsumableChanged;
    }
    private void OnDisable()
    {
        WeaponEvents.OnConsumableChanged -= OnConsumableChanged;
    }

    private void OnConsumableChanged(int current)
    {
        _bombCountText.text = current.ToString();
    }
}
