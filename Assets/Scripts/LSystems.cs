using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class TransformInfo
{
    public Vector3 position;
    public Quaternion rotation;
    public List<Vector3> startVertices = new List<Vector3>();
    public Vector3 growDir;
    public Vector3 pivot;
    public Vector3 lastPivot;
    public Vector3 growth;
}

[System.Serializable]
public class LSystems : MonoBehaviour
{
    [SerializeField] public GameObject gameObject;
    [SerializeField] public string axiom;
    [SerializeField] public int recursion = 1;
    [SerializeField] float _angle = 5;
    [SerializeField] int treeAmount;

    private Stack<TransformInfo> transformStack;
    [SerializeField] public List<char> letter;
    [SerializeField] public List<string> rule;
    Dictionary<char, string> rules;
    private string currentString = string.Empty;

    TreeGenerator treeGenerator;
    PentagonalDodecahedronGenerator pentagonalDodecahedronGenerator;
    // Start is called before the first frame update
    void Start()
    {
        transformStack = new Stack<TransformInfo>();
        if (letter.Count != rule.Count)
        {
            Debug.LogError("The 'letter' and 'rule' lists must have the same length.");
            return;
        }
        rules = letter.Zip(rule, (l, r) => new { Letter = l, Rule = r })
            .ToDictionary(item => item.Letter, item => item.Rule);

        treeGenerator = FindObjectOfType<TreeGenerator>();
        pentagonalDodecahedronGenerator = FindObjectOfType<PentagonalDodecahedronGenerator>();
        spawntrees();
    }

    void spawntrees()
    {
        for (int offsetX = 0; offsetX < 750; offsetX += 50)
        {
            for (int offsetY = 0; offsetY < 750; offsetY += 50)
            {
                Generate(new Vector3(offsetX, 0, offsetY));
            }

        }
    }
    void Generate(Vector3 offset)
    {
        float angle = _angle;
        currentString = axiom;
        GameObject tree = new GameObject("tree");
        tree.transform.position = offset;
        tree.AddComponent<MeshRenderer>();
        tree.AddComponent<MeshFilter>();
        transform.position = Vector3.zero;
        transform.rotation = new Quaternion(0, 0, 0, 0);

        for (int i = 0; i < recursion; i++)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in currentString)
            {
                if (rules.ContainsKey(c))
                {
                    stringBuilder.Append(rules[c]);
                }
                else
                {
                    stringBuilder.Append(c);
                }
            }
            currentString = stringBuilder.ToString();
        }
        Debug.Log(currentString);

        // Create an array of CombineInstance with the appropriate size
        List<CombineInstance> branchCombine = new List<CombineInstance>();
        List<CombineInstance> leafCombine = new List<CombineInstance>();
        List<GameObject> oldGameObjects = new List<GameObject>();

        Color randomTreeColor = treeGenerator.gradient.Evaluate(Random.Range(0f, 1f));
        bool firstbranch = true;
        Vector3 growDirection = Vector3.up;
        Vector3 pivot = Vector3.zero;
        Vector3 lastPivot = Vector3.zero;
        Vector3 growth = Vector3.zero;
        Vector3[] _vertices = new Vector3[treeGenerator.faces];

        foreach (char c in currentString)
        {
            switch (c)
            {
                case 'A':
                    break;
                case 'F':
                    lastPivot = pivot;
                    Vector3 initialPos = transform.position;
                    Quaternion initialRotation = transform.rotation;
                    transform.Translate(Vector3.up * ((treeGenerator.cylinderHeight * treeGenerator.floors) - treeGenerator.cylinderHeight));
                    GameObject branch = new GameObject("Branch");
                    branch.AddComponent<MeshFilter>();
                    branch.AddComponent<MeshRenderer>();
                    Vector3 growDir = treeGenerator.getRandomVectorInCone(angle, growDirection);
                    if (!firstbranch)
                    {if(transformStack.Count() != 0)
                        {
                            Vector3[] verts = transformStack.Peek().startVertices.ToArray();
                            if (verts != null)
                            {
                                treeGenerator.GenerateTree(branch, transformStack.Peek().pivot, initialRotation, growDir, angle, verts, out growDirection, out pivot);
                                Debug.Log(transformStack.Peek().pivot);
                                //GameObject cube = Instantiate(gameObject, transformStack.Peek().pivot, Quaternion.identity, gameObject.transform);
                                //cube.name = "cube";
                                Debug.DrawRay(pivot, growDirection);
                            }
                        }

                    }
                    else
                    {
                        treeGenerator.GenerateTree(branch, initialPos, initialRotation, growDir, angle, out growDirection, out pivot);
                        Debug.DrawRay(initialPos, growDirection);
                        firstbranch = false;
                    }
                    angle += 1;
                    pivot += lastPivot;
                    growth = pivot - lastPivot;
                    CombineInstance branchInstance = new CombineInstance();
                    
                        Vector3[] vertices = branch.GetComponent<MeshFilter>().sharedMesh.vertices;

                    // Add vertices to the startVertices list
                    for (int i = 0; i < treeGenerator.faces; i++)
                    {
                        int index = vertices.Length - (treeGenerator.faces - i);
                        if (index >= 0 && index < vertices.Length)
                        {

                            _vertices[i] = (vertices[index] - growth);
                        }
                    }
                    branchInstance.mesh = branch.GetComponent<MeshFilter>().sharedMesh;
                    branchInstance.transform = branch.GetComponent<MeshFilter>().transform.localToWorldMatrix;

                    branchCombine.Add(branchInstance);
                    oldGameObjects.Add(branch);
                    break;
                case 'L':
                    Vector3 initialPoss = transform.position;
                    transform.Translate(Vector3.up * ((treeGenerator.cylinderHeight * treeGenerator.floors) - treeGenerator.cylinderHeight));
                    GameObject leave = new GameObject("Leaf");
                    leave.AddComponent<MeshFilter>();
                    leave.AddComponent<MeshRenderer>();
                    MeshRenderer leafRenderer = leave.GetComponent<MeshRenderer>();
                    
                    // Set the material for leaves
                    leafRenderer.sharedMaterial = pentagonalDodecahedronGenerator.material; // Replace with your leaf material

                    pentagonalDodecahedronGenerator.GeneratePentagonalDodecahedron(leave, transformStack.Peek().pivot);

                    CombineInstance leafInstance = new CombineInstance();
                    leafInstance.mesh = leave.GetComponent<MeshFilter>().sharedMesh;
                    leafInstance.transform = leave.GetComponent<MeshFilter>().transform.localToWorldMatrix;

                    leafCombine.Add(leafInstance);
                    oldGameObjects.Add(leave);

                    break;
                case '[':

                    transformStack.Push(new TransformInfo()
                    {
                        position = transform.position,
                        rotation = transform.rotation,
                        growDir = growDirection,
                        pivot = pivot,
                        lastPivot = lastPivot,
                        growth = growth,
                        startVertices = _vertices.ToList()
                    }) ;
                    

                    break;
                case ']':
                    TransformInfo ti = transformStack.Pop();
                    transform.position = ti.position;
                    transform.rotation = ti.rotation;
                    growDirection = ti.growDir;
                    pivot = ti.pivot;
                    lastPivot = ti.lastPivot;
                    growth = ti.growth;
                    _vertices = ti.startVertices.ToArray();

                    break;
                case '>':
                    transform.Rotate(Vector3.right * Random.Range(-angle, angle));
                    break;
                case '<':
                    transform.Rotate(Vector3.left * Random.Range(-angle, angle));
                    break;
                case '+':
                    transform.Rotate(Vector3.forward * Random.Range(-angle, angle));
                    break;
                case '-':
                    transform.Rotate(Vector3.forward * Random.Range(-angle, angle));
                    break;
            }

        }
        foreach (var oldGameObject in oldGameObjects)
        {
            Destroy(oldGameObject);
        }

        // Create separate meshes for branches and leaves
        var branchMesh = new Mesh();
        branchMesh.CombineMeshes(branchCombine.ToArray());

        var leafMesh = new Mesh();
        leafMesh.CombineMeshes(leafCombine.ToArray());

        // Assign the branch and leaf meshes to the tree's MeshFilter
        tree.GetComponent<MeshFilter>().sharedMesh = branchMesh;

        // Set the material for branches
        tree.GetComponent<MeshRenderer>().sharedMaterial = treeGenerator.material;
        tree.GetComponent<MeshRenderer>().material.SetColor("_Color", randomTreeColor);
        tree.GetComponent<MeshRenderer>().material.SetVector("_Vector2", new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)));
        float randomFloat = Random.Range(0f, 1f);
        float randomFloat2 = Random.Range(0f, 1f);

        Debug.Log(randomFloat);
        tree.GetComponent<MeshRenderer>().material.SetFloat("_Float", randomFloat);
        //tree.GetComponent<MeshFilter>().sharedMesh = MeshSmoothener.SmoothMesh(tree.GetComponent<MeshFilter>().sharedMesh, 1, MeshSmoothener.Filter.Laplacian);
        // Optionally, you can create a separate GameObject for leaves if needed
        GameObject leavesObject = new GameObject("Leaves");
        leavesObject.transform.position = offset;

        MeshFilter leavesMeshFilter = leavesObject.AddComponent<MeshFilter>();
        leavesMeshFilter.sharedMesh = leafMesh;
        MeshRenderer leavesMeshRenderer = leavesObject.AddComponent<MeshRenderer>();

        // Set the material for leaves
        leavesMeshRenderer.sharedMaterial = pentagonalDodecahedronGenerator.material; // Replace with your leaf material
        leavesMeshRenderer.material.SetFloat("_Float", randomFloat2);
    }
}
