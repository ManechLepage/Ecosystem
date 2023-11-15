using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntityData : ScriptableObject
{
    public BellCurve lifespan;
    public bool immortal;
    public string objectName;
    public GameObject prefab;
}
