using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
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
    }

    private void OnToggleInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isVisible = !isVisible;
            gameObject.SetActive(isVisible);
            Debug.Log("ToggleInventory event!");
        }
    }
}
