using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public bool MoveByTouch;
    private Rigidbody PlrRb;
    private Animator StickMan_Anim;

    public float walkSpeed = 1f;
    public float jumpForce = 5f;
    public bool isGrounded = false;

    void Start()
    {
        PlrRb = GetComponentInChildren<Rigidbody>();
        StickMan_Anim = GetComponentInChildren<Animator>();

        GameObject groundPrefab = GameObject.FindGameObjectWithTag("Ground");
        if (groundPrefab != null)
        {
            Debug.Log("Ground prefab found!");
            // You can perform additional actions here if the Ground prefab is found
        }
        else
        {
            Debug.Log("No Ground prefab found in the scene!");
            // Handle the case where no Ground prefab is found (e.g., spawn one dynamically)
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MoveByTouch = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            MoveByTouch = false;
        }

        if (Input.GetKeyDown(KeyCode.Space)) // Check for spacebar key press to trigger jump
        {
            TryJump();
        }

        if (MoveByTouch)
        {
            MovePlayer();
        }
        else
        {
            StickMan_Anim.SetFloat("run", 0f); // Stop walk animation if not moving
        }
    }

    void MovePlayer()
    {
        // Calculate movement direction based on input
        float moveInput = Input.GetAxis("Horizontal"); // Assuming horizontal movement
        Vector3 movement = new Vector3(moveInput, 0f, 1f) * walkSpeed * Time.deltaTime;

        // Apply movement to the player's position
        transform.Translate(movement, Space.World);

        StickMan_Anim.SetFloat("run", 1f); // Trigger walk animation based on movement
    }

    void TryJump()
    {
        if (isGrounded)
        {
            Debug.Log("Jumping!");
            PlrRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            StickMan_Anim.SetTrigger("jump"); // Trigger jump animation
        }
        else
        {
            Debug.Log("Not grounded, cannot jump!");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Entering: ");
        Debug.Log(collision.gameObject.tag);
        // Check if the player collides with a ground object (tagged as "Ground" for example)
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log("Exiting: ");
        Debug.Log(collision.gameObject.tag);

        // Reset isGrounded when the player leaves the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
