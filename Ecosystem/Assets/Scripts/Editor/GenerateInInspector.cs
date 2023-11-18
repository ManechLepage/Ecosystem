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
            generator.Initialize();
            generator.DeleteTerrain();
            generator.Initialize();
            generator.GenerateEcosystem();

            string pop_text = "Populations :\n";
            foreach (KeyValuePair<System.Enum, int> population in generator.populations)
                pop_text += "   " + population.Key.ToString() + " : " + population.Value.ToString() + "\n";
            Debug.Log(pop_text);
        }

        if (GUILayout.Button("Delete Terrain"))
        {
            generator.DeleteTerrain();
        }

        if (GUILayout.Button("Initialize Variables"))
        {
            generator.Initialize();
        }

        if (GUILayout.Button("Update Simulation"))
        {
            generator.SimulationUpdate();
        }
    }
}
