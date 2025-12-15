using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerHitScreen : MonoBehaviour
{
    [SerializeField] private Image _hitScreen;
    [SerializeField] private Player _player;
    private Sequence _hitSequence;

    private void Start()
    {
        _hitScreen.gameObject.SetActive(false);
        _player.OnHitPlayer += PlayerHit;
    }

    private void PlayerHit()
    {
        _hitScreen.gameObject.SetActive(true);
        _hitScreen.color = new Color(1f, 1f, 1f, 1f);

        _hitSequence?.Kill();

        _hitSequence = DOTween.Sequence();
        _hitSequence.Append(_hitScreen.transform.DOScale(Vector3.one, 0.25f))
                    .Append(_hitScreen.transform.DOScale(Vector3.one * 1.3f, 0.5f))
                    .Join(_hitScreen.DOFade(0, 0.5f))
                    .OnComplete(() => _hitScreen.gameObject.SetActive(false));
    }
}
