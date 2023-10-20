using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OakTree : Plant
{
    public override void Start()
    {
        base.Start();

        float m = 13f;
        gameObject.transform.localScale *= m;

        // Ne fonctionne pas (tentative de mettre les arbres sur le sol)
        // print($"{gameObject.GetComponent<MeshRenderer>().bounds.size.y / 2f * (m / 1000f)} avec m={m}");
        // Le problème est que (probablement) l'origine des prefabs des arbres n'est pas au sol...
        gameObject.transform.position += new Vector3(
            0f,
            gameObject.GetComponent<MeshRenderer>().bounds.size.y / 2f,
            0f);
    }
}
