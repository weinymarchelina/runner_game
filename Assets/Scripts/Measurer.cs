using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Measurer : MonoBehaviour
{
    private float totalLength = 0f;
    void Start()
    {
        totalLength = CalculateTotalLength();
        Debug.Log("Total Length of the Road: " + totalLength);
    }

    float CalculateTotalLength()
    {
        // Get the bounding box of the current GameObject's visual representation
        Bounds totalBounds = new Bounds(Vector3.zero, Vector3.zero);

        // Iterate over all child GameObjects that have a Renderer component
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            // Encapsulate the child renderer's bounds into the totalBounds
            totalBounds.Encapsulate(renderer.bounds);
        }

        // Calculate and print the total length (extents) of all child prefabs
        return totalBounds.size.z;
    }

    public float GetRoadLength()
    {
        return totalLength;
    }
}