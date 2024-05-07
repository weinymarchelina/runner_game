using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurdleSpawnScript : MonoBehaviour
{
    public GameObject hurdle;
    public float lengthOffset = 2.5f;
    int hurdleCount = 0;
    float startPoint = -0.55f;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            spawnHurdle();
            hurdleCount++;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void spawnHurdle()
    {
        // Set the specific position, rotation, and scale for the instantiated hurdle
        float offset = startPoint + lengthOffset*hurdleCount + Random.Range(0f, lengthOffset);
        Vector3 spawnPosition = new Vector3(2f, 0.1f, offset);
        Quaternion spawnRotation = Quaternion.Euler(0f, 90f, 0f); // Rotation in Euler angles (0, 90, 0)
        Vector3 spawnScale = new Vector3(0.3f, 0.3f, 0.55f);

        GameObject newHurdle = Instantiate(hurdle, spawnPosition, spawnRotation);
        newHurdle.transform.localScale = spawnScale;
    }
}
