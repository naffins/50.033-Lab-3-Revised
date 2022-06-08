using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxComponentController : MonoBehaviour
{

    public GameObject mainCamera;

    public Material[] parallaxMaterials;
    public float[] parallaxSpeeds;

    private float initialY;

    void Start()
    {
        initialY = mainCamera.transform.position.y;
    }

    void Update()
    {
        float offset = mainCamera.transform.position.y - initialY;
        Vector2 offsetVect = new Vector2(0F,offset);
        for (int i=0;i<parallaxMaterials.Length;i++) {
            parallaxMaterials[i].mainTextureOffset = offsetVect * parallaxSpeeds[i];
        }
    }
}
