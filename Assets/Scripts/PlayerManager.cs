using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    private Rigidbody playerRB;
    private AudioSource footstepsSound;
    private Animator animator;
    private AudioManager audioManager;
    private Measurer measurer;
    private LogicManager logicManager;

    private const float BASE_ACCELARATION = 0.5f;
    private const float BASE_WALK_SPEED = 1f;
    private const float BASE_INTERPOLATION_SPEED = 2.5f;

    private float finalStandingPositionZ = 25f;

    private float walkSpeed = BASE_WALK_SPEED;
    private float jumpForce = 1.3f;
    private float leapLength = 1f;

    private bool isMoveByTouch = false;
    private bool isOnGround = false;
    private bool isAbleToWalk = true;
    private bool hasReachedFinal = false;
    
    private Vector3 targetPosition; // Target position to smoothly move towards
    private Quaternion targetRotation; // Target rotation (180 degrees around Y-axis)

    private void Start()
    {
        StartCoroutine(InitializePlayer());
    }

    IEnumerator InitializePlayer()
    {
        // Get necessary components
        playerRB = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        footstepsSound = GetComponent<AudioSource>();
        logicManager = GameObject.FindObjectOfType<LogicManager>();

        // Wait for Measurer component to be initialized
        yield return WaitForMeasurerInitialization();

        // Calculate final standing position Z based on road length
        finalStandingPositionZ = measurer.GetRoadLength() + 1.5f;

    }

    IEnumerator WaitForMeasurerInitialization()
    {
        while (measurer == null)
        {
            measurer = GameObject.FindObjectOfType<Measurer>();
            yield return null; // Wait for one frame before checking again
        }
    }

    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
    void Update()
    {
        // Handle input using switch-case
        if (!logicManager.hasGameEnd)
        {
            switch (GetInputType())
            {
                case InputType.Walk:
                    isMoveByTouch = true;
                    break;

                case InputType.Stop:
                    isMoveByTouch = false;
                    animator.SetBool("IsRunning", false);
                    footstepsSound.Stop();
                    break;

                case InputType.Jump:
                    ActionJump();
                    break;

                default:
                    break;
            }
        }

        // Move player if allowed and not reached final
        if (isMoveByTouch && isAbleToWalk && !hasReachedFinal)
        {
            ActionRun();
        }
        else
        {
            walkSpeed = BASE_WALK_SPEED;
            animator.SetBool("IsRunning", false);
            footstepsSound.Stop();
        }

        // Handle reaching the final destination
        if (hasReachedFinal && !isAbleToWalk)
        {
            float t = Time.deltaTime * BASE_INTERPOLATION_SPEED; // Adjust the interpolation speed if needed
            transform.position = Vector3.Lerp(transform.position, targetPosition, t);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);

            // Check if the distance between current position and target position is small enough to consider reached
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition; // Snap to the final position
                transform.rotation = targetRotation; // Snap to the final rotation
            }

            logicManager.winnerPlayer = transform.tag;
            logicManager.hasGameEnd = true;
        }

        if (logicManager.hasGameEnd)
        {
            isAbleToWalk = false;

            if (logicManager.winnerPlayer != transform.tag)
            {
                animator.SetBool("IsLosing", true);
            }

            string sceneToLoad = (logicManager.winnerPlayer == "Player1") ? "RestartScene" : "RabbitScene";
            StartCoroutine(LoadSceneAfterDelay(sceneToLoad, 5f));
        }
    }

    // Define enum for input types
    public enum InputType
    {
        None,
        Walk,
        Stop,
        Jump
    }

    // Method to determine the current input type
    InputType GetInputType()
    {
        if (gameObject.CompareTag("Player1"))
        {
            // Player 1 uses mouse hold to walk and spacebar to jump
            if (Input.GetMouseButtonDown(0))
            {
                return InputType.Walk;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                return InputType.Stop;
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                return InputType.Jump;
            }
            else
            {
                return InputType.None;
            }
        }
        else if (gameObject.CompareTag("Player2"))
        {
            // Player 2 uses return key to walk and arrow keys to jump
            if (Input.GetKeyDown(KeyCode.Return))
            {
                return InputType.Walk;
            }
            else if (Input.GetKeyUp(KeyCode.Return))
            {
                return InputType.Stop;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                return InputType.Jump;
            }
            else
            {
                return InputType.None;
            }
        }

        return InputType.None;
    }

    public void ActionRun()
    {
        if (!hasReachedFinal)
        {
            // Calculate movement direction based on input
            float moveInput = Input.GetAxis("Horizontal");
            Vector3 movement = new Vector3(moveInput, 0f, BASE_WALK_SPEED) * walkSpeed * Time.deltaTime;

            walkSpeed += Time.deltaTime * BASE_ACCELARATION;
            transform.Translate(movement, Space.World);

            animator.SetBool("IsRunning", true);
            PlayFootstepSound();
        }
    }

    public void ActionJump()
    {
        if (isOnGround)
        {
            Debug.Log("Jumping!");
            audioManager.PlaySFXP1(audioManager.jump);
            audioManager.PlaySFXP2(audioManager.jump);
            playerRB.AddForce(new Vector3(0, jumpForce, leapLength), ForceMode.Impulse);
        }
        else
        {
            Debug.Log("Not grounded, cannot jump!");
        }
    }

    void PlayFootstepSound()
    {
        if (!footstepsSound.isPlaying)
        {
            footstepsSound.Play();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Entering: " + collision.gameObject.tag);

        // Check if the player collides with a hurdle (tagged as "Hurdle")
        if (collision.gameObject.CompareTag("Hurdle"))
        {
            walkSpeed = BASE_WALK_SPEED;
            animator.SetBool("IsFalling", true);

            if (gameObject.CompareTag("Player1"))
            {
                audioManager.PlaySFXP1(audioManager.crash_chicken);
            }
            else
            {
                audioManager.PlaySFXP2(audioManager.crash_rabbit);
            }
                
            // Hurdle Manager
            Rigidbody hurdleRb = collision.gameObject.GetComponent<Rigidbody>();

            Quaternion currentRotation = hurdleRb.rotation;
            Quaternion newRotation = Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y, 90f);
            hurdleRb.MoveRotation(newRotation);

            Vector3 currentPosition = hurdleRb.position;
            Vector3 newPosition = new Vector3(currentPosition.x, -0.5f, currentPosition.z);
            hurdleRb.MovePosition(newPosition);

            // Player Falling State
            StartCoroutine(DisableMovementForDuration(3f));
        }

        // Check if the player collides with a ground object (tagged as "Ground")
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Final"))
        {
            isOnGround = true;
        }

        if (collision.gameObject.CompareTag("Final"))
        {
            hasReachedFinal = true;
            isAbleToWalk = false; // Disable further movement
            targetPosition = new Vector3(transform.position.x, transform.position.y, finalStandingPositionZ);
            targetRotation = Quaternion.Euler(0f, 180f, 0f);
            animator.SetBool("IsWinning", true);
            audioManager.PlaySFXP1(audioManager.cheer);
            audioManager.PlaySFXP2(audioManager.cheer);
            Debug.Log("Reached Final!");
        }
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log("Exiting: " + collision.gameObject.tag);

        // Reset isGrounded when the player leaves the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = false;
        }
    }

    IEnumerator DisableMovementForDuration(float duration)
    {
        isAbleToWalk = false;
        isOnGround = false;
        yield return new WaitForSeconds(duration);
        Debug.Log("Awaken!");
        isOnGround = true;
        isAbleToWalk = true;
        animator.SetBool("IsFalling", false);
        playerRB.AddForce(new Vector3(0, 1f, 0), ForceMode.Impulse);
    }
    public bool CanWalk()
    {
        return isAbleToWalk;
    }
}
