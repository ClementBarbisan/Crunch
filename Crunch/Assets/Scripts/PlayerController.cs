using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    /*[Header("Movement")]
    public float moveSpeedSlow = 5f, moveSpeedFast = 10f;
    [SerializeField] float acceleration = 10f;
    [SerializeField] float drag = 4f;
    
    [Header(("Look"))]
    [SerializeField] private float speedRotate = 10f;

    [HideInInspector]
    public float speedMove;
    private Camera _cam;
    private InputSystem_Actions _controls;
    private Vector2 _moveInput;
    private Rigidbody _rb;
   
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _controls = new InputSystem_Actions();
   
        _controls.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

        _cam = Camera.main;
        //Cursor.lockState = CursorLockMode.Confined;

        speedMove = moveSpeedFast;
    }
   
    void OnEnable() => _controls.Enable();
    void OnDisable() => _controls.Disable();
    
    void Update()
    {
        RotateTowardMouse();
    }
    
    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        Vector3 moveDirection = transform.forward;
        Vector3 targetVelocity = moveDirection.normalized * speedMove;
        Vector3 velocityChange = targetVelocity - _rb.linearVelocity;
        velocityChange.y = 0;

        _rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);
        
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x * (1 - drag * Time.fixedDeltaTime), _rb.linearVelocity.y, _rb.linearVelocity.z * (1 - drag * Time.fixedDeltaTime));
    }

    void RotateTowardMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); 

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            Vector3 direction = hitPoint - transform.position;
            direction.y = 0; 
            transform.forward = direction.normalized;
        }
    }*/
    
    [Header("Movement")]
    public float moveSpeedSlow = 5f, moveSpeedFast = 10f;
    [HideInInspector]
    public float speedMove;
    [SerializeField] float acceleration = 10f;
    [SerializeField] float drag = 4f;

    [Header("Look")]
    [SerializeField] Transform bodyToRotate;
    private Camera _cam;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private InputSystem_Actions _controls;

    void Awake()
    {
        _controls = new InputSystem_Actions();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        _cam = Camera.main;
        speedMove = moveSpeedFast;
    }
    
    void OnEnable()
    {
        _controls.Enable();
        _controls.Player.Move.performed += OnMove;
        _controls.Player.Move.canceled += OnStop;
    } 
    void OnDisable()
    {
        _controls.Disable();
        _controls.Player.Move.performed -= OnMove;
        _controls.Player.Move.canceled -= OnStop;
    }

    void Update()
    {
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        
        Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 target = ray.GetPoint(distance);
            Vector3 lookDir = target - transform.position;
            lookDir.y = 0;

            if (lookDir.sqrMagnitude > 0.01f)
                bodyToRotate.forward = Vector3.Lerp(bodyToRotate.forward, lookDir.normalized, Time.deltaTime * 50f);
        }
    }

    void FixedUpdate()
    {
        Vector3 targetVelocity = moveDirection.normalized * speedMove;
        Vector3 velocityChange = targetVelocity - rb.linearVelocity;
        velocityChange.y = 0;

        rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);
        
        rb.linearVelocity = new Vector3(rb.linearVelocity.x * (1 - drag * Time.fixedDeltaTime), rb.linearVelocity.y, rb.linearVelocity.z * (1 - drag * Time.fixedDeltaTime));
    }
    
    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnStop(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }
    
    
}
