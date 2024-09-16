using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using RenderSettings = UnityEngine.RenderSettings;

public class SpectatorCamera : MonoBehaviour
{
    public float movementSpeed = 16f;
    public float sensitivity = 4f;
    public float yPosition = 6.5f;
    public float minHeight = 1.5f;
    public float maxHeight = 45f;

    private float _rotationX;
    private bool _mouseLocked;
    private Vector3? _lockedMousePosition;

    private void Start()
    {
        var localTransform = transform;
        Vector3 startPos = localTransform.position;
        startPos.y = yPosition;
        _rotationX = localTransform.rotation.eulerAngles.x;
        localTransform.position = startPos;
    }

    private void Update()
    {
        Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        inputDirection.Normalize();
        float verticalMovement = 0f;
        if (Input.GetKey(KeyCode.Space) && transform.position.y <= maxHeight)
        {
            verticalMovement = 1f;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && transform.position.y >= minHeight)
        {
            verticalMovement = -1f;
        }

        Vector3 worldDirection = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * inputDirection;
        Vector3 horizontalMovement = worldDirection * (movementSpeed * Time.deltaTime);
        Vector3 verticalMovementVector = Vector3.up * (verticalMovement * movementSpeed * Time.deltaTime);
        Vector3 movement = horizontalMovement + verticalMovementVector;
        transform.position += movement;

        if (Input.GetMouseButtonDown(1))
        {
            _lockedMousePosition = Input.mousePosition;
            Cursor.lockState = CursorLockMode.Locked;
            _mouseLocked = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None;
            _mouseLocked = false;
        }

        if (Input.GetMouseButton(1) && _mouseLocked)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            _rotationX -= Input.GetAxis("Mouse Y") * sensitivity;
            _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);

            // Apply Y rotation and lock X and Z rotations to zero
            transform.rotation = Quaternion.Euler(_rotationX, transform.rotation.eulerAngles.y + mouseX, 0);
            Input.mousePosition.Set(_lockedMousePosition.Value.x, _lockedMousePosition.Value.y,
                _lockedMousePosition.Value.z);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Time.timeScale = 4.0f;
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            Time.timeScale = 1.0f;
        }
    }
}