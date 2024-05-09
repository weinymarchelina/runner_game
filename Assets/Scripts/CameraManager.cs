using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    private Measurer measurer;

    public float stopFollowZPosition = 30f;
    public float stopFollowXPositionPlayer1 = 0f;
    public float stopFollowXPositionPlayer2 = 20f;

    private bool stopFollowing = false;

    private Vector3 targetPosition; // Target position to smoothly move towards
    private Quaternion targetRotation = Quaternion.Euler(3f, 0f, 0f); // Target rotation (180 degrees around Y-axis)

    void Start()
    {
        StartCoroutine(InitializeCameraManager());
    }

    IEnumerator InitializeCameraManager()
    {
        // Get the Cinemachine Virtual Camera component
        virtualCamera = GetComponent<CinemachineVirtualCamera>();

        // Wait for the Measurer component to be initialized
        yield return WaitForMeasurerInitialization();

        // Calculate stopFollowZPosition based on road length
        stopFollowZPosition = measurer.GetRoadLength() * 0.95f;

        // Determine targetPosition based on the tag of the followed object (Player1 or Player2)
        if (virtualCamera.Follow != null)
        {
            string followTag = virtualCamera.Follow.tag;

            if (followTag == "Player1")
            {
                targetPosition = new Vector3(stopFollowXPositionPlayer1, 1f, stopFollowZPosition);
            }
            else if (followTag == "Player2")
            {
                targetPosition = new Vector3(stopFollowXPositionPlayer2, 1f, stopFollowZPosition);
            }
            else
            {
                Debug.LogWarning("Followed object has an unrecognized tag.");
            }
        }
    }

    IEnumerator WaitForMeasurerInitialization()
    {
        while (measurer == null)
        {
            measurer = GameObject.FindObjectOfType<Measurer>();
            yield return null; // Wait for one frame before checking again
        }
    }

    void Update()
    {
        if (!stopFollowing && virtualCamera != null && virtualCamera.transform.position.z >= stopFollowZPosition)
        {
            stopFollowing = true;

            // Stop following the player
            virtualCamera.Follow = null;
            virtualCamera.LookAt = null;

            virtualCamera.transform.position = targetPosition;

            Quaternion newRotation = targetRotation;
            virtualCamera.transform.rotation = newRotation;
        }
    }
}
