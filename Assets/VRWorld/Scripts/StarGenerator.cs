using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public ParticleSystem particle_system;
    public int maxParticles = 10000;
    public TextAsset starCSV;
    public string[] lines;
    public bool starsGenerated;
    private Vector3 normalizedPosition;
    public float farclipplane;
    void Awake()
    {
        starsGenerated = false;
        ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1];
        var main = particle_system.main; main.maxParticles = maxParticles;
        bursts[0].minCount = (short)maxParticles;
        bursts[0].maxCount = (short)maxParticles;
        bursts[0].time = 0.0f;
        particle_system.emission.SetBursts(bursts, 1);
    }
    void LateUpdate()
    {
        if (!starsGenerated)
        {
            starsGenerated = true;
            lines = starCSV.text.Split('\n');
            ParticleSystem.Particle[] particleStars = new ParticleSystem.Particle[maxParticles];
            particle_system.GetParticles(particleStars);
            for (int i = 0; i < maxParticles; i++)
            {
                string[] components = lines[i].Split(',');
                normalizedPosition = new Vector3(float.Parse(components[1]),float.Parse(components[3]),float.Parse(components[2])) * farclipplane;
                particleStars[i].position = normalizedPosition;
                particleStars[i].remainingLifetime = Mathf.Infinity;
                particleStars[i].startColor = (Color.white * (1.0f - ((float.Parse(components[0]) + 1.44f) / 8)));
        }
        particle_system.SetParticles(particleStars, maxParticles);
        }
    }
}
