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
}

[System.Serializable]
public class LSystems : MonoBehaviour
{
    [SerializeField] public string axiom;
    [SerializeField] public int recursion = 1;
    [SerializeField] float angle = 20;
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
        for(int offsetX = 0; offsetX < 1250; offsetX+= 50)
        {
            for(int offsetY = 0;  offsetY < 1250; offsetY+= 50)
            { 
                Generate(new Vector3(offsetX, 0, offsetY));
            }

        }
    }
    void Generate(Vector3 offset)
    {
        currentString = axiom;

        transform.position = offset;
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
        foreach (char c in currentString)
        {
            switch (c)
            {

                case 'A':
                    break;
                case 'F':
                    Vector3 initialPos = transform.position;
                    Quaternion initialRotation = transform.rotation;
                    transform.Translate(Vector3.up * ((treeGenerator.cylinderHeight * treeGenerator.floors) - treeGenerator.cylinderHeight));
                    GameObject tree = new GameObject("tree");
                    treeGenerator.GenerateTree(tree, initialPos, initialRotation);
                    break;
                case 'L':
                    Vector3 initialPoss = transform.position;
                    transform.Translate(Vector3.up * ((treeGenerator.cylinderHeight * treeGenerator.floors) - treeGenerator.cylinderHeight));
                    GameObject leave = new GameObject("leave");
                    pentagonalDodecahedronGenerator.GeneratePentagonalDodecahedron(leave, initialPoss);
                    break;
                case '[':
                    transformStack.Push(new TransformInfo()
                    {
                        position = transform.position,
                        rotation= transform.rotation
                    });
                    break;
                case ']':
                    TransformInfo ti = transformStack.Pop();
                    transform.position = ti.position;
                    transform.rotation = ti.rotation;
                    break;
                case '>':
                    transform.Rotate(Vector3.right * Random.Range(-angle,angle));
                    break;
                case '<':
                    transform.Rotate(Vector3.left* Random.Range(-angle, angle));
                    break;
                case '+':
                    transform.Rotate(Vector3.forward * Random.Range(-angle, angle));
                    break;
                case '-':
                    transform.Rotate(Vector3.forward * Random.Range(-angle, angle));
                    break;

            }
        }
    }
}
