using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    [SerializeField]public Material material;
    [SerializeField] PentagonalDodecahedronGenerator pentagonalDodecahedronGenerator;


    [Header("Sizes")]
    public int faces = 4;
    public int floors = 3;
    public float cylinderHeight = 2f;
    public float thiccness = 1f;
    public float leaveSize = 1f;
    [Range(0.9f,1f)]
    public float reductionRate = 1f;

    [Header("Random")]
    public float randomness = 1f;
    public float randomnessScale = 1f;
    public float randomnessX = 1f;
    public float randomnessY = 1f;
    public float randomnessZ = 1f;

    [Header("Root")]
    public float rootChance = 1f;
    public float rootThiccness = 1f;

    [Header("Idk yet")]
    public float angularOffset = 2f;
    public float growDir = 1f;
    [SerializeField]public Gradient gradient;


    // Start is called before the first frame update
    void Start()
    {
    }

    void AddVertex()
    {

    }

    void AddTriangle()
    {

    }

    Vector3 GetRandomVec3()
    {
        Vector3 random = new Vector3(Random.Range(-randomnessX, randomnessX), Random.Range(-randomnessY, randomnessY), Random.Range(-randomnessZ, randomnessZ));

        return random / randomnessScale;
    }

    public void GenerateTree(GameObject tree, Vector3 startPos, Quaternion startRot)
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
            if ((Random.Range(0f, 1f) * 100) < rootChance)
            {
                Vector3 thickness1 = pos * rootThiccness;
                if(j < faces - 1)
                {
                    vertices[j] = thickness1;
                    vertices[j + 1] = thickness1;
                    plusJ = true;
                }

            }
            else
            {
                vertices[j] = (pos);
            }
            uvs[j] = new Vector2(j * angularStep, uvs[j].y);
            if (plusJ)
            {
                j++;
            }
        }


        tree.transform.position = startPos;
        tree.transform.rotation = startRot;

        Mesh mesh = GenerateLayer(tree, vertices, triangles, uvs);
        tree.GetComponent<MeshFilter>().mesh = mesh;

        tree.GetComponent<MeshRenderer>().sharedMaterial = material;
    }

    Mesh GenerateInsideMesh()
    {
        Mesh mesh = new Mesh();

        return mesh;
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
        float _thiccness = thiccness;
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[faces * floors];
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

        float currentReduction = _thiccness;

        Vector3 lastPivot = startingPos;
        for (int i = 1; i < floors; i++)
        {
            Vector3 pivot = lastPivot + cylinderHeight * Vector3.up;
            lastPivot = pivot;
            for (int j = 0; j < faces; j++)
            {
                float x = Mathf.Cos(j * angularStep + angularOffset);
                float z = Mathf.Sin(j * angularStep + angularOffset);

                Vector3 pos = new Vector3(x, 0f, z) + GetRandomVec3();
                pos *= _thiccness;
                vertices[i * faces + j] = pos + pivot;
                uvs[i * faces + j] = new Vector2(j * angularStep, vertices[i * faces + j].y);

                triangles[6 * ((i - 1) * faces + j)] = (i - 1) * faces + j;
                triangles[6 * ((i - 1) * faces + j) + 1] = (i) * faces + j;
                triangles[6 * ((i - 1) * faces + j) + 2] = (i - 1) * faces + (j + 1) % faces;
                triangles[6 * ((i - 1) * faces + j) + 3] = (i - 1) * faces + (j + 1) % faces;
                triangles[6 * ((i - 1) * faces + j) + 4] = (i) * faces + j;
                triangles[6 * ((i - 1) * faces + j) + 5] = (i) * faces + (j + 1) % faces;

            }
            currentReduction *= reductionRate;
            _thiccness = currentReduction;
            //if(i == floors-1)
            //{
            //    GameObject PentagonalDodecahedron = new GameObject("PentagonalDodecahedron");
            //    pentagonalDodecahedronGenerator.radius = currentReduction * leaveSize;
            //    pentagonalDodecahedronGenerator.GeneratePentagonalDodecahedron(PentagonalDodecahedron, _tree, pivot);
            //}
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        return mesh;
    }

    Mesh GeneratePineLeaves()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[faces + 2];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[faces * 3];

        vertices[0] = new Vector3();

        for (int i = 0; i < vertices.Length; i++)
        {
            for (int j = 0; j < faces; j++)
            {
                triangles[3 * ((i - 1) * faces + j)] = 0;
                triangles[3 * ((i - 1) * faces + j) + 1] = 0;//change this number
                triangles[3 * ((i - 1) * faces + j) + 2] = 0;//change this number
                triangles[3 * ((i - 1) * faces + j) + 3] = 0;
                triangles[3 * ((i - 1) * faces + j) + 4] = 0;//change this number
                triangles[3 * ((i - 1) * faces + j) + 5] = 0;//change this number

            }
        }


        return mesh;
    }


    // Update is called once per frame
    void Update()
    {

    }
}
