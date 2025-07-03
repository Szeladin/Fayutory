using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private string _horizontalInputAxis = "Horizontal",
                  _verticalInputAxis = "Vertical";
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField]
    private float _moveSpeed = 5f;

    private Vector2 _input;
    private void FixedUpdate()
    {
        _rigidbody.linearVelocity = _input * _moveSpeed;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw(_horizontalInputAxis);
        float verticalInput = Input.GetAxisRaw(_verticalInputAxis);
        _input = new Vector2(horizontalInput, verticalInput).normalized;
    }
}
    