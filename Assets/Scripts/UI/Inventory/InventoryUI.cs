using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static ItemStack draggedStack = null;
    public static int draggedFromIndex = -1;
    [SerializeField] private Transform slotsParent;
    [SerializeField] private InventorySlot slotPrefab;
    [SerializeField] private Inventory inventory;
    private List<InventorySlot> slots = new List<InventorySlot>();

    [SerializeField] private GameObject dragGhostPrefab;
    private GameObject dragGhostInstance;
    private Image ghostIcon;
    private TMP_Text ghostAmount;

    private PlayerInput controls;
    private bool isVisible = false;

    private void Awake()
    {
        controls = new PlayerInput();
        controls.Enable();
        controls.UI.ToggleInventory.performed += OnToggleInventory;
    }

    private void OnDestroy()
    {
        controls.UI.ToggleInventory.performed -= OnToggleInventory;
        controls.Dispose();
    }
    
    private void Start()
    {
        gameObject.SetActive(false);
        GenerateSlots();
        UpdateInventoryUI(inventory.slots);
        
        // Inicjalizacja drag ghosta
        dragGhostInstance = Instantiate(dragGhostPrefab, transform.parent); // najlepiej na tym samym Canvasie
        dragGhostInstance.SetActive(false);
        ghostIcon = dragGhostInstance.transform.Find("Icon").GetComponent<Image>();
        ghostAmount = dragGhostInstance.transform.Find("Amount").GetComponent<TMP_Text>();
    }

    private void OnToggleInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isVisible = !isVisible;
            gameObject.SetActive(isVisible);
        }
    }

    private void GenerateSlots()
    {
        // Tworzy sloty tylko raz na starcie
        for (int i = 0; i < inventory.size; i++)
        {
            InventorySlot newSlot = Instantiate(slotPrefab, slotsParent);
            newSlot.SetSlot(i, null); // na starcie puste
            slots.Add(newSlot);
        }
    }

    public void UpdateInventoryUI(ItemStack[] inventorySlots)
    {
        Debug.Log("Aktualizujê UI ekwipunku...");
            for (int i = 0; i < slots.Count; i++)
        {
            Debug.Log($"Slot UI {i}: {(inventorySlots[i] != null ? inventorySlots[i].item.itemName + " x" + inventorySlots[i].amount : "pusty")}");
            slots[i].SetSlot(i, inventorySlots[i]);
        }
    }

    public void SwapSlots(int fromIndex, int toIndex)
    {
        if (fromIndex == toIndex) return;

        // Zamiana miejsc w tablicy slotów ekwipunku
        var temp = inventory.slots[fromIndex];
        inventory.slots[fromIndex] = inventory.slots[toIndex];
        inventory.slots[toIndex] = temp;

        UpdateInventoryUI(inventory.slots);
    }

    public void ShowDragGhost(Sprite icon, int amount)
    {
        dragGhostInstance.SetActive(true);
        ghostIcon.sprite = icon;
        ghostIcon.enabled = true;
        ghostAmount.text = (amount > 1) ? amount.ToString() : "";
    }

    public void MoveDragGhost(Vector2 position)
    {
        dragGhostInstance.transform.position = position;
    }

    public void HideDragGhost()
    {
        dragGhostInstance.SetActive(false);
    }

}
