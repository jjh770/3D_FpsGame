using UnityEngine;
using UnityEngine.UI;

public class UI_BulletStats : MonoBehaviour
{
    [SerializeField] private Text _bulletCountText;
    [SerializeField] private Text _bulletClipCountText;
    [SerializeField] private Image _bulletIcon;

    private void OnEnable()
    {
        WeaponEvents.OnAmmoChanged += OnAmmoChanged;
        WeaponEvents.OnReload += OnReload;
        WeaponEvents.OnChangeWeapon += OnChangeIcon;
    }

    private void OnDisable()
    {
        WeaponEvents.OnAmmoChanged -= OnAmmoChanged;
        WeaponEvents.OnReload -= OnReload;
    }

    private void OnAmmoChanged(int currentBullet, int reserveBullet)
    {
        _bulletCountText.text = currentBullet.ToString();
        _bulletClipCountText.text = reserveBullet.ToString();
    }

    private void OnChangeIcon(Sprite bulletIcon)
    {
        _bulletIcon.sprite = bulletIcon;
    }

    private void OnReload(float reloadTime)
    {

    }
}
