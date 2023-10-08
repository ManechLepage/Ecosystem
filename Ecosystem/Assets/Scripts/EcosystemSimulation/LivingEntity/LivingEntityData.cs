using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntityData : ScriptableObject
{
    public BellCurve lifespan;
    public string objectName;
    public List<float> growth_sizes;
    public List<Mesh> growth_stages;
    public System.Enum type;
}
