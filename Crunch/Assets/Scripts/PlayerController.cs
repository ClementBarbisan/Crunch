using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    public float moveSpeedSlow = 5f, moveSpeedFast = 10f;
    [HideInInspector]
    public float speedMove;
    [SerializeField] float acceleration = 10f;
    [SerializeField] float drag = 4f;

    [Header("Look")]
    // NB : PlayerInteractor.cs is also modifying animator values
    [SerializeField] Animator animator;
    private string _isWalkingParamName = "isWalking";

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
        animator.SetBool(_isWalkingParamName, true);
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnStop(InputAction.CallbackContext context)
    {
        animator.SetBool(_isWalkingParamName, false);
        moveInput = Vector2.zero;
    }

    public void OnStun(float timeStun)
    {
        StartCoroutine(StunForAWhile(timeStun));
    }

    private IEnumerator StunForAWhile(float timeStun)
    {
        rb.isKinematic = true;
        yield return new WaitForSeconds(timeStun);
    }
}
