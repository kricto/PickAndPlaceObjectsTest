using R3;
using UnityEngine;

public class ObjectWithLogic : MonoBehaviour
{
    [SerializeField] private PickableObject _pickableObject;

    private void OnValidate()
    {
        _pickableObject ??= GetComponent<PickableObject>();
    }

    private void Start()
    {
        _pickableObject.OnPickUp.Subscribe(_ => PickUpLogic()).AddTo(this);
        _pickableObject.OnPlace.Subscribe(_ => PlaceLogic()).AddTo(this);
    }

    private void PickUpLogic()
    {
        //Do something
    }

    private void PlaceLogic()
    {
        //Do something
    }
}
