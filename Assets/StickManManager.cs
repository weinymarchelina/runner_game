using System.Collections;
using UnityEngine;

public class StickManManager : MonoBehaviour
{
    public bool MoveByTouch;
    public Rigidbody PlrRb;
    Animator animator;
    float velocity = 0.5f;
    public float accleration = 0.9f;
    public float decelaration = 0.1f;
    int VelocityHash;

    public float walkSpeed = 1f;
    public float jumpForce = 5f;
    public float leapForwardForce = 1.5f;
    public bool isGrounded = false;
    private bool canMove = true;
    private bool reachedFinal = false;

    private Vector3 targetPosition; // Target position to smoothly move towards
    private Quaternion targetRotation; // Target rotation (180 degrees around Y-axis)

    private void Start()
    {
        animator = GetComponent<Animator>();
        VelocityHash = Animator.StringToHash("Velocity");
    }

    void Update()
    {
        bool walkPressed = Input.GetMouseButtonDown(0);
        bool stopPressed = Input.GetMouseButtonUp(0);
        bool runPressed = Input.GetKey(KeyCode.UpArrow);
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);


        if (walkPressed)
        {
            MoveByTouch = true;
        }

        if (stopPressed)
        {
            MoveByTouch = false;
        }

        if (jumpPressed) 
        {
            TryJump();
        }

        if (MoveByTouch && canMove)
        {
            MovePlayer();
        }
        else
        {
            if (velocity > 0.0f)
            {
                velocity -= Time.deltaTime * decelaration;
            }
            else
            {
                velocity = 0.0f;
            }
        }

        if (reachedFinal && !canMove)
        {
            float t = Time.deltaTime * 2.5f; // Adjust the interpolation speed if needed
            transform.position = Vector3.Lerp(transform.position, targetPosition, t);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);

            // Check if the distance between current position and target position is small enough to consider reached
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition; // Snap to the final position
                transform.rotation = targetRotation; // Snap to the final rotation
            }
        }

        animator.SetFloat(VelocityHash, velocity);
    }

    void MovePlayer()
    {
        if (!reachedFinal)
        {
            // Calculate movement direction based on input
            float moveInput = Input.GetAxis("Horizontal"); // Assuming horizontal movement
            Vector3 movement = new Vector3(moveInput, 0f, 1f) * walkSpeed * Time.deltaTime;
            
            if (velocity < 1.0f)
            {
                velocity += Time.deltaTime * accleration;
            }

            // Apply movement to the player's position
            transform.Translate(movement, Space.World);

            // animator.SetFloat("run", 1f); // Trigger walk animation based on movement
        }
    }

    void TryJump()
    {
        if (isGrounded)
        {
            Debug.Log("Jumping!");
            PlrRb.AddForce(new Vector3(0, jumpForce, leapForwardForce), ForceMode.Impulse);
            // StickMan_Anim.SetTrigger("jump"); // Trigger jump animation
        }
        else
        {
            Debug.Log("Not grounded, cannot jump!");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Entering: " + collision.gameObject.tag);

        // Check if the player collides with a hurdle (tagged as "Hurdle")
        if (collision.gameObject.CompareTag("Hurdle"))
        {
            animator.SetBool("IsFalling", true);

            Rigidbody hurdleRb = collision.gameObject.GetComponent<Rigidbody>();

            Quaternion currentRotation = hurdleRb.rotation;
            Quaternion newRotation = Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y, 90f);
            hurdleRb.MoveRotation(newRotation);

            Vector3 currentPosition = hurdleRb.position;
            Vector3 newPosition = new Vector3(currentPosition.x, -0.5f, currentPosition.z);
            hurdleRb.MovePosition(newPosition);

            StartCoroutine(DisableMovementForDuration(5f));

            
        }

        // Check if the player collides with a ground object (tagged as "Ground")
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Final"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Final"))
        {
            reachedFinal = true;
            canMove = false; // Disable further movement
            targetPosition = new Vector3(transform.position.x, transform.position.y, 20f);
            targetRotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + 180f, 0f);
            animator.SetBool("IsWinning", true);
            Debug.Log("Reached Final!");
        }
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log("Exiting: " + collision.gameObject.tag);

        // Reset isGrounded when the player leaves the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    IEnumerator DisableMovementForDuration(float duration)
    {
        canMove = false;
        isGrounded = false;
        yield return new WaitForSeconds(duration);
        Debug.Log("Awaken!");
        isGrounded = true;
        canMove = true;
        animator.SetBool("IsFalling", false);
        PlrRb.AddForce(new Vector3(0, 1f, 0), ForceMode.Impulse);
    }
}
