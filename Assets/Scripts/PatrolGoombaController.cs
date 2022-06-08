using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PatrolGoombaController : MonoBehaviour
{
    private const int debrisCount = 15;
    private const float debrisForceMagnitude = 12.5F;
    private const float debrisForceMagnitudeVariance = 4F;
    private const float debrisAngleVarianceAmplitude = 45F;
    private const float debrisDestroyDelay = 10F;


    public GameObject debrisLauncherPrefab;

    public float patrolPeriod;
    public float patrolAmplitude;
    public float animationPeriod;
    
    private Vector4 debrisColor = new Vector4(256F,40F,40F,256F) / 256F;

    private float animationTimeElapsed, motionTimeElapsed;
    private float animationHalfPeriod, patrolQuarterPeriod, patrolDirectionMultiplier;
    private Vector3 initialPosition;
    private SpriteRenderer ownRenderer, childRenderer;

    private bool isDestroyed = false;

    void Awake()
    {
        animationTimeElapsed = 0F;
        motionTimeElapsed = 0F;
        animationHalfPeriod = animationPeriod / 2F;
        patrolQuarterPeriod = patrolPeriod / 4F;
        patrolDirectionMultiplier = (new System.Random()).NextDouble() < 0.5F? 1F : -1F;

        initialPosition = transform.position;
        ownRenderer = GetComponent<SpriteRenderer>();
        
    }

    void Start() {
        foreach (Transform t in transform) childRenderer = t.gameObject.GetComponent<SpriteRenderer>();
    }

    void Update() {
        Animate();
    }

    private void Animate() {
        animationTimeElapsed = (animationTimeElapsed + Time.deltaTime) % animationPeriod;
        bool flip = animationTimeElapsed >= animationHalfPeriod;
        ownRenderer.flipX = flip;
        childRenderer.flipX = flip;
    }

    void FixedUpdate() {
        Move();
    }

    private void Move()
    {
        motionTimeElapsed = (motionTimeElapsed + Time.deltaTime) % patrolPeriod;
        Vector3 targetPosition = new Vector3(initialPosition.x,transform.position.y,transform.position.z);
        if (motionTimeElapsed < patrolQuarterPeriod) {
            targetPosition += new Vector3(patrolAmplitude*motionTimeElapsed/patrolQuarterPeriod,0F,0F) * patrolDirectionMultiplier;
        }
        else if (motionTimeElapsed > patrolPeriod - patrolQuarterPeriod) {
            targetPosition += new Vector3(patrolAmplitude*(motionTimeElapsed-patrolPeriod)/patrolQuarterPeriod,0F,0F) * patrolDirectionMultiplier;
        }
        else {
            targetPosition += new Vector3(patrolAmplitude*((0.5F*patrolPeriod)-motionTimeElapsed)/patrolQuarterPeriod,0F,0F) * patrolDirectionMultiplier;
        }
        transform.position = targetPosition;
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag!=Utils.offensiveParticleTag) return;
        if (isDestroyed) return;
        isDestroyed = true;
        GameObject debrisLauncher = GameObject.Instantiate(debrisLauncherPrefab,transform.position,Quaternion.identity);
        debrisLauncher.GetComponent<DebrisLauncherController>().LaunchDebris(
            debrisCount,
            debrisForceMagnitude,
            debrisForceMagnitudeVariance,
            other.gameObject.transform.rotation * Vector3.right,
            debrisAngleVarianceAmplitude,
            debrisColor,
            debrisDestroyDelay
        );
        GameObject.Destroy(gameObject,0F);
        GameObject.Destroy(other.gameObject,0F);
    }

    
}
