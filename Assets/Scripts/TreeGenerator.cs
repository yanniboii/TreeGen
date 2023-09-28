using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    [SerializeField] Material material;


    public int faces = 4;
    public int floors = 3;
    public float thiccness = 1f;
    [Header("Random")]
    public float randomness = 1f;
    public float randomnessScale = 1f;
    public float randomnessX = 1f;
    public float randomnessY = 1f;
    public float randomnessZ = 1f;

    [Header("Root Amount")]
    public float rootChance = 1f;

    public float rootThiccness = 1f;
    public float cylinderHeight = 2f;
    public float angularOffset = 2f;

    public bool useFlatShading;
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

    Vector3 GetRandomVec3()
    {
        Vector3 random = new Vector3(Random.Range(-randomnessX,randomnessX), Random.Range(-randomnessY, randomnessY), Random.Range(-randomnessZ, randomnessZ));

        return random / randomnessScale;
    }

    void GenerateTree()
    {
        Vector3[] vertices = new Vector3[faces];
        int[] triangles = new int[faces * 6];
        Vector2[] uvs = new Vector2[vertices.Length];

        float angularStep = 2f * Mathf.PI / (float)(faces);
        for (int j = 0; j < faces; ++j)
        {
            bool plusJ = false;
            Vector3 pos = new Vector3(Mathf.Cos(j * angularStep + angularOffset), 0f, Mathf.Sin(j * angularStep + angularOffset)) + GetRandomVec3();
            pos *= thiccness;
            if((Random.Range(0f,1f)*100) < rootChance)
            {
                Vector3 pos2 = new Vector3(Mathf.Cos(j+1 * angularStep + angularOffset), 0f, Mathf.Sin(j+1 * angularStep + angularOffset)) + GetRandomVec3();
                Vector3 thickness1 = pos * rootThiccness;
                Vector3 thickness2 = pos2 * rootThiccness;

                vertices[j] = thickness1;
                vertices[j+1] = thickness2;
                plusJ = true;
            }
            else
            {
                vertices[j] = (pos);
            }
            uvs[j] = new Vector2(j * angularStep, uvs[j].y);
            if(plusJ)
            {
                j++;
            }
        }


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

        Vector3[] vertices = new Vector3[faces*floors];
        Vector2[] uvs = new Vector2[(vertices.Length)];

        for (int i = 0; i < _vertices.Length; i++)
        {
            vertices[i] = _vertices[i];
            uvs[i] = _uvs[i];
        }

        int[] triangles = new int[6 * (vertices.Length - faces)];

        Vector3 startingPos = new Vector3();

        float angularStep = 2f * Mathf.PI / (float)(faces);
        Vector3 first = new Vector3(Mathf.Cos(angularOffset), 0f, Mathf.Sin(angularOffset));
        first = ChangeCoordinates(first, Vector3.up, Vector3.up);
        first += startingPos;

        Vector3 lastPivot =startingPos;
        for (int i = 1; i < vertices.Length / faces; i++)
        { 
            Vector3 pivot = lastPivot + cylinderHeight * Vector3.up;
            lastPivot = pivot;
            for (int j = 0; j < faces; j++)
            {
                float x = Mathf.Cos(j * angularStep + angularOffset);
                float z = Mathf.Sin(j * angularStep + angularOffset);

                Vector3 pos = new Vector3(x, 0f, z) + GetRandomVec3();
                pos *= thiccness;
                vertices[i * faces + j] = pos+pivot;
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
        if (useFlatShading)
        {
            Vector3[] flatShadedVertices = new Vector3[triangles.Length];
            Vector2[] flatShadedUvs = new Vector2[triangles.Length];

            for (int i = 0; i < triangles.Length; i++)
            {
                flatShadedVertices[i] = vertices[triangles[i]];
                flatShadedUvs[i] = uvs[triangles[i]];
                triangles[i] = i;
            }

            vertices = flatShadedVertices;
            uvs = flatShadedUvs;
            mesh.RecalculateNormals();
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        return mesh;
    }

    void FlatShading(int[] triangles, Vector3[] vertices, Vector2[] uvs)
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}