using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 10f;

    [Header("Camera")]
    public float mouseSensitivity = 2f;
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 60f;
    private float rotationX = 0f;
    private float rotationY = 0f;

    [Header("Attack")]
    public float attackRange = 2f;
    public int attackDamage = 20;
    public Transform attackPoint;
    public LayerMask enemyLayer;

    [Header("References")]
    public Animator animator; // DRAG Darius Maximus here in Inspector

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Try to find animator automatically if not assigned
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator == null)
            animator = GetComponentInParent<Animator>();

        if (animator == null)
            Debug.LogError("❌ Animator NOT found! Please assign it manually in Inspector.");
        else
            Debug.Log("✅ Animator found on: " + animator.gameObject.name);

        // Lock cursor for camera control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleCameraRotation();
        HandleAttack();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D
        float moveZ = Input.GetAxis("Vertical");   // W/S

        // Check if running (Left Shift)
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : moveSpeed;

        Vector3 move = new Vector3(moveX, 0f, moveZ) * currentSpeed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        // Safety check
        if (animator == null) return;

        // Set animation float
        float speed = new Vector3(moveX, 0f, moveZ).magnitude;

        if (isRunning && speed > 0)
            animator.SetFloat("Walk", 5f); // Walk > 4 triggers Run
        else
            animator.SetFloat("Walk", speed); // 0 = Idle, >0 = Walk

        // Rotate player toward movement direction
        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(
                new Vector3(move.x, 0, move.z)
            );
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 10f
            );
        }
    }

    void HandleCameraRotation()
    {
        rotationX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotationY = Mathf.Clamp(rotationY, minVerticalAngle, maxVerticalAngle);

        transform.rotation = Quaternion.Euler(0, rotationX, 0);
    }

    void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (animator != null)
                animator.SetTrigger("Attack");

            Attack();
        }
    }

    void Attack()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("⚠️ Attack Point not assigned!");
            return;
        }

        Collider[] enemies = Physics.OverlapSphere(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        foreach (Collider enemy in enemies)
        {
            Debug.Log("Hit: " + enemy.name);

            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(attackDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}