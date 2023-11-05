using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantData", menuName = "Entities/PlantData")]
public class PlantData : LivingEntityData
{
    public List<PlantMesh> meshes;
    public float stageIncrement;
    public Vector2 minMaxSizeRange;
    public float nutritionPerStage;
    public int minStageToBeEaten; // >=
    public int nbrOfUpdatesToEat;
}

[System.Serializable]
public class PlantMesh
{
    public float scale = 1f;
    public bool isCenterAnchored = false;
    public List<Mesh> meshes;
    public Material[] materials;
}
