using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : MonoBehaviour
{

    public Enemy[] enemy;
    public Character player;
    public bool PointActive;

    void Start()
    {
        enemy = gameObject.GetComponentsInChildren<Enemy>();
        player = FindFirstObjectByType<Character>();
        PointActive = true;
        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].controlPoint = this;
        }
        PointDiactivation();
    }

    public virtual void PointActivation()
    {
        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].gameObject.SetActive(true);
            enemy[i].Player = player.gameObject;
            enemy[i].CanShoot = true;
            enemy[i].CanMove = true;
            enemy[i].animator.Play("Activation");
        }
    }

    public virtual void PointDiactivation()
    {
        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].gameObject.SetActive(true);
            enemy[i].CanShoot = false;
            enemy[i].CanMove = false;
            
        }
    }

    public void CheckPoint()
    {
        PointActive = false;
        for (int i = 0; i < enemy.Length; i++)
        {
            if (enemy[i].EnemyActive)
            {
                PointActive = true;
            }
        }
        if(!PointActive)
        {
            //gameObject.SetActive(false);
            player.InPoint = false;
        }
    }

}
