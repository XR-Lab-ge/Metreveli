using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.3f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private bool isGrounded;
    private Camera cam;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;

        // მაუსი იმალება
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // მიწაზეა?
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundRadius,
            groundLayer);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector3(
                rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(
                Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // მოძრაობა
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 camFwd = cam.transform.forward;
        Vector3 camRight = cam.transform.right;

        camFwd.y = 0f;
        camRight.y = 0f;
        camFwd.Normalize();
        camRight.Normalize();

        Vector3 move = (camFwd * v + camRight * h).normalized;

        rb.MovePosition(rb.position +
            move * moveSpeed * Time.fixedDeltaTime);
    }
}