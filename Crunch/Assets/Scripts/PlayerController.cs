using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    [SerializeField] private float speedRotate = 10f;

    private Camera _cam;
    private InputSystem_Actions _controls;
    private Vector2 _moveInput;
    private CharacterController _controller;
   
    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _controls = new InputSystem_Actions();
   
        _controls.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

        _cam = Camera.main;
        //Cursor.lockState = CursorLockMode.Confined;
    }
   
    void OnEnable() => _controls.Enable();
    void OnDisable() => _controls.Disable();
    
    void Update()
    {
        Move();
        RotateTowardMouse();
    }

    void Move()
    {
        Vector3 move = new Vector3(_moveInput.x, 0f, _moveInput.y);
        _controller.Move(move * moveSpeed * Time.deltaTime);
        
        if (transform.position.y != 0f)
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    void RotateTowardMouse()
    {
        Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 targetPos = hitInfo.point;
            Vector3 direction = targetPos - transform.position;
            direction.y = 0f; 

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speedRotate);
            }
        }
    }
}
