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
            Debug.Log($"Slot {index}: ustawiam {stack.item.itemName} x{stack.amount}");
            item = stack.item;
            amount = stack.amount;
            icon.sprite = item.icon;
            icon.enabled = true;
            amountText.text = (amount > 1) ? amount.ToString() : "";
        }
        else
        {
            Debug.Log($"Slot {index}: czyszczê slot");
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
        originalParent = transform.parent;
        originalPosition = transform.position;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(transform.root); // przenieœ na wierzch, by nie by³o pod slotami
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsEmpty()) return;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent);
        transform.position = originalPosition;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot draggedSlot = eventData.pointerDrag?.GetComponent<InventorySlot>();
        if (draggedSlot == null || draggedSlot == this) return;

        InventoryUI inventoryUI = GetComponentInParent<InventoryUI>();
        if (inventoryUI != null)
        {
            inventoryUI.SwapSlots(draggedSlot.slotIndex, this.slotIndex);
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
