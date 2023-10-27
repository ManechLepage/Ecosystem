using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantData", menuName = "Entities/PlantData")]
public class PlantData : LivingEntityData
{
    public List<PlantMesh> meshes;
    public Material[] materials;
    public float stageIncrement;
    public Vector2 minMaxSizeRange;
}

[System.Serializable]
public class PlantMesh
{
    public float scale = 1f;
    public List<Mesh> meshes;
}
