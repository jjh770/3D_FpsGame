using UnityEngine;

[CreateAssetMenu(fileName = "CharacterMoveConfigSO", menuName = "Character Common/CharacterMoveConfig")]
public class CharacterMoveConfigSO : ScriptableObject
{
    [SerializeField] private float _runStamina;
    [SerializeField] private float _jumpStamina;

    public float RunStamina => _runStamina;
    public float JumpStamina => _jumpStamina;
}
