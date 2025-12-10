using UnityEngine;
using UnityEngine.UI;

public class UI_BulletStats : MonoBehaviour
{
    [SerializeField] private WeaponStats _weaponStats;
    [SerializeField] private Text _bulletCountText;
    [SerializeField] private Text _bulletClipCountText;

    private void Update()
    {
        _bulletCountText.text = _weaponStats.BulletCount.CurrentCount.ToString();
        _bulletClipCountText.text = _weaponStats.BulletClipCount.CurrentCount.ToString();
    }
}
