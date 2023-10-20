using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OakTree : Plant
{
    public override void Start()
    {
        base.Start();

        float m = 350f;
        gameObject.transform.localScale *= m;
        m = gameObject.transform.localScale.x;

        // Ne fonctionne pas (tentative de mettre les arbres sur le sol)
        // print($"{gameObject.GetComponent<MeshRenderer>().bounds.size.y / 2f * (m / 1000f)} avec m={m}");
        // Le problème est que (probablement) l'origine des prefabs des arbres n'est pas au sol...
        gameObject.transform.localScale = new Vector3( m / 2f, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
    }
}
