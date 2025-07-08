using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerDownHandler // Zmieniamy z IPointerClickHandler na IPointerDownHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amountText;
    [HideInInspector] public int slotIndex;
    private Item item;
    private int amount;

    private void Awake()
    {
        // CanvasGroup niepotrzebny bez dragowania
    }

    public void SetSlot(int index, ItemStack stack)
    {
        slotIndex = index;
        if (stack != null && stack.item != null && stack.amount > 0)
        {
            item = stack.item;
            amount = stack.amount;
            icon.sprite = item.icon;
            icon.enabled = true;
            amountText.text = (amount > 1) ? amount.ToString() : "";
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        item = null;
        amount = 0;
        icon.sprite = null;
        icon.enabled = false;
        amountText.text = "";
    }

    public bool IsEmpty()
    {
        return item == null || amount <= 0;
    }

    // KLIKANIE

    public void OnPointerDown(PointerEventData eventData) // Zmieniamy metodê na OnPointerDown
    {
        var ui = GetComponentInParent<InventoryUI>();
        var inventory = ui.inventory;

        // LEWY PRZYCISK
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (InventoryUI.heldStack == null && !IsEmpty())
            {
                InventoryUI.heldStack = new ItemStack(item, amount);
                InventoryUI.heldFromIndex = slotIndex;
                inventory.slots[slotIndex] = null;
                ui.UpdateInventoryUI(inventory.slots);
                ui.ShowDragGhost(InventoryUI.heldStack.item.icon, InventoryUI.heldStack.amount);
            }
            else if (InventoryUI.heldStack != null && IsEmpty())
            {
                inventory.slots[slotIndex] = InventoryUI.heldStack;
                InventoryUI.heldStack = null;
                InventoryUI.heldFromIndex = -1;
                ui.UpdateInventoryUI(inventory.slots);
                ui.HideDragGhost();
            }
            else if (InventoryUI.heldStack != null && !IsEmpty() && item == InventoryUI.heldStack.item && amount < item.maxStackSize)
            {
                int space = item.maxStackSize - amount;
                int toAdd = Mathf.Min(space, InventoryUI.heldStack.amount);
                amount += toAdd;
                InventoryUI.heldStack.amount -= toAdd;
                inventory.slots[slotIndex] = new ItemStack(item, amount);

                if (InventoryUI.heldStack.amount <= 0)
                {
                    InventoryUI.heldStack = null;
                    InventoryUI.heldFromIndex = -1;
                    ui.HideDragGhost();
                }
                else
                {
                    ui.ShowDragGhost(InventoryUI.heldStack.item.icon, InventoryUI.heldStack.amount);
                }
                ui.UpdateInventoryUI(inventory.slots);
            }
            else if (InventoryUI.heldStack != null && !IsEmpty())
            {
                var temp = new ItemStack(item, amount);
                inventory.slots[slotIndex] = InventoryUI.heldStack;
                InventoryUI.heldStack = temp;
                InventoryUI.heldFromIndex = slotIndex;
                ui.UpdateInventoryUI(inventory.slots);
                ui.ShowDragGhost(InventoryUI.heldStack.item.icon, InventoryUI.heldStack.amount);
            }
        }

        // PRAWY PRZYCISK
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (InventoryUI.heldStack == null && !IsEmpty())
            {
                int half = (amount + 1) / 2;
                int rest = amount - half;
                InventoryUI.heldStack = new ItemStack(item, half);
                InventoryUI.heldFromIndex = slotIndex;
                amount = rest;
                if (amount <= 0)
                    inventory.slots[slotIndex] = null;
                else
                    inventory.slots[slotIndex] = new ItemStack(item, amount);
                ui.UpdateInventoryUI(inventory.slots);
                ui.ShowDragGhost(InventoryUI.heldStack.item.icon, InventoryUI.heldStack.amount);
            }
            else if (InventoryUI.heldStack != null && IsEmpty())
            {
                inventory.slots[slotIndex] = new ItemStack(InventoryUI.heldStack.item, 1);
                InventoryUI.heldStack.amount -= 1;
                if (InventoryUI.heldStack.amount <= 0)
                {
                    InventoryUI.heldStack = null;
                    InventoryUI.heldFromIndex = -1;
                    ui.HideDragGhost();
                }
                else
                {
                    ui.ShowDragGhost(InventoryUI.heldStack.item.icon, InventoryUI.heldStack.amount);
                }
                ui.UpdateInventoryUI(inventory.slots);
            }
            else if (InventoryUI.heldStack != null && !IsEmpty() && item == InventoryUI.heldStack.item && amount < item.maxStackSize)
            {
                amount += 1;
                InventoryUI.heldStack.amount -= 1;
                inventory.slots[slotIndex] = new ItemStack(item, amount);
                if (InventoryUI.heldStack.amount <= 0)
                {
                    InventoryUI.heldStack = null;
                    InventoryUI.heldFromIndex = -1;
                    ui.HideDragGhost();
                }
                else
                {
                    ui.ShowDragGhost(InventoryUI.heldStack.item.icon, InventoryUI.heldStack.amount);
                }
                ui.UpdateInventoryUI(inventory.slots);
            }
            else if (InventoryUI.heldStack != null && !IsEmpty())
            {
                var temp = new ItemStack(item, amount);
                inventory.slots[slotIndex] = InventoryUI.heldStack;
                InventoryUI.heldStack = temp;
                InventoryUI.heldFromIndex = slotIndex;
                ui.UpdateInventoryUI(inventory.slots);
                ui.ShowDragGhost(InventoryUI.heldStack.item.icon, InventoryUI.heldStack.amount);
            }
        }
    }

    public void AddItem(Item newItem, int quantity = 1)
    {
        if (item == null)
        {
            item = newItem;
            amount = quantity;
            icon.sprite = item.icon;
            icon.enabled = true;
            amountText.text = (amount > 1) ? amount.ToString() : "";
        }
        else if (item == newItem && item.isStackable)
        {
            amount += quantity;
            if (amount > item.maxStackSize)
                amount = item.maxStackSize;
            amountText.text = (amount > 1) ? amount.ToString() : "";
        }
        else
        {
            Debug.LogWarning("Próbujesz dodaæ inny item do zajêtego slotu!");
        }
    }
}
