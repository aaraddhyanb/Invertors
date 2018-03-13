using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    public float wallOffset;
    public float zDist;
    public float snapDistance;
    public float snapOffset;

    private Vector3 rot;
    private Vector3 myPosition;
    private Vector3 unUsedInvertorPosition;
    private string houseTag;
    private bool invertorAttached;

    void Start()
    {
        houseTag = "House";
        invertorAttached = false;
        unUsedInvertorPosition = new Vector3(-11, 98.2f, 30);
    }

    void OnMouseDrag()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag == houseTag)
            {
                rot = hit.collider.transform.rotation.eulerAngles;
                myPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z) + (hit.normal.normalized * wallOffset);

                MeshCollider meshCollider = hit.collider as MeshCollider;
                Mesh mesh = meshCollider.sharedMesh;
                Vector3[] vertices = mesh.vertices;
                int[] triangles = mesh.triangles;

                Vector3[] p = new Vector3[3];
                p[0] = vertices[triangles[hit.triangleIndex * 3 + 0]];
                p[1] = vertices[triangles[hit.triangleIndex * 3 + 1]];
                p[2] = vertices[triangles[hit.triangleIndex * 3 + 2]];
                Transform hitTransform = hit.collider.transform;
                p[0] = hitTransform.TransformPoint(p[0]);
                p[1] = hitTransform.TransformPoint(p[1]);
                p[2] = hitTransform.TransformPoint(p[2]);

                //Skip the longest edge of the triangle
                float dist1 = Vector3.Distance(p[1], p[0]);
                float dist2 = Vector3.Distance(p[2], p[1]);
                float dist3 = Vector3.Distance(p[0], p[2]);
                float maxDist = Mathf.Max(Mathf.Max(dist1, dist2), dist3);

                if (maxDist != dist1)
                    myPosition = SnapIfPossible(p[1], p[0], myPosition);

                if (maxDist != dist2)
                    myPosition = SnapIfPossible(p[2], p[1], myPosition);

                if (maxDist != dist3)
                    myPosition = SnapIfPossible(p[0], p[2], myPosition);

                invertorAttached = true;
            }
        }
        else
        {
            myPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDist));
            invertorAttached = false;
        }

        transform.position = myPosition;
        transform.rotation = Quaternion.Euler(rot);
    }

    void OnMouseUp()
    {
        if (!invertorAttached)
        {
            myPosition = unUsedInvertorPosition;
            transform.position = unUsedInvertorPosition;
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }

    private Vector3 SnapIfPossible(Vector3 point1, Vector3 point2, Vector3 myPosition)
    {
        Vector3 lineDirection = (point1 - point2).normalized;
        Vector3 v = myPosition - point2;
        float d = Vector3.Dot(v, lineDirection);
        Vector3 closestPoint = point2 + lineDirection * d;
   
        float distance = Vector3.Distance(myPosition, closestPoint);
        Vector3 offsetDir = (myPosition - closestPoint).normalized;

        if (distance < snapDistance)
            return closestPoint + (offsetDir * snapOffset);
        else
            return myPosition;
    }
}

//Explanation
//In our case, it turns out that we never want to snap to the longest edge of the hit triangle as not all edges are valid snap edges.
//One exception is the triangular area between the walls of the house and the slopes of the roof. In this case, we have an edge from the wall triangle that it snaps to.
