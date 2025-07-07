using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amountText;
    [HideInInspector] public int slotIndex; // publiczny, by InventoryUI móg³ ustawiaæ

    private Item item;
    private int amount;

    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector2 originalPosition;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
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

    // DRAG & DROP

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsEmpty()) return;
        InventoryUI.draggedStack = new ItemStack(item, amount);
        InventoryUI.draggedFromIndex = slotIndex;

        // Poka¿ ducha
        var ui = GetComponentInParent<InventoryUI>();
        if (ui != null)
        {
            ui.ShowDragGhost(item.icon, amount);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        var ui = GetComponentInParent<InventoryUI>();
        if (ui != null)
        {
            ui.MoveDragGhost(eventData.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var ui = GetComponentInParent<InventoryUI>();
        if (ui != null)
        {
            ui.HideDragGhost();
        }
        InventoryUI.draggedStack = null;
        InventoryUI.draggedFromIndex = -1;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Przeci¹gany stack i indeks Ÿród³owy
        var dragged = InventoryUI.draggedStack;
        int fromIdx = InventoryUI.draggedFromIndex;

        if (dragged == null || fromIdx == slotIndex) return;

        InventoryUI inventoryUI = GetComponentInParent<InventoryUI>();
        if (inventoryUI != null)
        {
            inventoryUI.SwapSlots(fromIdx, slotIndex);
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
