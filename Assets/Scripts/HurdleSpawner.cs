using UnityEngine;
using System.Collections;

public class HurdleSpawner : MonoBehaviour
{
    private Measurer measurer;
    public GameObject hurdle;

    private const float INITIAL_X_POS_SPAWN_POINT_P1 = 2f;
    private const float INITIAL_X_POS_SPAWN_POINT_P2 = 22f;
    private const float INITIAL_Y_POS_SPAWN_POINT = 0.1f;
    private float INITIAL_Z_POS_SPAWN_POINT = 2f;

    private Quaternion FIXED_SPAWN_ROTATION = Quaternion.Euler(0f, 90f, 0f);
    private Vector3 FIXED_SPAWN_SCALE = new Vector3(0.3f, 0.3f, 0.55f);
    private Vector3 spawnPosition;

    public float lengthOffset = 2f;
    public int hurdleCountMaxLimit = 10;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitializeSpawner());
    }

    IEnumerator InitializeSpawner()
    {
        yield return WaitForMeasurerInitialization();

        Debug.Log("The road length would be " + measurer.GetRoadLength());
        hurdleCountMaxLimit = (int)((measurer.GetRoadLength() * 0.75f) / lengthOffset);
        Debug.Log("We would have " + hurdleCountMaxLimit.ToString() + " hurdles");

        INITIAL_Z_POS_SPAWN_POINT = measurer.GetRoadLength() * 0.1f;

        for (int i = 0; i < hurdleCountMaxLimit; i++)
        {
            spawnHurdle(i);
            Debug.Log("Spawning hurdle at " + transform.tag);
            yield return null; // Yield to allow one frame for each hurdle spawn
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

    void spawnHurdle(int hurdleIndex)
    {
        float offset = INITIAL_Z_POS_SPAWN_POINT + lengthOffset * hurdleIndex + Random.Range(0f, lengthOffset);

        if (transform.tag == "HurdleSpawner1")
        {
            spawnPosition = new Vector3(INITIAL_X_POS_SPAWN_POINT_P1, INITIAL_Y_POS_SPAWN_POINT, offset);
        }
        else if (transform.tag == "HurdleSpawner2")
        {
            spawnPosition = new Vector3(INITIAL_X_POS_SPAWN_POINT_P2, INITIAL_Y_POS_SPAWN_POINT, offset);
        }
        else
        {
            Debug.Log(transform.tag + " is not valid");
        }

        GameObject newHurdle = Instantiate(hurdle, spawnPosition, FIXED_SPAWN_ROTATION);
        newHurdle.transform.localScale = FIXED_SPAWN_SCALE;
    }
}
