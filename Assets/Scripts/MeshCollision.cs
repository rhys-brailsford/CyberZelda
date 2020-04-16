using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Collision using MeshCollider
// Not working currently, but being left for prosperity
public class MeshCollision : Collision
{
    private Mesh mesh;

    List<Face> faces;

    private struct Face
    {
        public List<Vector3> points;
        public Vector3 norm;

        public Face(List<Vector3> points, Vector3 normal)
        {
            this.points = points;
            this.norm = normal;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mesh = gameObject.GetComponent<MeshCollider>().sharedMesh;
        faces = new List<Face>();

        List<Face> tris = new List<Face>();

        Matrix4x4 localToWorld = transform.localToWorldMatrix;

        Vector3[] verts = mesh.vertices;
        int[] triIndices = mesh.triangles;


        //foreach (int i in triIndices)
        for (int i=0; i<triIndices.Length; i=i+3)
        {
            Vector3 vert1_l = mesh.vertices[triIndices[i]];
            Vector3 vert2_l = mesh.vertices[triIndices[i+1]];
            Vector3 vert3_l = mesh.vertices[triIndices[i+2]];

            Vector3 vert1_w = localToWorld.MultiplyPoint3x4(vert1_l);
            Vector3 vert2_w = localToWorld.MultiplyPoint3x4(vert2_l);
            Vector3 vert3_w = localToWorld.MultiplyPoint3x4(vert3_l);

            //Vector3 norm = Vector3.Normalize(Vector3.Cross(vert2_w - vert1_w, vert2_w - vert3_w));
            Vector3 norm = Vector3.Normalize(Vector3.Cross(vert2_w - vert1_w, vert3_w - vert1_w));

            List<Vector3> points = new List<Vector3>();
            points.Add(vert1_w);
            points.Add(vert2_w);
            points.Add(vert3_w);

            tris.Add(new Face(points, norm));
        }

        for (int i=0; i<tris.Count; i++)
        {
            for (int j = i + 1; j < tris.Count; j++)
            {
                if (tris[i].norm == tris[j].norm)
                {
                    // We use a hashset to prevent duplicates when adding
                    HashSet<Vector3> pointsHash = new HashSet<Vector3>();
                    foreach (Vector3 point in tris[i].points)
                    {
                        pointsHash.Add(point);
                    }
                    foreach (Vector3 point in tris[j].points)
                    {
                        pointsHash.Add(point);
                    }
                    List<Vector3> pointsList = pointsHash.ToList<Vector3>();
                    faces.Add(new Face(pointsList, tris[i].norm));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override Vector3 Normal(Collider srcCol)
    {
        Vector3 closestCornerOnObj = gameObject.GetComponent<Collider>().ClosestPointOnBounds(srcCol.transform.position);

        // Find which faces are relevant
        List<Face> relevantFaces = new List<Face>();

        int i = 0;
        float minDot = 100;

        foreach (Face curFace in faces)
        {
            float dot = Vector3.Dot(closestCornerOnObj - curFace.points[0], curFace.norm);

            if (Mathf.Abs(dot) < minDot)
            {
                minDot = dot;
            }

            if (Mathf.Abs(dot) < 0.0001f)
            {
                relevantFaces.Add(curFace);
            }
        }

        if (relevantFaces.Count == 1)
        {
            return relevantFaces[0].norm;
        }
        else
        {
            Vector3 result = new Vector3();

            foreach (Face face in relevantFaces)
            {
                result += face.norm;
            }
            result = Vector3.Normalize(result);
            return result;
        }
    }

}
