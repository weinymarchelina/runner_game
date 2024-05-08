using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    private Measurer measurer;

    public float stopFollowZPosition = 10f;
    private bool stopFollowing = false;

    private Vector3 targetPosition; // Target position to smoothly move towards
    private Quaternion targetRotation = Quaternion.Euler(3f, 0f, 0f); // Target rotation (180 degrees around Y-axis)

    void Start()
    {
        // Get the Cinemachine Virtual Camera component
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        measurer = GameObject.FindObjectOfType<Measurer>();

        stopFollowZPosition = measurer.GetRoadLength() * 0.95f;
        targetPosition = new Vector3(0f, 1f, stopFollowZPosition);
    }

    void Update()
    {
        if (!stopFollowing && virtualCamera.transform.position.z >= stopFollowZPosition)
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
