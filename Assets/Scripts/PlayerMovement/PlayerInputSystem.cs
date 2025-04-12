using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSystem : MonoBehaviour
{
    [SerializeField] private InputActionReference _moveReference;
    [SerializeField] private InputActionReference _lookReference;
    [SerializeField] private InputActionReference _buildReference;
    [SerializeField] private InputActionReference _scrollReference;
    
    [SerializeField] private InputActionReference _escapeReference;
    
    public Vector2 InputVector { get; private set; }
    public Vector2 LookDelta { get; private set; }
    public Vector2 ScrollVector { get; private set; }
    public bool BuildingButtonPressed { get; private set; }

    private void Update()
    {
        InputVector = _moveReference.action.ReadValue<Vector2>();
        LookDelta = _lookReference.action.ReadValue<Vector2>();
        ScrollVector = _scrollReference.action.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        _buildReference.action.started += OnBuildAction;
        _escapeReference.action.started += OnEscapeAction;
    }

    private void OnDisable()
    {
        _buildReference.action.started -= OnBuildAction;
        _escapeReference.action.started -= OnEscapeAction;
    }

    private void OnBuildAction(InputAction.CallbackContext context)
    {
        BuildingButtonPressed = true;
    }

    private void OnEscapeAction(InputAction.CallbackContext context)
    {
        Application.Quit();
    }

    public void ExecuteBuildAction()
    {
        BuildingButtonPressed = false;
    }
}
