using UnityEngine;

[CreateAssetMenu(fileName = "Item Parameter Data", menuName = "Inventory/Item Data/Parameters/Item Parameter")]
public class ItemParameterData : ScriptableObject
{
    [field: SerializeField]
    public string ParameterName { get; private set; }
}
