using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantData", menuName = "Entities/PlantData")]
public class PlantData : LivingEntityData
{
    public List<PlantMesh> meshes;
    public Material[] materials;
    public float stageIncrement;
}

[System.Serializable]
public class PlantMesh
{
    public List<Mesh> meshes;
}
