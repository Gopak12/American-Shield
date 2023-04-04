using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BaseEnemy ParentEnemy;
    public Transform targetPos;

    [HideInInspector] public Vector3 blockedVector;
    public float speed;
    public bool reflected;
    public bool blocked;


    void Update()
    {
        if (reflected)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPos.transform.position.x, targetPos.transform.position.y + 1.75f, targetPos.transform.position.z), speed * Time.deltaTime);
        }
        else if (!blocked)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPos.transform.position.x, targetPos.transform.position.y + 0.9f, targetPos.transform.position.z), speed * Time.deltaTime);

        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(blockedVector.x, blockedVector.y, blockedVector.z), speed * Time.deltaTime);
        }
        if (ParentEnemy.State == EnemyState.Dead)
        {
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (reflected && other.GetComponentInParent<Enemy>())
        {
            other.GetComponentInParent<Enemy>().TakeDamage();
            Destroy(this.gameObject);
        }
    }

}
