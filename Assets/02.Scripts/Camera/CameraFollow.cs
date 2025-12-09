using DG.Tweening;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private Vector3 _tpsVector;
    private Sequence _changeSequence;
    private bool _isTps;
    private bool _isChanging;

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _isTps = !_isTps;
            _isChanging = true;

            _changeSequence?.Kill();
            _changeSequence = DOTween.Sequence();

            _changeSequence.AppendInterval(1f)
                .OnUpdate(() =>
                {
                    Vector3 newTarget = _isTps ? GetTpsPosition() : _cameraTarget.position;
                    transform.DOMove(newTarget, 1f);
                })
                .AppendCallback(() => _isChanging = false)
                .SetAutoKill(true);
        }
        CameraView();
    }

    private void CameraView()
    {
        if (_isChanging) return;

        if (_isTps)
        {
            transform.position = GetTpsPosition();
            transform.LookAt(_cameraTarget);
        }
        else
        {
            transform.position = _cameraTarget.position;
        }
    }

    private Vector3 GetTpsPosition()
    {
        Vector3 backDirection = -_cameraTarget.forward;
        Vector3 heightOffset = Vector3.up * _tpsVector.y;

        return _cameraTarget.position + backDirection * Mathf.Abs(_tpsVector.z) + heightOffset;
    }

    private void OnDestroy()
    {
        _changeSequence?.Kill();
    }
}
