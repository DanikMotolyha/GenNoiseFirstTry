using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (NoiseGenerator))]
public class MapGenerateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NoiseGenerator mapGen = (NoiseGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.GenerateMap();
            }
        }
        if (GUILayout.Button("Gen"))
        {
            mapGen.GenerateMap();
        }
    }
}
