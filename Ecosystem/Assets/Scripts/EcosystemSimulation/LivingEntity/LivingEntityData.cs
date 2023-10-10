using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntityData : ScriptableObject
{
    public BellCurve lifespan;
    public string objectName;
    public System.Enum type;
}
