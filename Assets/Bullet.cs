using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Enemy ParentEnemy;
    public Transform targetPos;

    [HideInInspector] public Vector3 blockedVector;
    public float speed;
    public bool reflected;


    void Update()
    {
        if (!ParentEnemy.EnemyActive && reflected)
        {
            Destroy(this.gameObject);
        }
        else if (blockedVector == Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPos.transform.position.x, targetPos.transform.position.y + 0.9f, targetPos.transform.position.z), speed * Time.deltaTime);
        }else
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(blockedVector.x, blockedVector.y, blockedVector.z), speed * Time.deltaTime);
        }

    }

    
}
