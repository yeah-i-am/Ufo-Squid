using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParticlesController : MonoBehaviour
{
    public ParticleSystem[] backgroundParticles;
    public bool isUnity;

    private ParticleSystem.Particle[] particles = new ParticleSystem.Particle[500];

    // Start is called before the first frame update
    void Start()
    {
        foreach (ParticleSystem ps in backgroundParticles)
        {
            /* Changing particle system scale */
            float scale = 11f * Screen.width / Screen.height;
            var shape = ps.shape;
            shape.scale = new Vector3(scale, 11f, 1f);

            /* Changing particles size */
            /*var main = ps.main;
            main.startSize = (isUnity ? 50f : 300f) / Screen.dpi; */

            ps.Emit(500);
            ps.GetParticles(particles);

            for(int i = 0; i < particles.Length; i++)
                particles[i].remainingLifetime = Random.Range(0, particles[i].startLifetime);

            ps.SetParticles(particles);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
