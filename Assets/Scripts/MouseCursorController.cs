using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering.Universal;

public class MouseCursorController : MonoBehaviour
{

    public Transform particleParent;
    public GameObject[] particles;
    public AudioClip electricAudioClip;

    private const float interParticleSeparation = 0.5F;

    private Vector3 iVector = new Vector3(1F,0F,0F);
    private Vector3 kVector = new Vector3(0F,0F,1F);
    

    private RectTransform rectTransform;
    private Light2D ownLight;
    private Material ownMaterial;

    private MainCameraController mainCameraController;
    private PlayerController playerController;
    private AudioSource audioSource;

    private Vector3 lastMousePosition;
    private bool isTrackingMouse;

    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        ownLight = GetComponent<Light2D>();
        ownMaterial = GetComponent<SpriteRenderer>().material;
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = electricAudioClip;

        isTrackingMouse = false;
    }

    void Start() {
        mainCameraController = GameObject.FindGameObjectWithTag(Utils.MainCameraTag).GetComponent<MainCameraController>();
        playerController = GameObject.FindGameObjectWithTag(Utils.PlayerTag).GetComponent<PlayerController>();
    }

    void Update() {
        Glow();
    }

    private void Glow() {
        Vector4 basicColor = playerController.GetShirtGlow();
        ownMaterial.color = basicColor;
        if (!playerController.IsAnyColorKeyPressed()) {
            ownLight.color = basicColor;
            ownLight.enabled = false;
            return;
        }
        ownLight.enabled = true;
        ownLight.color = basicColor + playerController.GetGlowShirtAlphaIncrement();
    }

    void FixedUpdate()
    {
        Vector3 currentPosition = MoveAndGetPosition();
        TrackParticleSpawn(currentPosition);
    }

    private Vector3 MoveAndGetPosition() {
        Vector3 mouseScreenRelativePosition = UpdateAndGetMouseScreenRelativePosition();        
        Vector3 mouseWorldPosition = GetMouseWorldPosition(mouseScreenRelativePosition);
        rectTransform.position = new Vector3(
            mouseWorldPosition.x,
            mouseWorldPosition.y,
            rectTransform.position.z
        );
        return mouseWorldPosition;
    }

    private Vector3 UpdateAndGetMouseScreenRelativePosition() {
        Vector3 mousePositionOnScreen = Input.mousePosition;
        Vector3 relativePosition = new Vector3(
            Math.Min(0.5F,Math.Max(-0.5F,(mousePositionOnScreen.x/Screen.width) - 0.5F)),
            Math.Min(0.5F,Math.Max(-0.5F,(mousePositionOnScreen.y/Screen.height) - 0.5F)),
            0F
        );
        return relativePosition;
    }

    private Vector3 GetMouseWorldPosition(Vector3 mouseScreenRelativePosition) {
        Vector3 cameraWorldPosition = mainCameraController.GetCameraPosition();
        Vector3 cameraViewDimensions = mainCameraController.GetCameraViewDimensions();
        return new Vector3(
            (mouseScreenRelativePosition.x*cameraViewDimensions.x) + cameraWorldPosition.x,
            (mouseScreenRelativePosition.y*cameraViewDimensions.y) + cameraWorldPosition.y,
            0F
        );
    }

    private void TrackParticleSpawn(Vector3 currentPosition) {
        if (IsParticleSpawningEnabled()) {
            if (!isTrackingMouse) {
                isTrackingMouse = true;
                lastMousePosition = currentPosition;
                audioSource.Play();
            }
            Vector3 positionChange = currentPosition - lastMousePosition;
            if (positionChange.magnitude >= interParticleSeparation) SpawnParticleAndUpdateLastPosition(playerController.GetColorKeyPressed(),currentPosition);
        }
        else {
            if (isTrackingMouse){
                isTrackingMouse = false;
                audioSource.Pause();
            }
        }
    }

    private bool IsParticleSpawningEnabled() {
        return Input.GetMouseButton(0) && playerController.IsAnyColorKeyPressed();
    }

    private void SpawnParticleAndUpdateLastPosition(int colorKeyIndex, Vector3 newPosition) {
        Vector3 diffVector = newPosition - lastMousePosition;
        Vector3 intermediatePosition = 0.5F * (lastMousePosition + newPosition);
        float directionAngle = GetAdjustedAngle(diffVector);
        GameObject newParticle = Instantiate(particles[colorKeyIndex],intermediatePosition,Quaternion.identity,particleParent);
        newParticle.transform.Rotate(kVector * directionAngle);
        newParticle.transform.localScale = new Vector3(newParticle.transform.localScale.x*diffVector.magnitude,
            newParticle.transform.localScale.y,newParticle.transform.localScale.z);
        lastMousePosition = newPosition;
    }

    private float GetAdjustedAngle(Vector3 currentDirection) {
        float rawAngle = Vector3.SignedAngle(iVector,currentDirection,kVector);
        if (Math.Abs(rawAngle)<90F) return rawAngle;
        if (rawAngle>0F) return rawAngle-180F;
        return rawAngle+180F;
    }
}
