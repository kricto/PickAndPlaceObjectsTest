using UnityEngine;

public interface IStackable
{
    bool CanHaveStack { get; set; }
    bool CanBeStacked { get; set; }
    bool HaveStack { get; }
    bool IsStacked { get; }
    
    Vector3 GetStackPosition();
    void SwitchStackItemState(bool haveStack);
    void SetStackParent(IStackable stackParent);
    void ReleaseStackParent();
}