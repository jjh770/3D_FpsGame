#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class PatrolPathCreator : EditorWindow
{
    private Transform[] _selectedTransforms;
    private string _pathName = "NewPatrolPath";
    private string _description = "패트롤 경로 설명";

    [MenuItem("Tools/Monster System/Patrol Path Creator")]
    public static void ShowWindow()
    {
        GetWindow<PatrolPathCreator>("Patrol Path Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Patrol Path Creator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "1. Hierarchy에서 패트롤 포인트로 사용할 Transform들을 선택하세요.\n" +
            "2. 'Load Selected Transforms' 버튼을 클릭하세요.\n" +
            "3. 이름과 설명을 입력하세요.\n" +
            "4. 'Create Patrol Path SO' 버튼을 클릭하세요.",
            MessageType.Info);

        EditorGUILayout.Space();

        // 선택된 Transform 로드 버튼
        if (GUILayout.Button("Load Selected Transforms", GUILayout.Height(30)))
        {
            LoadSelectedTransforms();
        }

        EditorGUILayout.Space();

        // 선택된 Transform 표시
        if (_selectedTransforms != null && _selectedTransforms.Length > 0)
        {
            EditorGUILayout.LabelField($"선택된 Transform: {_selectedTransforms.Length}개");

            EditorGUI.indentLevel++;
            for (int i = 0; i < _selectedTransforms.Length; i++)
            {
                if (_selectedTransforms[i] != null)
                {
                    EditorGUILayout.LabelField($"{i}: {_selectedTransforms[i].name} - {_selectedTransforms[i].position}");
                }
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            // 이름 입력
            _pathName = EditorGUILayout.TextField("Path Name", _pathName);

            // 설명 입력
            EditorGUILayout.LabelField("Description");
            _description = EditorGUILayout.TextArea(_description, GUILayout.Height(60));

            EditorGUILayout.Space();

            // SO 생성 버튼
            if (GUILayout.Button("Create Patrol Path SO", GUILayout.Height(40)))
            {
                CreatePatrolPathSO();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Transform이 선택되지 않았습니다.", MessageType.Warning);
        }
    }

    private void LoadSelectedTransforms()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects == null || selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("오류", "Transform을 선택해주세요!", "확인");
            return;
        }

        _selectedTransforms = new Transform[selectedObjects.Length];
        for (int i = 0; i < selectedObjects.Length; i++)
        {
            _selectedTransforms[i] = selectedObjects[i].transform;
        }

        Debug.Log($"{_selectedTransforms.Length}개의 Transform을 로드했습니다.");
    }

    private void CreatePatrolPathSO()
    {
        if (_selectedTransforms == null || _selectedTransforms.Length == 0)
        {
            EditorUtility.DisplayDialog("오류", "Transform을 먼저 로드해주세요!", "확인");
            return;
        }

        if (string.IsNullOrWhiteSpace(_pathName))
        {
            EditorUtility.DisplayDialog("오류", "경로 이름을 입력해주세요!", "확인");
            return;
        }

        // PatrolPathSO 생성
        PatrolPathSO newPath = CreateInstance<PatrolPathSO>();

        // Transform 위치를 SO에 설정
        newPath.SetWaypointsFromTransforms(_selectedTransforms);

        // 파일로 저장
        string path = $"Assets/Resources/PatrolPaths/{_pathName}.asset";
        string directory = "Assets/Resources/PatrolPaths";

        // 디렉토리가 없으면 생성
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }
        if (!AssetDatabase.IsValidFolder(directory))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "PatrolPaths");
        }

        // 중복 파일명 처리
        path = AssetDatabase.GenerateUniqueAssetPath(path);

        AssetDatabase.CreateAsset(newPath, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 생성된 SO 선택
        EditorGUIUtility.PingObject(newPath);
        Selection.activeObject = newPath;

        EditorUtility.DisplayDialog("성공", $"PatrolPathSO가 생성되었습니다!\n경로: {path}", "확인");

        Debug.Log($"PatrolPathSO 생성 완료: {path}");
    }
}
#endif
