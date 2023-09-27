using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    [SerializeField] Material material;


    public int faces = 4;
    // Start is called before the first frame update
    void Start()
    {
        GenerateTree();
    }

    void AddVertex()
    {

    }

    void AddTriangle()
    {

    }

    void GenerateTree()
    {
        Vector3[] vertices = new Vector3[faces * 2];
        int[] triangles = new int[faces * 6];
        Vector2[] uvs = new Vector2[vertices.Length];


        GameObject tree = new GameObject("tree");

        tree.transform.position = new Vector3(0, 5, 0);

        Mesh mesh = GenerateLayer(tree, vertices, triangles, uvs);
        MeshRenderer quadRenderer = tree.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = tree.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        quadRenderer.sharedMaterial = material;

    }

    Vector3 ChangeCoordinates(Vector3 input, Vector3 inputNormal, Vector3 newNormal)
    {
        float angle = Vector3.Angle(inputNormal, newNormal);
        Vector3 axis = Vector3.Cross(inputNormal, newNormal);
        Quaternion rot = Quaternion.AngleAxis(angle, axis);
        return rot * input;
    }

    Mesh GenerateLayer(GameObject _tree, Vector3[] _vertices, int[] _triangles, Vector2[] _uvs)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[faces*2];
        Vector2[] uvs = new Vector2[(vertices.Length)];

        for (int i = 0; i < _vertices.Length; i++)
        {
            vertices[i] = _vertices[i];
            uvs[i] = _uvs[i];
        }

        int[] triangles = new int[6 * (vertices.Length - faces)];

        Vector3 startingPos = new Vector3();
        float angularOffset = 2f;


        float angularStep = 2f * Mathf.PI / (float)(faces);
        Vector3 first = new Vector3(Mathf.Cos(angularOffset), 0f, Mathf.Sin(angularOffset));
        first = ChangeCoordinates(first, Vector3.up, Vector3.up);
        first += startingPos;

        Vector3 lastPivot =startingPos;
        for (int i = 1; i < vertices.Length / faces; i++)
        {
            Vector3 pivot = lastPivot + Vector3.up * 2f;
            float yPos = (float)i / (vertices.Length / faces - 1) * 2f;

            for (int j = 0; j < faces; j++)
            {
                float x = Mathf.Cos(j * angularStep + angularOffset);
                float z = Mathf.Sin(j * angularStep + angularOffset);

                Vector3 pos = new Vector3(x, 0f, z);
                Vector3 pos2 = new Vector3(x,yPos, z);
                vertices[i * faces + j] = pos;
                vertices[i*faces] = pos2;
                Debug.Log("ASD");
                uvs[i * faces + j] = new Vector2(j * angularStep, vertices[i * faces + j].y);

                triangles[6 * ((i - 1) * faces + j)]     = (i - 1) * faces + j;
                triangles[6 * ((i - 1) * faces + j) + 1] = (i) * faces + j;
                triangles[6 * ((i - 1) * faces + j) + 2] = (i - 1) * faces + (j + 1) % faces;
                triangles[6 * ((i - 1) * faces + j) + 3] = (i - 1) * faces + (j + 1) % faces;
                triangles[6 * ((i - 1) * faces + j) + 4] = (i) * faces + j;
                triangles[6 * ((i - 1) * faces + j) + 5] = (i) * faces + (j + 1) % faces;

            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        return mesh;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
