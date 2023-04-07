using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePoint : ControlPoint
{
    public Transform platform;
    public float value;
    public float speed;
    public Vector3 startPos;

   

    void Start()
    {
        player = FindFirstObjectByType<Character>();
        PointActive = true;
        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].controlPoint = this;
        }
        PointDeactivate();
        startPos = platform.transform.position;
    }
    private void Update()
    {
        //platform.position = Vector3.Lerp(startPos - Vector3.right * value, startPos + Vector3.right * value, Mathf.PingPong(Time.time* speed, 1f));
        platform.position = Vector3.Lerp(startPos - Vector3.right * value, startPos + Vector3.right * value, ( Mathf.Sin(Time.time* speed)+1)/2);
    }

    public override void PointActivation()
    {
        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].gameObject.SetActive(true);
            enemy[i].Player = player.gameObject;
            enemy[i].CanShoot = true;
            enemy[i].CanMove = false;

            enemy[i].animator.Play("Activation");
        }
    }

    public override void PointDeactivate()
    {
        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].gameObject.SetActive(true);
            enemy[i].CanShoot = false;
            enemy[i].CanMove = false;
        }
    }

}
