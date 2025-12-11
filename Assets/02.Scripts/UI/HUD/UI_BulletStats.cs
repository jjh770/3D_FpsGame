using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_BulletStats : MonoBehaviour
{
    [SerializeField] private Text _bulletCountText;
    [SerializeField] private Text _bulletClipCountText;
    [SerializeField] private Image _bulletIcon;
    [SerializeField] private Image _reloadIcon;

    private Coroutine _reloadCoroutine;

    private void Awake()
    {
        if (_reloadIcon != null)
        {
            _reloadIcon.gameObject.SetActive(false);
            _reloadIcon.fillAmount = 0f;
        }
    }
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
        WeaponEvents.OnChangeWeapon -= OnChangeIcon;
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
        if (_reloadCoroutine != null)
        {
            StopCoroutine(_reloadCoroutine);
        }

        _reloadCoroutine = StartCoroutine(ReloadCoroutine(reloadTime));
    }
    private IEnumerator ReloadCoroutine(float reloadTime)
    {
        if (_reloadIcon == null) yield break;

        _reloadIcon.gameObject.SetActive(true);
        _reloadIcon.fillAmount = 0f;

        float elapsedTime = 0f;

        while (elapsedTime < reloadTime)
        {
            elapsedTime += Time.deltaTime;
            _reloadIcon.fillAmount = Mathf.Clamp01(elapsedTime / reloadTime);
            yield return null;
        }

        _reloadIcon.fillAmount = 1f;
        yield return new WaitForSeconds(0.1f);

        _reloadIcon.gameObject.SetActive(false);

        _reloadCoroutine = null;
    }
}
