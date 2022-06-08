using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainCameraController : MonoBehaviour
{

    private const float playerYDeviationTolerance = 0.2F;
    private Transform playerTransform;

    private Vector3 initialPosition;

    void Awake() {
        initialPosition = transform.position;
    }

    void Start() {
        playerTransform = GameObject.FindGameObjectWithTag(Utils.PlayerTag).transform;
    }

    void Update()
    {
        float height, absTolerance;
        height = GetCameraViewDimensions().y;
        absTolerance = height * playerYDeviationTolerance;
        if (playerTransform.position.y>=transform.position.y) {
            if (playerTransform.position.y - transform.position.y < absTolerance) return;
            transform.position = new Vector3(transform.position.x, playerTransform.position.y-absTolerance, transform.position.z);
            return;
        }
        if (transform.position.y - playerTransform.position.y < absTolerance) return;
        transform.position = new Vector3(transform.position.x, Math.Max(initialPosition.y,playerTransform.position.y+absTolerance), transform.position.z);
    }

    public Vector2 GetCameraViewDimensions() {
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(Vector3.zero);
        return (new Vector2(transform.position.x - bottomLeft.x,transform.position.y - bottomLeft.y)) * 2F;
    }

    public Vector2 GetCameraPosition() {
        return new Vector2(transform.position.x,transform.position.y);
    }
}
