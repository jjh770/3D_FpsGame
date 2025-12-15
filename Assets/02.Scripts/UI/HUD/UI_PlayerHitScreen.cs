using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerHitScreen : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float _scaleDownDuration = 0.25f;
    [SerializeField] private float _scaleUpDuration = 0.5f;
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private float _maxScale = 1.3f;

    [Header("Components")]
    [SerializeField] private Image _hitScreen;
    [SerializeField] private Player _player;

    private Sequence _hitSequence;
    private Transform _hitScreenTransform;

    private void Awake()
    {
        _hitScreenTransform = _hitScreen.transform;
    }
    private void Start()
    {
        _hitScreen.gameObject.SetActive(false);
        _player.OnHitPlayer += PlayerHit;
    }
    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.OnHitPlayer -= PlayerHit;
        }
        _hitSequence?.Kill();
    }
    private void PlayerHit()
    {
        _hitScreen.gameObject.SetActive(true);
        _hitScreen.color = new Color(1f, 1f, 1f, 1f);

        _hitSequence?.Kill();

        _hitSequence = DOTween.Sequence();
        _hitSequence.Append(_hitScreenTransform.DOScale(Vector3.one, _scaleDownDuration))
                    .Append(_hitScreenTransform.DOScale(Vector3.one * _maxScale, _scaleUpDuration))
                    .Join(_hitScreen.DOFade(0, _fadeDuration))
                    .OnComplete(() => _hitScreen.gameObject.SetActive(false));
    }
}
