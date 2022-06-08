using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticBrickController : MonoBehaviour
{

    public GameObject debrisLauncherPrefab;

    private const int debrisCount = 15;
    private const float debrisForceMagnitude = 15F;
    private const float debrisForceMagnitudeVariance = 4F;
    private const float debrisAngleVarianceAmplitude = 30F;
    private const float debrisDestroyDelay = 10F;

    private Vector4 debrisColor = new Vector4(40F,256F,40F,256F) / 256F;

    private bool isDestroyed = false;

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
