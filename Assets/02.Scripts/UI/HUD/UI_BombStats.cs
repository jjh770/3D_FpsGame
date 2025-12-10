using UnityEngine;
using UnityEngine.UI;

public class UI_BombStats : MonoBehaviour
{
    [SerializeField] private WeaponStats _weaponStats;
    [SerializeField] private Text _bombCountText;

    private void Update()
    {
        _bombCountText.text = _weaponStats.BombCount.CurrentCount.ToString();
    }
}
