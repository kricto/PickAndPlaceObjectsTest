using UnityEngine;

public class StackableObject : PickableObject, IStackable
{
    [SerializeField] private Transform _stackPoint;
    
    [field: SerializeField] public bool CanHaveStack { get; set; }
    [field: SerializeField] public bool CanBeStacked { get; set; }
    
    public bool HaveStack { get; private set; }
    public bool IsStacked { get; private set; }

    private GameObject _stackedObject;
    private IStackable _stackParentObject;
    
    public Vector3 GetStackPosition()
    {
        return _stackPoint.position;
    }

    public void SwitchStackItemState(bool haveStack)
    {
        HaveStack = haveStack;
    }

    public void SetStackParent(IStackable stackParent)
    {
        _stackParentObject = stackParent;
        IsStacked = true;
    }

    public void ReleaseStackParent()
    {
        _stackParentObject?.SwitchStackItemState(false);
        _stackParentObject = null;
        IsStacked = true;
    }
}