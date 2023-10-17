using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herb : Plant
{
    public override void Start()
    {
        base.Start();

        gameObject.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        gameObject.transform.localScale = new Vector3(150f, 150f, 150f);
    }
}
