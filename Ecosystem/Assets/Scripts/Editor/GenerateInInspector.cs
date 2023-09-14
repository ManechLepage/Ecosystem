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
            generator.Start();
            generator.DeleteTerrain();
            generator.GenerateTerrain();
            generator.PopulateTerrain();

            string pop_text = "Populations :\n";
            foreach (KeyValuePair<System.Type, int> population in generator.populations)
            {
                pop_text += "   " + population.Key.ToString() + " : " + population.Value.ToString() + "\n";
            }

            Debug.Log(pop_text);
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
