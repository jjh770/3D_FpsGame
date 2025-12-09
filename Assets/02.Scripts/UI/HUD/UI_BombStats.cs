using UnityEngine;
using UnityEngine.UI;

public class UI_BombStats : MonoBehaviour
{
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private Text _bombCountText;

    private void Update()
    {
        _bombCountText.text = _stats.BombCount.CurrentCount.ToString();
    }
}
