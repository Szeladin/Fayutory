using UnityEngine;
using UnityEngine.InputSystem;

public class TestAddItem : MonoBehaviour
{
    public Inventory inventory;
    public Item item;

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
        Debug.Log("Wywo³ano akcjê dodania itemu!");
        inventory.Add(item, 10);
    }
}
