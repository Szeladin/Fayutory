using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite icon = null;
    public bool isStackable = false;
    public int maxStackSize = 1;

    public virtual void Use()
    {
        Debug.Log("Using " + itemName);
    }
}
