using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

[CustomEditor(typeof(TreeGenerator))]
public class TreeGeneratorEditor : Editor
{
    private string[] tabs = { "Values", "L-System rules" };

    private int tabIndex = 0;



    private void OnEnable()
    {


    }
    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();
        tabIndex = GUILayout.Toolbar(tabIndex,tabs);
        EditorGUILayout.EndVertical();
        switch (tabIndex)
        {
            case 0:
                Values();
                break; 
            case 1:
                LSystemRules();
                break; 
            default: 
                break;
        }
    }

    private void Values()
    {
        TreeGenerator treeGenerator = (TreeGenerator)target;
        treeGenerator.faces = EditorGUILayout.IntField("Faces", treeGenerator.faces);
        treeGenerator.floors = EditorGUILayout.IntField("Floors", treeGenerator.floors);
        treeGenerator.cylinderHeight = EditorGUILayout.FloatField("cylinderHeight", treeGenerator.cylinderHeight);
        treeGenerator.thiccness = EditorGUILayout.FloatField("thiccness", treeGenerator.thiccness);
        treeGenerator.leaveSize = EditorGUILayout.FloatField("leaveSize", treeGenerator.leaveSize);
        treeGenerator.reductionRate = EditorGUILayout.Slider(treeGenerator.reductionRate, 0.9f, 1.0f);

        treeGenerator.randomness = EditorGUILayout.FloatField("randomness", treeGenerator.randomness);
        treeGenerator.randomnessScale = EditorGUILayout.FloatField("randomnessScale", treeGenerator.randomnessScale);
        treeGenerator.randomnessX = EditorGUILayout.FloatField("randomnessX", treeGenerator.randomnessX);
        treeGenerator.randomnessY = EditorGUILayout.FloatField("randomnessY", treeGenerator.randomnessY);
        treeGenerator.randomnessZ = EditorGUILayout.FloatField("randomnessZ", treeGenerator.randomnessZ);

        treeGenerator.rootChance = EditorGUILayout.FloatField("rootChance", treeGenerator.rootChance);
        treeGenerator.rootThiccness = EditorGUILayout.FloatField("rootThiccness", treeGenerator.rootThiccness);

        treeGenerator.angularOffset = EditorGUILayout.FloatField("angularOffset", treeGenerator.angularOffset);
        treeGenerator.growDir = EditorGUILayout.FloatField("growDir", treeGenerator.growDir);
        treeGenerator.gradient = EditorGUILayout.GradientField("Gradient", treeGenerator.gradient);


    }
    private void LSystemRules()
    {
        LSystems lSystems = target.GetComponent<LSystems>();

        lSystems.axiom = EditorGUILayout.TextField("Axiom", lSystems.axiom);
        lSystems.recursion = EditorGUILayout.IntField("Recursion", lSystems.recursion);

    }
}
