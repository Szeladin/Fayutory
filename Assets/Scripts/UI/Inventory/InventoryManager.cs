using UnityEngine;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] slots; // Przypisz sloty w Inspectorze

    public void Add(Item item)
    {
        foreach (var slot in slots)
        {
            if (slot.IsEmpty())
            {
                slot.AddItem(item);
                return;
            }
        }
        // Ewentualnie: wy�wietl info o braku miejsca
    }
}