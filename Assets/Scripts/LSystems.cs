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

    private Stack<TransformInfo> transformStack;
    [SerializeField] public List<char> letter;
    [SerializeField] public List<string> rule;
    Dictionary<char, string> rules;
    private string currentString = string.Empty;

    TreeGenerator treeGenerator;
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
        Generate();
    }

    void Generate()
    {
        currentString = axiom;

        StringBuilder stringBuilder = new StringBuilder();

        for (int i = 0; i < recursion; i++)
        {

            foreach (char c in currentString)
            {
                stringBuilder.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
            }
        }
        currentString = stringBuilder.ToString();
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
                case 'X':
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
                    transform.Rotate(Vector3.right * 30);
                    break;
                case '<':
                    transform.Rotate(Vector3.left*30);
                    break;
                case '+':
                    transform.Rotate(Vector3.up * 30);
                    break;
                case '-':
                    transform.Rotate(Vector3.down * 30);
                    break;
                case '^':
                    transform.Rotate(Vector3.forward * 30);
                    break;
                case '~':
                    transform.Rotate(Vector3.back * 30);
                    break;
            }
        }
    }
}
