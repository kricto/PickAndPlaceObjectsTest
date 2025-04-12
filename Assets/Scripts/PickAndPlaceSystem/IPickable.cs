using UnityEngine;

public interface IPickable
{
    LayerMask InvalidLayers { get; set; }
    LayerMask PlaceableLayers { get; set; }
    
    void PickUpObject();
    void UpdatePlaceVisuals(bool canPlace);
    void PlaceObject();
    bool CanPlace();
}