using UnityEngine;

public class PlayerGunFire : MonoBehaviour
{
    // 마우스의 왼쪽 버튼을 누르면 바라보는 방향으로 총을 발사하고 싶다. (총을 발사하고 싶다)
    [SerializeField] private WeaponController _weaponController;

    private void Update()
    {
        if (_weaponController == null) return;

        // 1. 마우스 왼쪽 버튼이 눌린다면
        if (Input.GetMouseButton(0))
        {
            _weaponController.TryShoot();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            _weaponController.TryReload();
        }
    }
}
