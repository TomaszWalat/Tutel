using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshCollider))]
[RequireComponent (typeof(MeshRenderer))]

public class TileMapGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BuildMesh();
    }

    void BuildMesh()
    {
        //generate the actual mesh
        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[2 * 3];
        Vector3[] normals = new Vector3[4];

        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(1, 0, 0);
        vertices[2] = new Vector3(0, 0, -1);
        vertices[3] = new Vector3(1, 0, -1);

        triangles[0] = 0;
        triangles[1] = 3;
        triangles[2] = 2;

        triangles[3] = 0;
        triangles[4] = 1;
        triangles[5] = 3;

        normals[0] = Vector3.up;
        normals[1] = Vector3.up;
        normals[2] = Vector3.up;
        normals[3] = Vector3.up;


        //creating the actual mesh
        Mesh tilemapBase = new Mesh();
        tilemapBase.vertices = vertices;
        tilemapBase.triangles = triangles;
        tilemapBase.normals = normals;

        //sending data where it's due
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        tilemapBase.name = "Base Grid";
        meshFilter.mesh = tilemapBase;
    }

}
