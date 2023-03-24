using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public enum ShieldState { InHand, Flying, Returning }
    public ShieldState state = ShieldState.InHand;
    public Transform playerTransform, handHolder;
    public Vector3 curveRotation, throwRotation;
    public float flyingSpeed;
    public float chargeSpeed;
    public float xSpinSpeed;
    public float returnDistance;
    public bool mustReturn, firstHitting, returns, targeted;
    public float dsd;

    public Collider col;
    public Character player;
    Vector3 rot;
    Rigidbody rb;
    public Transform ShieldModel, cam;



    void Start()
    {
        cam = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (state.Equals(ShieldState.InHand) && !returns)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Throw();
            }
            rb.velocity = Vector3.zero;
        }
        else if (state.Equals(ShieldState.Flying))
        {
            transform.Translate(transform.forward * flyingSpeed * Time.deltaTime, Space.World);
            Rotation();
            if (Input.GetMouseButtonUp(0))
            {
                mustReturn = true;
            }
        }

        if (playerTransform)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance > returnDistance && !returns)
            {
                if (playerTransform)
                {
                    StartCoroutine(BackMove());
                }
            }
        }

    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponent<Enemy>())
        {
            rot = transform.rotation.eulerAngles;
            if (player.enemiesQueue.Contains(other.gameObject.GetComponent<Enemy>()) && NearestEnemy(other.gameObject.GetComponent<Enemy>()) != transform)
            {
                Vector3 NextEnemy = NearestEnemy(other.gameObject.GetComponent<Enemy>()).position - transform.position;
                transform.rotation = Quaternion.LookRotation(new Vector3(NextEnemy.x, 0, NextEnemy.z));
                targeted = true;
            }
            else if (rot.y > 2 && rot.y < 360 - 2 && !targeted)
            {
                transform.rotation = Quaternion.Euler(rot.x, -(rot.y), rot.z);
            }
            else if (!targeted)
            {
                if (rot.y > 2)
                {
                    transform.rotation = Quaternion.Euler(rot.x, -30 * ((360 - rot.y) + 0.1f), rot.z);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(rot.x, 30 * (rot.y + 0.1f), rot.z);

                }

            }
            other.gameObject.GetComponent<Enemy>().Death();
        }
        else if (other.gameObject.GetComponent<Reflector>())
        {
            rot = transform.rotation.eulerAngles - other.transform.eulerAngles;
            Vector3 TarRot = transform.rotation.eulerAngles + other.transform.eulerAngles;

            if (rot.y > 0)
            {
                transform.rotation = Quaternion.Euler(rot.x, -(TarRot.y), rot.z);
                Debug.Log(1);
            }
            else if (rot.y < 0)
            {
                transform.rotation = Quaternion.Euler(rot.x, (TarRot.y), rot.z);

                Debug.Log(2);
            }

        }
        else if (other.gameObject.GetComponent<FireButton>()&&!returns)
        {
            other.gameObject.GetComponent<FireButton>().Activate();
        }
        else if (other.gameObject.tag == "Wall" && !returns)
        {
            StartCoroutine(BackMove());
        }

    }

    public Transform NearestEnemy(Enemy enemyTransform)
    {
        float dist = 100f;
        Transform nearestEnemy = this.transform;
        if (player.enemiesQueue.Contains(enemyTransform))
        {
            player.enemiesQueue.Remove(enemyTransform);
        }
        if (player.enemiesQueue.Count != 0)
        {
            for (int i = 0; i < player.enemiesQueue.Count; i++)
            {
                float currentDist = Vector3.Distance(transform.position, player.enemiesQueue[i].transform.position);
                if (currentDist <= dist && currentDist >= 0.5f)
                {
                    dist = currentDist;
                    nearestEnemy = player.enemiesQueue[i].transform;
                }
            }
        }
        return nearestEnemy;
    }

    public void Rotation()
    {
        ShieldModel.RotateAround(transform.position, ShieldModel.forward, xSpinSpeed * Time.deltaTime);
    }

    void Throw()
    {
        firstHitting = false;
        transform.SetParent(null);
        transform.position = cam.position + cam.forward;
        transform.eulerAngles = cam.eulerAngles + throwRotation;
        transform.localRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 40);
        col.enabled = true;
        rb.isKinematic = false;
        state = ShieldState.Flying;
    }
    IEnumerator BackMove()
    {

        state = ShieldState.Returning;
        returns = true;
        Vector3 StartPos = transform.position;
        Vector3 A = new Vector3((transform.position + cam.position).x / 2 + (cam.rotation.eulerAngles.y > 90 ? (360 - cam.rotation.eulerAngles.y) / 4 : -cam.rotation.eulerAngles.y / 4), (transform.position + cam.position).y / 2, (transform.position + cam.position).z / 2);
        Vector3 B;
        Vector3 C;
        //Quaternion Rotat = ShieldModel.rotation;
        for (float t = 0; t < 1;)
        {


            t = Mathf.MoveTowards(t, 1, 1.7f * Time.deltaTime);
            B = Vector3.Lerp(StartPos, A, t);
            C = Vector3.Lerp(A, handHolder.position, easeInCubic(t));
            transform.position = Vector3.Lerp(B, C, t);
            ShieldModel.RotateAround(transform.position, ShieldModel.forward, -2 * xSpinSpeed * Time.deltaTime);
            //ShieldModel.rotation = Quaternion.Lerp(Rotat, Quaternion.Euler(270f, 350f, 340f),t*2-1);
            yield return null;
        }
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.isKinematic = true;
        mustReturn = false;
        col.enabled = false;
        targeted = false;
        transform.SetParent(handHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0, 0, 180);

        returns = false;
        state = ShieldState.InHand;
    }

    public float easeInCubic(float x)
    {
        return x * x * x;
    }
}
