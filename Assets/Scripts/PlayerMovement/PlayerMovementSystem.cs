using Reflex.Attributes;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerMovementSystem : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    
    [Space]
    [SerializeField] private float _moveSpeed = 5f;

    [Inject] private PlayerInputSystem _playerInputSystem;
    [Inject] private CinemachineCamera _camera;
    
    private void OnValidate()
    {
        _characterController ??= GetComponent<CharacterController>();
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        Vector3 move = new Vector3(_playerInputSystem.InputVector.x, 0, _playerInputSystem.InputVector.y);
        move = _camera.transform.TransformDirection(move);
        
        move.y = 0f;
        move *= _moveSpeed * Time.deltaTime;

        _characterController.Move(move);
    }
}
