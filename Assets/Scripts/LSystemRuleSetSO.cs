using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "L-SystemRuleSetSO", menuName = "Scriptable Objects/L-SystemRuleSetSO")]
public class LSystemRuleSetSO : ScriptableObject
{
    public string axiom;
    public int recursion = 1;
    public float _angle = 5;
    public int treeAmount;
    public bool useThiccness;
    public List<int> lIndex = new List<int>();
    public GameObject leaveObject;

    public List<char> letter;
    public List<string> rule;
}
