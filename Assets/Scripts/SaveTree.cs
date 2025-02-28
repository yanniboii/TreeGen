using System;
using UnityEditor;
using UnityEngine;

public class SaveTree : MonoBehaviour
{
    string path = "Assets/Prefab/Trees/Tree";
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SaveAsset()
    {
        Mesh mesh = transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh;
        AssetDatabase.CreateAsset(mesh, path + DateTime.Now.ToString().Replace("/", "_").Replace(":", "-") + ".asset");
        AssetDatabase.SaveAssets();
    }
}
