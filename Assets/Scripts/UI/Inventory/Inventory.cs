using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int size = 27; // liczba slot�w
    public ItemStack[] slots; // Ka�dy slot pami�ta sw�j przedmiot i ilo��
    public InventoryUI inventoryUI;

    private void Awake()
    {
        slots = new ItemStack[size];
    }

    // Dodaje item do KONKRETNEGO slotu (np. po rozdzieleniu stacka)
    public bool AddToSlot(int slotIndex, Item itemToAdd, int quantity = 1)
    {
        if (slotIndex < 0 || slotIndex >= size) return false;

        var stack = slots[slotIndex];
        if (stack == null)
        {
            slots[slotIndex] = new ItemStack(itemToAdd, quantity);
            inventoryUI.UpdateInventoryUI(slots);
            return true;
        }
        else if (stack.item == itemToAdd && itemToAdd.isStackable && stack.amount < itemToAdd.maxStackSize)
        {
            int addable = Mathf.Min(quantity, itemToAdd.maxStackSize - stack.amount);
            stack.amount += addable;
            inventoryUI.UpdateInventoryUI(slots);
            return addable == quantity;
        }
        // Slot zaj�ty przez inny item lub pe�ny stack
        return false;
    }

    // Dodaje item do pierwszego wolnego slotu lub do stacka, je�li si� da
    public bool Add(Item itemToAdd, int quantity = 1)
    {
        Debug.Log($"Pr�ba dodania: {itemToAdd.itemName}, ilo��: {quantity}");

        // Najpierw pr�bujemy do�o�y� do istniej�cych stack�w
        if (itemToAdd.isStackable)
        {
            for (int i = 0; i < size; i++)
            {
                var stack = slots[i];
                if (stack != null && stack.item == itemToAdd && stack.amount < itemToAdd.maxStackSize)
                {
                    int addable = Mathf.Min(quantity, itemToAdd.maxStackSize - stack.amount);
                    stack.amount += addable;
                    quantity -= addable;
                    if (quantity <= 0)
                    {
                        inventoryUI.UpdateInventoryUI(slots);
                        return true;
                    }
                }
            }
        }
        // Potem do pustych slot�w
        for (int i = 0; i < size && quantity > 0; i++)
        {
            if (slots[i] == null)
            {
                int addAmount = itemToAdd.isStackable ? Mathf.Min(quantity, itemToAdd.maxStackSize) : 1;
                slots[i] = new ItemStack(itemToAdd, addAmount);
                quantity -= addAmount;
            }
        }

        Debug.Log("Dodano item do ekwipunku. Aktualna zawarto��:");
        for (int i = 0; i < slots.Length; i++)
        {
            var stack = slots[i];
            if (stack != null)
                Debug.Log($"Slot {i}: {stack.item.itemName}, ilo��: {stack.amount}");
            else
                Debug.Log($"Slot {i}: pusty");
        }

        inventoryUI.UpdateInventoryUI(slots);
        return quantity <= 0;
    }

    // Usuwa item z konkretnego slotu
    public void RemoveFromSlot(int slotIndex, int quantity = 1)
    {
        if (slotIndex < 0 || slotIndex >= size) return;
        var stack = slots[slotIndex];
        if (stack == null) return;

        stack.amount -= quantity;
        if (stack.amount <= 0)
            slots[slotIndex] = null;

        inventoryUI.UpdateInventoryUI(slots);
    }

    // Usuwa item z dowolnego slotu (pierwszy pasuj�cy)
    public void Remove(Item itemToRemove, int quantity = 1)
    {
        for (int i = 0; i < size && quantity > 0; i++)
        {
            var stack = slots[i];
            if (stack != null && stack.item == itemToRemove)
            {
                int removeAmount = Mathf.Min(quantity, stack.amount);
                stack.amount -= removeAmount;
                quantity -= removeAmount;
                if (stack.amount <= 0)
                    slots[i] = null;
            }
        }
        inventoryUI.UpdateInventoryUI(slots);
    }
}
