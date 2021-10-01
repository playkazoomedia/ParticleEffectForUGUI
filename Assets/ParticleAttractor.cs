using UnityEngine;
using System.Collections;
using Coffee.UIExtensions;
using Coffee.UIParticleExtensions;

[ExecuteAlways]
public class ParticleAttractor : MonoBehaviour
{
    public Transform destination;

    // public float distanceNeeded = 0.5f;
    public ParticleSystem myParticleSystem;

    private ParticleSystem.Particle[] s_TmpParticles = new ParticleSystem.Particle[2048];

    public AnimationCurve curve;
    public float AttractionDistance = 1;
    [Range(0f, 0.95f)] public float delay = 0;
    public float multiplier = 1;

    public float attractionTime = 1;
    public float maxSpeed = 1;

    public UIParticle uip;

    void OnEnable()
    {
        myParticleSystem = this.GetComponent<ParticleSystem>();

        if (myParticleSystem == null)
        {
            Debug.Log("No particle system attached to particle attractor script");
            this.enabled = false;
        }

        s_TmpParticles = new ParticleSystem.Particle[myParticleSystem.main.maxParticles];
    }

    void Update()
    {
        if (destination == null || uip == null)
            return;

        var scale = uip.transform.localScale;
        scale.Scale(uip.scale3D);



        var dstPos = destination.transform.position;
        if (myParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.Local)
        {
            dstPos = Vector3.Scale(myParticleSystem.transform.InverseTransformPoint(dstPos), scale.Inverse());
        }
        else
        {
            dstPos = Vector3.Scale(dstPos, scale.Inverse());
        }

        var count = myParticleSystem.particleCount;
        if (s_TmpParticles.Length < count)
        {
            var size = Mathf.NextPowerOfTwo(count);
            s_TmpParticles = new ParticleSystem.Particle[size];
        }

        myParticleSystem.GetParticles(s_TmpParticles);
        for (var i = 0; i < count; i++)
        {
            var particle = s_TmpParticles[i];

            if (Vector3.Distance(particle.position, dstPos) < AttractionDistance)
            {
                particle.remainingLifetime = 0.01f;
            }
            else
            {
                var delayTime = particle.startLifetime * delay;
                var duration = particle.startLifetime - delayTime;
                var time = Mathf.Max(0, particle.startLifetime - particle.remainingLifetime - delayTime);
                particle.position = Vector3.Slerp(particle.position, dstPos, time / duration);
            }

            s_TmpParticles[i] = particle;
        }

        myParticleSystem.SetParticles(s_TmpParticles, count);
    }
}
