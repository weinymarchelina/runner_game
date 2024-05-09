using UnityEngine;
using System.Collections;

public class FinalPlatformManager : MonoBehaviour
{
    private Measurer measurer;
    private float zPos;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitializeFinalPlatform());
    }

    IEnumerator InitializeFinalPlatform()
    {
        // Wait for Measurer component to be initialized
        yield return WaitForMeasurerInitialization();

        // Get road length from Measurer and set the platform's Z position accordingly
        zPos = measurer.GetRoadLength() + 1.5f;
        transform.position = new Vector3(0, 0, zPos);
    }

    IEnumerator WaitForMeasurerInitialization()
    {
        while (measurer == null)
        {
            measurer = GameObject.FindObjectOfType<Measurer>();
            yield return null; // Wait for one frame before checking again
        }
    }

    // Update is called once per frame
    void Update()
    {
        // You can add update logic here if needed
    }
}
