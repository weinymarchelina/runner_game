using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Windows.Kinect;
using Joint = Windows.Kinect.Joint;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class BodySourceView : MonoBehaviour
{
    public BodySourceManager mBodySourceManager;
    public GameObject mJointObject;
    public PlayerManager mPlayerManager1; // For Player 1
    public PlayerManager mPlayerManager2; // For Player 2

    private float upHandThreshold = 5f;

    // Define the boundaries for the domain
    //public float minX = -10;
    //public float maxX = 0f;
    //public float minY = 0f;
    //public float maxY = 10f;

    private Dictionary<ulong, GameObject> mBodies = new Dictionary<ulong, GameObject>();
    private Dictionary<ulong, PlayerManager> playerTracking = new Dictionary<ulong, PlayerManager>();
    private List<JointType> _joints = new List<JointType>
    {
        JointType.HandLeft,
        JointType.HandRight,
    };

    void Update()
    {
        if (mBodySourceManager == null)
        {
            Debug.LogError("BodySourceManager is not assigned.");
            return;
        }

        // Check active scene and switch functionality
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            HandleCursorControl();
        }
        else if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            HandleAvatarControl();
        }
    }

    void HandleCursorControl()
    {
        #region Get Kinect data
        Body[] data = mBodySourceManager.GetData();
        if (data == null)
        {
            Debug.Log("No data received from BodySourceManager.");
            return;
        }

        foreach (var body in data)
        {
            if (body == null || !body.IsTracked)
            {
                continue;
            }

            Joint leftHand = body.Joints[JointType.HandLeft];
            Joint rightHand = body.Joints[JointType.HandRight];
            Vector3 leftHandPos = GetVector3FromJoint(leftHand);
            Vector3 rightHandPos = GetVector3FromJoint(rightHand);

            if (leftHandPos.y > upHandThreshold && rightHandPos.y > upHandThreshold)
            {
                // Both hands raised: simulate click
                SimulateClick();
            }
        }
        #endregion
    }

    void HandleAvatarControl()
    {
        #region Get Kinect data
        Body[] data = mBodySourceManager.GetData();
        if (data == null)
        {
            Debug.Log("No data received from BodySourceManager.");
            return;
        }

        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }
        #endregion

        #region Delete Kinect Bodies
        List<ulong> knownIds = new List<ulong>(mBodies.Keys);
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                // Destroy body object
                Destroy(mBodies[trackingId]);
                mBodies.Remove(trackingId);
                playerTracking.Remove(trackingId); // Remove from player tracking as well
                Debug.Log("Destroyed body object for tracking ID: " + trackingId);
            }
        }
        #endregion

        #region Create Kinect Bodies
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                if (!mBodies.ContainsKey(body.TrackingId))
                {
                    mBodies[body.TrackingId] = CreateBodyObject(body.TrackingId);

                    // Assign player manager based on the number of tracked players
                    if (playerTracking.Count == 0)
                    {
                        playerTracking[body.TrackingId] = mPlayerManager1;
                    }
                    else if (playerTracking.Count == 1)
                    {
                        playerTracking[body.TrackingId] = mPlayerManager2;
                    }

                    Debug.Log("Created body object for tracking ID: " + body.TrackingId);
                }

                UpdateBodyObject(body, mBodies[body.TrackingId], playerTracking[body.TrackingId]);
                Debug.Log("Updated body object for tracking ID: " + body.TrackingId);
            }
        }
        #endregion
    }

    private void SimulateClick()
    {
        // Simulate a click action here
        Debug.Log("Simulate click action with both hands raised.");

        // Check for EventSystem to ensure UI interaction
        if (EventSystem.current == null)
        {
            Debug.LogWarning("No EventSystem found. Ensure there is an EventSystem in the scene.");
            return;
        }

        // Create a pointer event
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = -1,
            position = new Vector2(Screen.width / 2, Screen.height / 2) // Assuming center screen for cursor
        };

        // Raycast to find the UI element under the cursor
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        if (results.Count > 0)
        {
            GameObject clickedObject = results[0].gameObject;
            Debug.Log("Clicked on: " + clickedObject.name);

            // Simulate the click event on the UI element
            ExecuteEvents.Execute(clickedObject, pointerData, ExecuteEvents.pointerClickHandler);
        }
    }

    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);

        foreach (JointType joint in _joints)
        {
            GameObject newJoint = Instantiate(mJointObject);
            newJoint.name = joint.ToString();
            newJoint.transform.parent = body.transform;

            // Set the initial position within the desired range
            //newJoint.transform.position = new Vector3(
            //    Mathf.Clamp(0, minX, maxX), // Start in the middle of the X range
            //    Mathf.Clamp(0, minY, maxY), // Start in the middle of the Y range
            //    0 // Ensure the z position is 0 for 2D appearance
            //);
        }

        return body;
    }

    private void UpdateBodyObject(Body body, GameObject bodyObject, PlayerManager playerManager)
    {
        bool jump = true;
        bool run = false;

        foreach (JointType _joint in _joints)
        {
            Joint sourceJoint = body.Joints[_joint];
            Vector3 targetPosition = GetVector3FromJoint(sourceJoint);

            // Debug the raw target position
            Debug.Log($"Raw target position for {_joint}: {targetPosition}");

            // Clamp the positions to stay within the defined domain
            targetPosition.z = 0; // Ensure the z position is 0 for 2D appearance

            // Debug the clamped target position
            Debug.Log($"Clamped target position for {_joint}: {targetPosition}");

            Transform jointObject = bodyObject.transform.Find(_joint.ToString());
            if (jointObject != null)
            {
                jointObject.position = targetPosition;
            }

            // Check if left hand is raised
            if (_joint == JointType.HandLeft)
            {
                if (targetPosition.y < upHandThreshold) // Adjust the threshold as necessary
                {
                    jump = false;
                }
            }

            if (_joint == JointType.HandRight)
            {
                if (targetPosition.y >= upHandThreshold)
                {
                    Debug.Log("IJJJDJIsndusda");
                    run = true;
                }
            }
        }

        // If left hand is raised, trigger the jump action
        if (jump && playerManager != null && playerManager.CanWalk())
        {
            playerManager.ActionJump();
        }

        if (run && playerManager != null && playerManager.CanWalk())
        {
            playerManager.ActionRun();
        }
    }

    private static Vector3 GetVector3FromJoint(Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
}