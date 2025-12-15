using DG.Tweening;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _offsetY = 10f;
    private float _minOffsetY = 5f;
    private float _maxOffsetY = 20f;
    private Tween _adjustTween;

    private void LateUpdate()
    {
        Vector3 targetPosition = _target.position;
        Vector3 finalPosition = targetPosition + new Vector3(0f, _offsetY, 0f);
        transform.position = finalPosition;


        Vector3 targetAngle = _target.eulerAngles;
        targetAngle.x = 90;
        transform.eulerAngles = targetAngle;
    }

    public void SetMapSize(float amount)
    {
        float newOffset = _offsetY + amount;

        if (newOffset > _maxOffsetY || newOffset < _minOffsetY)
        {
            return;
        }

        _adjustTween?.Kill();

        _adjustTween = DOTween.To(() => _offsetY, x => _offsetY = x, newOffset, 1f);
    }
}
