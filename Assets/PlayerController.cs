using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;          // drag the character's Animator here (or leave empty if on same object / child)

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float turnSpeed = 12f;

    [Header("Jump")]
    public float jumpHeight = 1.2f;
    public float gravity = -20f;

    private CharacterController controller;
    private Vector3 velocity;          // only the vertical part is used
    private float currentSpeed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        animator.applyRootMotion = false;   // the script moves the player, not the animation
    }

    void Update()
    {
        // ---------- Horizontal movement ----------
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0f, v).normalized;

        // Move relative to the camera
        Vector3 moveDir = input;

        bool isMoving = moveDir.sqrMagnitude > 0.01f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = isMoving ? (isRunning ? runSpeed : walkSpeed) : 0f;
        currentSpeed = targetSpeed;

        if (isMoving)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }

        // ---------- Jump + gravity ----------
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;                       // keeps the controller glued to the ground

        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }

        velocity.y += gravity * Time.deltaTime;

        // ---------- Apply movement ----------
        Vector3 move = moveDir * currentSpeed + Vector3.up * velocity.y;
        controller.Move(move * Time.deltaTime);

        // ---------- Attack (extra) ----------
        if (Input.GetMouseButtonDown(0))
            animator.SetTrigger("Attack");

        // ---------- Feed the Animator ----------
        animator.SetFloat("Speed", currentSpeed, 0.1f, Time.deltaTime);
        animator.SetBool("IsGrounded", controller.isGrounded);
    }
}