using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SimulationManager))]
public class GenerateInInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SimulationManager generator = (SimulationManager)target;
        
        if (GUILayout.Button("Generate Terrain"))
        {
            generator.DeleteTerrain();
            generator.Start();
            generator.GenerateTerrain();
        }

        if (GUILayout.Button("Delete Terrain"))
        {
            generator.DeleteTerrain();
        }

        if (GUILayout.Button("Initialize Variables"))
        {
            generator.Start();
        }
    }
}
