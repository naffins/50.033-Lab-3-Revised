using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericParticleController : MonoBehaviour
{
    void Start()
    {
        GameObject.Destroy(gameObject,GetDespawnTime());
    }

    protected virtual float GetDespawnTime() {
        return 10F;
    }
}
