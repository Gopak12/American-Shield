using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{

    public LayerMask DeadZoneLayer;

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer) == DeadZoneLayer.value)
        {
            other.gameObject.GetComponentInParent<Enemy>().TakeDamage();
            other.gameObject.GetComponentInParent<Enemy>().TakeDamage();
            other.gameObject.GetComponentInParent<Enemy>().TakeDamage();
            other.gameObject.GetComponentInParent<Enemy>().TakeDamage();
        }
    }
}
