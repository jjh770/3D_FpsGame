using UnityEngine;

/// <summary>
/// 런타임에 Player 참조를 공유하기 위한 ScriptableObject
/// Player가 Awake에서 자신을 등록하고, Monster들이 이를 통해 Player에 접근합니다.
/// </summary>
[CreateAssetMenu(fileName = "PlayerReference", menuName = "References/Player Reference")]
public class PlayerReferenceSO : ScriptableObject
{
    /// <summary>
    /// 현재 활성화된 Player 인스턴스
    /// 런타임에만 설정되며, 에디터 모드에서는 null입니다.
    /// </summary>
    public Player Current { get; set; }

    /// <summary>
    /// 에디터나 씬 전환 시 참조를 초기화합니다.
    /// </summary>
    private void OnDisable()
    {
        Current = null;
    }
}
