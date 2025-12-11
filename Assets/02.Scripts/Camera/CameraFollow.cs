using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private Transform _fpsTransform;
    [SerializeField] private Transform _tpsTransform;
    private bool _isTps;
    private bool _isChanging;

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _isTps = !_isTps;
        }
        CameraView();
    }

    private void CameraView()
    {
        if (_isChanging) return;

        if (_isTps)
        {
            transform.position = _tpsTransform.position;
            transform.LookAt(_cameraTarget);
        }
        else
        {
            transform.position = _fpsTransform.position;
        }
    }
}
