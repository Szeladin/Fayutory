using UnityEngine;
using UnityEngine.InputSystem;

public class TestAddItem : MonoBehaviour
{
    public Inventory inventory;
    public Item item1;
    public Item item2;

    private PlayerInput controls;

    private void Awake()
    {
        controls = new PlayerInput();
        controls.Enable();
        controls.Player.AddItem.performed += OnAddItem;
    }

    private void OnDestroy()
    {
        controls.Player.AddItem.performed -= OnAddItem;
        controls.Dispose();
    }

    private void OnAddItem(InputAction.CallbackContext context)
    {
        Debug.Log("Dodajê dwa ró¿ne itemy!");
        inventory.Add(item1, 10); // Dodaje pierwszy item
        inventory.Add(item2, 5);  // Dodaje drugi item (mo¿esz ustawiæ dowoln¹ iloœæ)
    }
}
