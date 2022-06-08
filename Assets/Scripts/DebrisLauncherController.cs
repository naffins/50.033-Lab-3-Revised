using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DebrisLauncherController : MonoBehaviour
{

    private GlobalSoundPlayerController globalSoundPlayerController;

    void Awake() {
        globalSoundPlayerController = GameObject.FindGameObjectWithTag(Utils.globalSoundTag)
            .GetComponent<GlobalSoundPlayerController>();
    }

    public GameObject debrisPrefab;

    public void LaunchDebris(int debrisCount, float forceMagnitude, float forceMagnitudeVarianceAmplitude,
        Vector3 direction, float angleVarianceAmplitude, Vector4 debrisColor, float destroyDelay)
    {
        System.Random rng = new System.Random();
        Vector3 unitDirection = direction.normalized;
        for (int i=0;i<debrisCount;i++) {
            float reverseDirection = rng.NextDouble() < 0.5F? 1F: -1F;
            
            float angleVariation = (float)((rng.NextDouble() - 0.5F) * 2F * angleVarianceAmplitude);
            Vector3 debrisDirection = Quaternion.AngleAxis(angleVariation,Vector3.forward) * unitDirection;

            float forceMagnitudeVariation = (float)((rng.NextDouble() - 0.5F) * 2F * forceMagnitudeVarianceAmplitude);
            float debrisForceMagnitude = forceMagnitude + forceMagnitudeVariation;
            
            GameObject debris = Instantiate(debrisPrefab,transform.position,Quaternion.identity);
            debris.GetComponent<DebrisController>().InstantiateAndLaunch(debrisDirection * debrisForceMagnitude * reverseDirection,
                debrisColor,destroyDelay);
        }
        GameObject.Destroy(this.gameObject,1F);

        globalSoundPlayerController.PlayGlassBreakSound();
    }
}
