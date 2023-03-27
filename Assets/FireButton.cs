using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireButton : MonoBehaviour
{
    public Enemy[] enemy;
    public float delay = 1;
    public GameObject Particle;

    public void Start()
    {
        Particle.SetActive(false);
    }

    public void Activate()
    {
        Particle.SetActive(true);
        for (int i = 0; i < enemy.Length; i++)
        {
            if (enemy[i].State == EnemyAtate.Alive)
            {
                enemy[i].Death();
            }

        }
        Invoke("Deactivate", delay);
    }

    public void Deactivate()
    {
        Particle.SetActive(false);
    }
}
