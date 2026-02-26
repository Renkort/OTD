using UnityEngine;

public abstract class HoldableItem : MonoBehaviour
{
    public abstract void UpdateUI();
}

public class EmptyHoldableItem : HoldableItem
{
    public override void UpdateUI() { }
}
