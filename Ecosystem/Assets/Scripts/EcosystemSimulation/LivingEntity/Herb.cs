using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herb : Plant
{
    public override void Start()
    {
        gameObject.transform.rotation = Quaternion.Euler(-90f, gameObject.transform.rotation.y, gameObject.transform.rotation.z);
        //gameObject.transform.localScale = new Vector3(650f, 650f, 650f);
        
        base.Start();
    }
}
