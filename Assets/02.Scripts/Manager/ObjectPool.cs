using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private static ObjectPool _instance;
    public static ObjectPool Instance => _instance;

    private Dictionary<string, List<GameObject>> _pools = new Dictionary<string, List<GameObject>>();
    private Dictionary<string, Transform> _poolParents = new Dictionary<string, Transform>(); // 풀별 부모 오브젝트

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject); // 선택사항: 씬 전환 시에도 유지
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string key = prefab.name;

        if (!_pools.ContainsKey(key))
        {
            _pools[key] = new List<GameObject>();
            CreatePoolParent(key); // 풀 부모 오브젝트 생성
        }

        foreach (GameObject obj in _pools[key])
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.SetActive(true);
                return obj;
            }
        }

        GameObject newObj = Instantiate(prefab, position, rotation, _poolParents[key]); // 부모 설정
        newObj.name = prefab.name;
        _pools[key].Add(newObj);
        return newObj;
    }

    public void Prewarm(GameObject prefab, int count)
    {
        string key = prefab.name;

        if (!_pools.ContainsKey(key))
        {
            _pools[key] = new List<GameObject>();
            CreatePoolParent(key); // 풀 부모 오브젝트 생성
        }

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab, _poolParents[key]); // 부모 설정
            obj.name = prefab.name;
            obj.SetActive(false);
            _pools[key].Add(obj);
        }
    }

    public void Despawn(GameObject obj, float delay = 0f)
    {
        if (obj == null) return;

        if (delay > 0f)
            StartCoroutine(DespawnAfterDelay(obj, delay));
        else
        {
            obj.SetActive(false);
            // 부모 오브젝트 아래로 다시 이동 (Hierarchy 정리)
            string key = obj.name;
            if (_poolParents.ContainsKey(key))
            {
                obj.transform.SetParent(_poolParents[key]);
            }
        }
    }

    private System.Collections.IEnumerator DespawnAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null)
        {
            obj.SetActive(false);
            // 부모 오브젝트 아래로 다시 이동
            string key = obj.name;
            if (_poolParents.ContainsKey(key))
            {
                obj.transform.SetParent(_poolParents[key]);
            }
        }
    }

    // 풀별 부모 오브젝트 생성
    private void CreatePoolParent(string poolName)
    {
        GameObject parent = new GameObject($"[Pool] {poolName}");
        parent.transform.SetParent(transform); // ObjectPool의 자식으로
        _poolParents[poolName] = parent.transform;
    }
}
