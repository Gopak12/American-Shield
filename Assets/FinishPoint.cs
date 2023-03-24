using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    public ParticleSystem[] particle;

    public void ParticleActivate()
    {
        for (int i = 0; i < particle.Length; i++)
        {
            particle[i].Play();
        }
    }
}
