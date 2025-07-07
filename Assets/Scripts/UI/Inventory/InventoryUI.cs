using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform slotsParent;
    [SerializeField] private InventorySlot slotPrefab;
    [SerializeField] private Inventory inventory;
    private List<InventorySlot> slots = new List<InventorySlot>();

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

}
