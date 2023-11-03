using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herb : Plant
{
    public override void Start()
    {
        base.Start();
        type = PlantType.herb;
        data.type = type;

        gameObject.transform.rotation = Quaternion.Euler(-90f, gameObject.transform.rotation.y, gameObject.transform.rotation.z);
    }
}
