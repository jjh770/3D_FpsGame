using UnityEngine;

public class PlayerRotate : PlayerComponent
{
    public float RotationSpeed = 200f; // 0 ~ 360;

    private float _accumulationX = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (!CanExecute()) return;

        float mouseX = Input.GetAxis("Mouse X");
        _accumulationX += mouseX * RotationSpeed * Time.deltaTime;

        transform.eulerAngles = new Vector3(0, _accumulationX);
    }
}
