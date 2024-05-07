using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public float stopFollowZPosition = 10f;
    public CinemachineVirtualCamera virtualCamera;

    private bool stopFollowing = false;

    void Start()
    {
        // Get the Cinemachine Virtual Camera component
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        if (!stopFollowing && virtualCamera.transform.position.z >= stopFollowZPosition)
        {
            stopFollowing = true;

            // Stop following the player
            virtualCamera.Follow = null;
            virtualCamera.LookAt = null;

            virtualCamera.transform.position = new Vector3(0f, 1f, 17f);
            Quaternion newRotation = Quaternion.Euler(3f, 0f, 0f);
            virtualCamera.transform.rotation = newRotation;
        }
    }
}
