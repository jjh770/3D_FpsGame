using UnityEngine;

[CreateAssetMenu(fileName = "GravityConfigSO", menuName = "Character Common/GravityConfig")]
public class GravityConfigSO : ScriptableObject
{
    [SerializeField] private float _gravity = -9.81f;

    public float Gravity => _gravity;
}
