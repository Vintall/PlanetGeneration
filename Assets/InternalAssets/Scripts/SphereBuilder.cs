using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SphereBuilder : MonoBehaviour
{
    [SerializeField, Range(1, 5)] float radius = 1;
    [SerializeField, Range(5, 320)] int pointsPerAxis;
    [SerializeField] Material meshMaterial;

    [SerializeField, Header("Create when entered play mode")] bool createOnStart;
    [SerializeField, Header("First triange out of quad")] bool fillFirstTriangle;
    [SerializeField, Header("Second triange out of quad")] bool fillSecondTriangle;
    [SerializeField] bool fillBackwardTriangle;
    [SerializeField, Range(0f, 1f), Header("0/1 - cube/sphere")] float interpolationValue;

    void Start()
    {
        if (createOnStart)
            CreateSphere();
    }
    public void CreateSphere()
    {
        RemoveChilds();

        GameObject newSphere = new GameObject();
        Transform newSphereTransform = newSphere.transform;
        newSphereTransform.parent = transform;

        Transform planeHolder;

        Vector2 planeSize = Vector2.one * radius * 2;

        // Side planes
        planeHolder = InstantiatePlane(Vector3.forward, Vector3.up, Vector3.right * radius, planeSize);
        planeHolder.parent = newSphereTransform;

        planeHolder = InstantiatePlane(Vector3.left, Vector3.up, Vector3.forward * radius, planeSize);
        planeHolder.parent = newSphereTransform;

        planeHolder = InstantiatePlane(Vector3.back, Vector3.up, Vector3.left * radius, planeSize);
        planeHolder.parent = newSphereTransform;
        
        planeHolder = InstantiatePlane(Vector3.right, Vector3.up, Vector3.back * radius, planeSize);
        planeHolder.parent = newSphereTransform;

        // Top Plate
        planeHolder = InstantiatePlane(Vector3.right, Vector3.forward, Vector3.up * radius, planeSize);
        planeHolder.parent = newSphereTransform;

        // Bottom Plate
        planeHolder = InstantiatePlane(Vector3.left, Vector3.forward, Vector3.down * radius, planeSize);
        planeHolder.parent = newSphereTransform;

    }
    Transform InstantiatePlane(Vector3 xVector, Vector3 yVector, Vector3 center, Vector2 size)
    {
        Mesh newMesh = BuildPlaneMesh(xVector, yVector, center, size);
        GameObject plane = new GameObject();
        MeshFilter meshFilter = plane.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = plane.AddComponent<MeshRenderer>();

        meshFilter.mesh = newMesh;
        meshRenderer.material = meshMaterial;

        return plane.transform;
    }
    Mesh BuildPlaneMesh(Vector3 xVector, Vector3 yVector, Vector3 center, Vector2 size)
    {
        xVector = xVector.normalized;
        yVector = yVector.normalized;

        Mesh newMesh = new Mesh();
        List<Vector3> points = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();


        Vector3 basePoint = center - xVector * size.x / 2 - yVector * size.y / 2;


        for (int x = 0; x < pointsPerAxis; ++x)
            for (int y = 0; y < pointsPerAxis; ++y)
            {
                Vector3 newPoint = basePoint;

                newPoint += x * size.x / (pointsPerAxis - 1) * xVector;
                newPoint += y * size.y / (pointsPerAxis - 1) * yVector;
                Vector3 spherePoint = newPoint.normalized * radius;
                points.Add(Vector3.Lerp(newPoint, spherePoint + spherePoint * 0.3f * Noise.GeneratePoint(0, 2, spherePoint + Vector3.one * 5000), interpolationValue));

                normals.Add(newPoint.normalized);

                if (x == 0 || y == 0)
                    continue;

                if (fillFirstTriangle)
                {
                    if (fillBackwardTriangle)
                    {
                        triangles.Add(pointsPerAxis * y + x - 1);
                        triangles.Add(pointsPerAxis * y + x);
                        triangles.Add(pointsPerAxis * (y - 1) + x - 1);
                    }

                    triangles.Add(pointsPerAxis * (y - 1) + x - 1);
                    triangles.Add(pointsPerAxis * y + x);
                    triangles.Add(pointsPerAxis * y + x - 1);
                }

                if (fillSecondTriangle)
                {
                    if (fillBackwardTriangle)
                    {
                        triangles.Add(pointsPerAxis * y + x);
                        triangles.Add(pointsPerAxis * (y - 1) + x);
                        triangles.Add(pointsPerAxis * (y - 1) + x - 1);
                    }

                    triangles.Add(pointsPerAxis * (y - 1) + x - 1);
                    triangles.Add(pointsPerAxis * (y - 1) + x);
                    triangles.Add(pointsPerAxis * y + x);

                }
            }

        newMesh.vertices = points.ToArray();
        newMesh.triangles = triangles.ToArray();
        newMesh.normals = normals.ToArray();

        return newMesh;
    }

    void RemoveChilds()
    {
        for (int i = 0; i < transform.childCount; ++i)
            Destroy(transform.GetChild(i).gameObject);
    }
}
