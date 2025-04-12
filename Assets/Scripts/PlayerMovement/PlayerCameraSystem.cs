using Reflex.Attributes;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCameraSystem : MonoBehaviour
{
    [SerializeField] private float _mouseSensitivity = 1f;

    private float _xRotation = 0f;
    
    [Inject] private PlayerInputSystem _playerInputSystem;
    [Inject] private CinemachineCamera _camera;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Turn();
    }

    private void Turn()
    {
        _xRotation -= _playerInputSystem.LookDelta.y * _mouseSensitivity * Time.deltaTime;
        
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        
        _camera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        
        transform.Rotate(Vector3.up * _playerInputSystem.LookDelta.x * _mouseSensitivity * Time.deltaTime);
    }
}
