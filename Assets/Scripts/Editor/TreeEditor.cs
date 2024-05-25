using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SaveTree))]
public class TreeEditor : Editor
{
    bool buttonPressed = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnInspectorGUI()
    {
        buttonPressed = GUILayout.Button("Save Mesh");
        if (buttonPressed)
        {
            SaveTree saveTree = (SaveTree)target;
            saveTree.SaveAsset();
        }
    }
}
