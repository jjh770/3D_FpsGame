using UnityEngine;

public interface IKnockbackable
{
    void TakeKnockback(Vector3 direction, float knockbackAmount);
}
