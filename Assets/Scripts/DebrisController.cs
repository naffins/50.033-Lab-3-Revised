using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisController : MonoBehaviour
{
    
    private bool isAlreadyInvisible = false;

    private SpriteRenderer spriteRenderer;
    private new Rigidbody2D rigidbody2D;
    
    private float destroyDelay = 10F;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void InstantiateAndLaunch(Vector3 force, Vector4 color, float destroyDelay) {
        spriteRenderer.color = color;
        this.destroyDelay = destroyDelay;

        rigidbody2D.AddForce(force,ForceMode2D.Impulse);
    }

    private void OnBecameInvisible() {
        if (isAlreadyInvisible) return;
        GameObject.Destroy(this.gameObject,destroyDelay);
        isAlreadyInvisible = true;
    }


}
