using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput controls;
    private Vector2 moveInput;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed = 5f;

    private void Awake()
    {
        controls = new PlayerInput();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
    private void FixedUpdate()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed * Time.fixedDeltaTime;
        Vector3 targetPosition = rb.position + move;

        // Ograniczenie pozycji do granic mapy
        if (MapGenerator.MapMin != MapGenerator.MapMax) // Sprawd� czy granice s� ustawione
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, MapGenerator.MapMin.x, MapGenerator.MapMax.x);
            targetPosition.z = Mathf.Clamp(targetPosition.z, MapGenerator.MapMin.y, MapGenerator.MapMax.y);
        }

        rb.MovePosition(targetPosition);

        // Teleportacja na �rodek, je�li gracz spadnie poni�ej y = -50
        if (rb.position.y < -50f)
        {
            rb.position = new Vector3(0f, 5f, 0f);
            rb.linearVelocity = Vector3.zero;
        }
    }
}