using UnityEngine;

public class PlayerBombs : MonoBehaviour
{
    [SerializeField] private ResourceStat _bombCount;

    public ResourceStat BombCount => _bombCount;
    private void Awake()
    {
        _bombCount.Initialize();
    }
}
