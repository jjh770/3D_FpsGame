using TMPro;
using UnityEngine;

public class UI_Coin : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _coinCountText;

    private void Start()
    {
        CoinManager.Instance.OnDataChanged += Refresh;
        Refresh(); // 초기값 표시
    }

    private void Refresh()
    {
        _coinCountText.text = $"Coin : {CoinManager.Instance.CoinCount * 10}G";
    }

    private void OnDestroy()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnDataChanged -= Refresh;
        }
    }
}
