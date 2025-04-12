using System.Collections.Generic;
using R3;
using UnityEngine;

public class PickableObject : MonoBehaviour, IPickable
{
    [SerializeField] private Collider _collider;
    
    [Header("Pick and Drop Visuals")]
    [SerializeField] private Color _previewValidColor;
    [SerializeField] private Color _previewInvalidColor;
    [SerializeField] private MeshRenderer _visual;
    
    [Header("Collision Detection")]
    [field: SerializeField] public LayerMask InvalidLayers { get; set; }
    [field: SerializeField] public LayerMask PlaceableLayers { get; set; }
    
    private bool _isValid = true;
    
    private List<Collider> _collidingObjects = new();

    private MaterialPropertyBlock _materialPropertyBlock;
    
    public ReactiveCommand OnPickUp = new();
    public ReactiveCommand OnPlace = new();
    
    private void OnValidate()
    {
        _collider ??= GetComponent<Collider>();
    }

    private void Start()
    {
        _materialPropertyBlock = new MaterialPropertyBlock();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & InvalidLayers) != 0)
        {
            _collidingObjects.Add(other);
            _isValid = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & InvalidLayers) != 0)
        {
            _collidingObjects.Remove(other);
            _isValid = _collidingObjects.Count <= 0;
        }
    }

    public void PickUpObject()
    {
        _collider.isTrigger = true;

        OnPickUp.Execute(new Unit());
    }

    public virtual void UpdatePlaceVisuals(bool canPlace)
    {
        _materialPropertyBlock.SetColor("_BaseColor", canPlace ? _previewValidColor : _previewInvalidColor);
        _visual.SetPropertyBlock(_materialPropertyBlock);
    }

    public void UpdatePosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    public void UpdateRotation(Quaternion newRotation)
    {
        transform.rotation = newRotation;
    }
    
    public virtual void PlaceObject()
    {
        _collider.isTrigger = false;
        ResetVisual();
        
        OnPlace.Execute(new Unit());
    }

    public virtual bool CanPlace()
    {
        return _isValid;
    }

    protected virtual void ResetVisual()
    {
        _materialPropertyBlock.SetColor("_BaseColor", Color.cyan);
        _visual.SetPropertyBlock(_materialPropertyBlock);
    }
}