using UnityEngine;

public class Bomb : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // 충돌하면 나 자신을 삭제한다.
        Destroy(gameObject);
    }
}
