using UnityEngine;

[CreateAssetMenu(fileName = "PatrolPath", menuName = "Monster System/Patrol Path", order = 0)]
public class PatrolPathSO : ScriptableObject
{
    [Header("Patrol Path Information")]
    [SerializeField] private string _pathName = "New Patrol Path";
    [TextArea(2, 4)]
    [SerializeField] private string _description = "패트롤 경로 설명";

    [Header("Waypoints")]
    [SerializeField] private Vector3[] _waypoints = new Vector3[3];

    public string PathName => _pathName;
    public string Description => _description;
    public Vector3[] Waypoints => _waypoints;
    public int WaypointCount => _waypoints?.Length ?? 0;

    public Vector3 GetWaypoint(int index)
    {
        if (_waypoints == null || index < 0 || index >= _waypoints.Length)
        {
            Debug.LogError($"[{name}] 유효하지 않은 웨이포인트 인덱스: {index}");
            return Vector3.zero;
        }
        return _waypoints[index];
    }

    // 에디터에서 씬의 Transform 위치를 가져오는 헬퍼 메서드
#if UNITY_EDITOR
    public void SetWaypointsFromTransforms(Transform[] transforms)
    {
        if (transforms == null || transforms.Length == 0)
        {
            Debug.LogWarning($"[{name}] Transform 배열이 비어있습니다.");
            return;
        }

        _waypoints = new Vector3[transforms.Length];
        for (int i = 0; i < transforms.Length; i++)
        {
            if (transforms[i] != null)
            {
                _waypoints[i] = transforms[i].position;
            }
        }

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[{name}] {transforms.Length}개의 웨이포인트가 설정되었습니다.");
    }
#endif

    private void OnValidate()
    {
        // 배열이 null이면 기본값으로 초기화
        if (_waypoints == null || _waypoints.Length == 0)
        {
            _waypoints = new Vector3[3];
        }
    }
}
