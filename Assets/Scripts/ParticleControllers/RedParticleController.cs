using UnityEngine;

public class RedParticleController : GenericParticleController
{
    protected override float GetDespawnTime() {
        return 0.5F;
    }
}