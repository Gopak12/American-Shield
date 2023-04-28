using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : MonoBehaviour
{

    public BaseEnemy[] enemy;
    public LaserEnemy[] turrets;
    public Character player;
    public bool PointActive;

    void Start()
    {
        enemy = gameObject.GetComponentsInChildren<BaseEnemy>();
        //turrets = gameObject.GetComponentsInChildren<LaserEnemy>();
        player = FindFirstObjectByType<Character>();
        PointActive = true;
        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].controlPoint = this;
        }

        if (turrets.Length > 0)
        {
            for (int i = 0; i < turrets.Length; i++)
            {
                turrets[i].controlPoint = this;
            }
        }
        PointDeactivate();
    }

    public virtual void PointActivation()
    {
        for (int i = 0; i < enemy.Length; i++)
        {
                enemy[i].Activate();
        }

        for (int i = 0; i < turrets.Length; i++)
        {
            turrets[i].Activate();
        }
    }

    public virtual void PointDeactivate()
    {
        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].Deactivate();
        }

        for (int i = 0; i < turrets.Length; i++)
        {

            turrets[i].Deactivate();
        }
    }

    public void CheckPoint()
    {
        PointActive = false;
        for (int i = 0; i < enemy.Length; i++)
        {
            if (enemy[i].State == EnemyState.Alive || enemy[i].State == EnemyState.Unconscious)
            {
                PointActive = true;
            }
        }
        for (int i = 0; i < turrets.Length; i++)
        {
            if (turrets[i].State == EnemyState.Alive)
            {
                PointActive = true;
            }
        }
        if (!PointActive)
        {
            //gameObject.SetActive(false);
            player.CheckPoint(this);
        }
    }

}
