using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{

    public float maxHorizontalSpeed;
    public float horizontalAccelerationTime;

    public float jumpImmediateSpeedIncrement;
    public float jumpGradualSpeedIncrement;
    public float maxJumpDuration;

    public AudioClip landingAudioClip, jumpAudioClip;

    private const KeyCode leftKey = KeyCode.A;
    private const KeyCode rightKey = KeyCode.D;
    private const KeyCode upKey = KeyCode.Space;

    private const float minHorizontalSlideSpeed = 0.1F;
    private const string animationStateVarName = "motionState";
    private const string shirtLightName = "Shirt Light";

    private KeyCode[] colorKeys = {KeyCode.Q, KeyCode.W, KeyCode.E};
    private Vector4 minimumColor = new Vector4(1F,1F,1F,0F) * 0.125F;
    private Vector4 glowShirtAlphaIncrement = new Vector4(0F,0F,0F,1F);

    private new Rigidbody2D rigidbody2D;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private Animator animator;
    private Material shirtMaterial;
    private Light2D glowShirtLight2D;
    private GameController gameController;

    private HashSet<GameObject> contactingTerrainSet;
    private HashSet<GameObject> emptyingTerrainSet;
    private bool[] upKeyPressed = {false, false};
    private float jumpDurationCounter;
    
    private bool isFacingRight;
    private int currentAnimationStatus;
    private Vector3 horizontalForceIncrement, jumpImmediateForceIncrement, jumpGradualForceIncrement;

    void Awake() {
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        contactingTerrainSet = new HashSet<GameObject>();
        emptyingTerrainSet = new HashSet<GameObject>();
        upKeyPressed[0] = false;
        upKeyPressed[1] = false;

        jumpDurationCounter = -1F;
        isFacingRight = true;
        currentAnimationStatus = 0;

        horizontalForceIncrement = new Vector3(maxHorizontalSpeed/horizontalAccelerationTime,0F,0F);
        jumpImmediateForceIncrement = new Vector3(0F,jumpImmediateSpeedIncrement,0F);
        jumpGradualForceIncrement = new Vector3(0F,jumpGradualSpeedIncrement/maxJumpDuration,0F);

        shirtMaterial = spriteRenderer.material;
    }

    void Start() {
        foreach (Transform t in transform) {
            if (t.gameObject.name==shirtLightName) glowShirtLight2D = t.gameObject.GetComponent<Light2D>();
        }
        gameController = GameObject.FindGameObjectWithTag(Utils.gameControllerTag).GetComponent<GameController>();
    }

    void FixedUpdate() {
        UpdateTerrainSet();
        TrackUpKeyPressed();
        MoveHorizontal();
        MoveVertical();
    }

    private void UpdateTerrainSet() {
        foreach (GameObject g in contactingTerrainSet) {
            if (g==null) {
                emptyingTerrainSet.Add(g);
            }
        }
        if (emptyingTerrainSet.Count>0) {
            foreach (GameObject g in emptyingTerrainSet) {
                contactingTerrainSet.Remove(g);
            }
            emptyingTerrainSet.Clear();
        }
    }

    private void MoveHorizontal() {
        if (!IsPressedHorizontally()) {
            if (Math.Abs(GetHorizontalSpeed())<minHorizontalSlideSpeed) rigidbody2D.velocity = new Vector2(0F,rigidbody2D.velocity.y);
            return;
        }

        float currentSpeed = GetHorizontalSpeed() * GetHorizontalAxis();
        
        if (currentSpeed<maxHorizontalSpeed) {
            rigidbody2D.AddForce(horizontalForceIncrement * GetHorizontalAxis());
        }
    }

    private bool IsPressedHorizontally() {
        return Input.GetKey(leftKey) ^ Input.GetKey(rightKey);
    }

    private float GetHorizontalAxis() {
        if (!IsPressedHorizontally()) return 0F;
        return Input.GetKey(rightKey)? 1F : -1F;
    }

    private float GetHorizontalSpeed() {
        return rigidbody2D.velocity.x;
    }

    private bool IsMovingHorizontally() {
        return Math.Abs(GetHorizontalSpeed()) >= Utils.epsilon;
    }

    public void OnChildTriggerEnter2D(Collider2D other) {
        GameObject otherObj = other.gameObject;
        switch(otherObj.tag) {
            case Utils.PlatformParticleTag:
            case Utils.PermanentTerrainTag:
            case Utils.TerrainTag:
            
                contactingTerrainSet.Add(otherObj);
                if ((contactingTerrainSet.Count==1)&&CanJump()) {
                    PlaySound(landingAudioClip);
                }
                break;
            default:
                break;
        }
    }

    public void OnChildTriggerExit2D(Collider2D other) {
        GameObject otherObj = other.gameObject;
        switch(otherObj.tag) {
            case Utils.PlatformParticleTag:
            case Utils.PermanentTerrainTag:
            case Utils.TerrainTag:
                contactingTerrainSet.Remove(otherObj);
                break;
            default:
                break;
        }
    }

    private void MoveVertical() {
        if (contactingTerrainSet.Count==0) {
            if (jumpDurationCounter < -0.5F) return;
            if ((!Input.GetKey(upKey))||(jumpDurationCounter > maxJumpDuration)) {
                jumpDurationCounter = -1F;
                return;
            }
            jumpDurationCounter += Time.deltaTime;
            rigidbody2D.AddForce(jumpGradualForceIncrement);
            return;
        }
        if (!CanJump()) return;
        if (!IsUpKeyDown()) return;
        rigidbody2D.AddForce(jumpImmediateForceIncrement,ForceMode2D.Impulse);
        jumpDurationCounter = 0F;
        PlaySound(jumpAudioClip);
    }

    private void TrackUpKeyPressed() {
        upKeyPressed[0] = upKeyPressed[1];
        upKeyPressed[1] = Input.GetKey(upKey);
    }

    private bool IsUpKeyDown() {
        return (!upKeyPressed[0]) && upKeyPressed[1];
    }

    private bool CanJump() {
        return (rigidbody2D.velocity.y<=Utils.epsilon) && (contactingTerrainSet.Count>0);
    }

    private void PlaySound(AudioClip audioClip) {
        audioSource.PlayOneShot(audioClip);
    }

    void Update() {
        Animate();
        Glow();
    }

    private void Animate() {
        if (IsMovingHorizontally()) {
            isFacingRight = GetHorizontalSpeed() >= 0F;
        }

        int nextAnimationStatus = GetNextAnimationStatus();
        if (nextAnimationStatus!=currentAnimationStatus) {
            currentAnimationStatus = nextAnimationStatus;
            animator.SetInteger(animationStateVarName,currentAnimationStatus);
        }

        spriteRenderer.flipX = (!isFacingRight) ^ (currentAnimationStatus==GetAnimationStateNumber("Skidding"));
    }

    private int GetNextAnimationStatus() {
        if (!CanJump()) {
            return GetAnimationStateNumber("Jumping");
        }
        if (!IsMovingHorizontally()) {
            return GetAnimationStateNumber("Idle");
        }
        if (!IsPressedHorizontally()) {
            return GetAnimationStateNumber("Skidding");
        }
        string status = 10000F * GetHorizontalSpeed() * GetHorizontalAxis() > 0F? "Running" : "Skidding";
        return GetAnimationStateNumber(status);
    }

    private int GetAnimationStateNumber(string state) {
        switch (state) {
            case "Idle":
                return 0;
            case "Jumping":
                return 1;
            case "Running":
                return 2;
            case "Skidding":
                return 3;
            default:
                throw new NotImplementedException();
        }
    }

    public bool IsAnyColorKeyPressed() {
        for (int i=0;i<colorKeys.Length;i++) {
            if (Input.GetKey(colorKeys[i])) return true;
        }
        return false;
    }

    public int GetColorKeyPressed() {
        for (int i=colorKeys.Length-1;i>=0;i--) if (Input.GetKey(colorKeys[i])) return i;
        return -1;
    }

    public Vector4 GetShirtGlow() {
        Vector4 newColor = Vector4.zero;
        if (IsAnyColorKeyPressed()) {
            for (int i=0;i<colorKeys.Length;i++) {
                if (Input.GetKey(colorKeys[i])) newColor[i] = 1F;
            }
            newColor += minimumColor;

            for (int i=0;i<3;i++) {
                newColor[i] = Math.Min(newColor[i],1F);
            }
        }
        return newColor;
    }

    private void Glow() {
        Vector4 shirtGlow = GetShirtGlow();
        spriteRenderer.material.color = shirtGlow;
        
        if (IsAnyColorKeyPressed()) {
            glowShirtLight2D.enabled = true;
            glowShirtLight2D.color = shirtGlow + glowShirtAlphaIncrement;
        }
        else {
            glowShirtLight2D.enabled = false;
            glowShirtLight2D.color = Vector4.zero;
        }
        
    }

    public Vector4 GetGlowShirtAlphaIncrement() {return glowShirtAlphaIncrement;}

    private void OnCollisionEnter2D(Collision2D other) {
        if (!other.gameObject.CompareTag(Utils.enemyTag)) return;
        gameController.ToggleLose();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.gameObject.CompareTag(Utils.exitTag)) return;
        gameController.ToggleWin();
    }
}
