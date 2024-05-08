using UnityEngine;

public class HurdleSpawner : MonoBehaviour
{
    Measurer measurer;

    public GameObject hurdle;

    private float INITIAL_X_POS_SPAWN_POINT = 2f;
    private const float INITIAL_Y_POS_SPAWN_POINT = 0.1f;
    private float INITIAL_Z_POS_SPAWN_POINT = 2f;

    private Quaternion FIXED_SPAWN_ROTATION = Quaternion.Euler(0f, 90f, 0f);
    private Vector3 FIXED_SPAWN_SCALE = new Vector3(0.3f, 0.3f, 0.55f);

    public float lengthOffset = 2f;
    public int hurdleCountMaxLimit = 10;

    // Start is called before the first frame update
    void Start()
    {
        measurer = GameObject.FindObjectOfType<Measurer>();

        hurdleCountMaxLimit = (int)((measurer.GetRoadLength() * 0.75f) / lengthOffset);

        INITIAL_Z_POS_SPAWN_POINT = measurer.GetRoadLength() * 0.1f;

        for (int i = 0; i < hurdleCountMaxLimit; i++)
        {
            spawnHurdle(i);
        }
    }

    void spawnHurdle(int hurdleIndex)
    {
        float offset = INITIAL_Z_POS_SPAWN_POINT + lengthOffset * hurdleIndex + Random.Range(0f, lengthOffset);

        Vector3 spawnPosition = new Vector3(INITIAL_X_POS_SPAWN_POINT, INITIAL_Y_POS_SPAWN_POINT, offset);
        GameObject newHurdle = Instantiate(hurdle, spawnPosition, FIXED_SPAWN_ROTATION);
        newHurdle.transform.localScale = FIXED_SPAWN_SCALE;
    }
}
