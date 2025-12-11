using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    private Dictionary<string, List<GameObject>> _pools = new Dictionary<string, List<GameObject>>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string key = prefab.name;

        if (!_pools.ContainsKey(key))
        {
            _pools[key] = new List<GameObject>();
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

        GameObject newObj = Instantiate(prefab, position, rotation);
        newObj.name = prefab.name;
        _pools[key].Add(newObj);
        return newObj;
    }

    public void Despawn(GameObject obj, float delay = 0f)
    {
        if (delay > 0f)
            StartCoroutine(DespawnAfterDelay(obj, delay));
        else
            obj.SetActive(false);
    }

    private System.Collections.IEnumerator DespawnAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }
}
