using Reflex.Attributes;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerPickAndPlaceSystem : MonoBehaviour
{
    [Header("Pick and Place Parameters")]
    [SerializeField] private float _pickRange = 3f;
    [SerializeField] private float _placeRange = 4f;
    [SerializeField] private LayerMask _pickableObjectsLayerMask;
    
    [Header("Raycast Parameters")]
    [SerializeField] private float _raycastDistance;
    [SerializeField] private float _raycastStartVerticalOffset;
    [SerializeField] private float _objectDistanceFromPlayer;
    
    private PickableObject _itemInHands = null;
    private Transform _itemInHandsTransform = null;

    private IStackable _stackableObject = null;
    private IStackable _stackableObjectInHands = null;
    
    private Vector3 _currentPlacementPosition = Vector3.zero;
    
    private bool _canChangeBuildMode = false;
    private bool _inBuildingMode = false;
    private bool _isValidPreviewState = true;
    
    private RaycastHit _hitInfo;
    
    [Inject] private PlayerInputSystem _playerInputSystem;
    [Inject] private CinemachineCamera _camera;

    private void Update()
    {
        UpdateBuildModeAvailability();
        UpdateInput();
        UpdateCurrentPlacementPosition();
    }

    private void UpdateInput()
    {
        if (_playerInputSystem.BuildingButtonPressed) //Switch build mode
        {
            if (_canChangeBuildMode)
            {
                _canChangeBuildMode = false;
                _inBuildingMode = !_inBuildingMode;
                SwitchBuildingMode();
            }
            _playerInputSystem.ExecuteBuildAction();
        }

        if (_playerInputSystem.ScrollVector.y > 0) //Rotate object
        {
            if (_itemInHands != null)
            {
                _itemInHandsTransform.Rotate(_itemInHandsTransform.rotation.x, _itemInHandsTransform.rotation.y + 45f, _itemInHandsTransform.rotation.z);
            }
        }

        if (_playerInputSystem.ScrollVector.y < 0) //Rotate object
        {
            if (_itemInHands != null)
            {
                _itemInHandsTransform.Rotate(_itemInHandsTransform.rotation.x, _itemInHandsTransform.rotation.y - 45f, _itemInHandsTransform.rotation.z);
            }
        }
    }

    #region Build Mode
    
    private void UpdateBuildModeAvailability()
    {
        if (_inBuildingMode)
            return;

        _canChangeBuildMode = Physics.Raycast(_camera.transform.position, _camera.transform.forward, out _hitInfo, _pickRange, _pickableObjectsLayerMask);
        
        //Pick up stack parent
        if (_hitInfo.collider != null)
        {
            var stackable = _hitInfo.collider.GetComponent<IStackable>();
            if (stackable is { HaveStack: true })
                _canChangeBuildMode = false;
        }
    }
    
    private void SwitchBuildingMode()
    {
        if (_inBuildingMode)
            EnterBuildingMode();
        else
            ExitBuildingMode();
    }
    
    private void EnterBuildingMode()
    {
        //Pick up object
        if (_hitInfo.collider != null && _itemInHands == null)
        {
            _itemInHands = _hitInfo.collider.GetComponent<PickableObject>();
            if (_itemInHands != null)
            {
                _itemInHands.PickUpObject();
                _itemInHandsTransform = _hitInfo.collider.transform;

                _stackableObjectInHands = _itemInHands.GetComponent<IStackable>();
                _stackableObjectInHands?.ReleaseStackParent();
            }
        }
    }

    private void ExitBuildingMode()
    {
        PlaceObject();
    }
    
    #endregion
    
    #region Object Placement
    
    private void UpdateCurrentPlacementPosition()
    {
        if (!_inBuildingMode)
            return;
        
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out _hitInfo, _placeRange, _itemInHands.PlaceableLayers, QueryTriggerInteraction.Ignore))
        {
            //Try get Stackable object
            _stackableObject = _hitInfo.collider.GetComponent<IStackable>();
            if (_stackableObject is { CanHaveStack: true, HaveStack: false })
            {
                _stackableObjectInHands = _itemInHands.GetComponent<IStackable>();
                if (_stackableObjectInHands is { CanBeStacked: true })
                {
                    _currentPlacementPosition = _stackableObject.GetStackPosition();
                    
                    _itemInHands.UpdatePlaceVisuals(CanPlaceObject());
                    _itemInHands.UpdatePosition(_currentPlacementPosition);
                    _itemInHands.UpdateRotation(_hitInfo.collider.transform.rotation);
                }
            }
            else
            {
                //Placement on non stackable objects
                if (_hitInfo.collider.GetComponent<IPickable>() != null)
                {
                    _currentPlacementPosition = (_camera.transform.position + _camera.transform.forward * _placeRange);
                    
                    _itemInHands.UpdatePlaceVisuals(false);
                    _itemInHands.UpdatePosition(_currentPlacementPosition);
                    _itemInHands.UpdateRotation(_itemInHandsTransform.rotation);
            
                    _canChangeBuildMode = false;
                    
                    return;
                }
                
                //Placement on surface according to normal
                _currentPlacementPosition = _hitInfo.point;
                
                _itemInHands.UpdatePlaceVisuals(CanPlaceObject());
                _itemInHands.UpdatePosition(_currentPlacementPosition);
                _itemInHands.UpdateRotation(Quaternion.FromToRotation(_itemInHandsTransform.up, _hitInfo.normal) * _itemInHandsTransform.rotation);
            }
            
            _canChangeBuildMode = _itemInHands.CanPlace();
        }
        else
        {
            //Placement in air
            _currentPlacementPosition = (_camera.transform.position + _camera.transform.forward * _placeRange);
            
            _itemInHands.UpdatePlaceVisuals(false);
            _itemInHands.UpdatePosition(_currentPlacementPosition);
            _itemInHands.UpdateRotation(_itemInHandsTransform.rotation);
            
            _canChangeBuildMode = false;
        }
    }

    private bool CanPlaceObject()
    {
        if (_itemInHands == null)
            return false;

        return _itemInHands.CanPlace();
    }
    
    private void PlaceObject()
    {
        if (!_itemInHands.CanPlace())
            return;

        _stackableObject?.SwitchStackItemState(true);
        _stackableObjectInHands?.SetStackParent(_stackableObject);
        
        _itemInHands.PlaceObject();
        _itemInHands = null;
        _itemInHandsTransform = null;

        _stackableObject = null;
        _stackableObjectInHands = null;
    }
    
    #endregion
}