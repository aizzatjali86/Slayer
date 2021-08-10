using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGrass : MonoBehaviour
{
    public float radius;
    public Vector2 regionSize = Vector2.one;
    public int rejectionSamples;
    public float displayRadius;

    public GameObject prefTree;

    List<Vector2> points;

    private void OnValidate()
    {
        points = PossionDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples);
    }

    private void OnDrawGizmos()
    {
        Vector3 wire = new Vector3(regionSize.x, 0, regionSize.y);
        Vector3 center = new Vector3(regionSize.x / 2, 0, regionSize.y / 2);
        Gizmos.DrawWireCube(center, wire);
        if (points != null)
        {
            foreach (Vector2 point in points)
            {
                Vector3 position = new Vector3(point.x, 0, point.y);
                Gizmos.DrawSphere(position, displayRadius);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DrawTrees();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DrawTrees()
    {

        if (points != null)
        {
            foreach (Vector2 point in points)
            {
                Vector3 position = new Vector3(point.x - regionSize.x /2, -0.2f, point.y - regionSize.y / 2);
                Instantiate(prefTree, position, Quaternion.identity);
            }
        }

    }
}
