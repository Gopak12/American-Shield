using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public ControlPoint controlPoint;
    public bool EnemyActive;
    public bool CanShoot;
    public bool CanMove;
    public float time;
    public float speed;
    public float Rotspeed;
    public float shotingSpeed;
    public float shotingCooldowm;
    public float deathColorSpeed;
    public GameObject Bullet;
    public GameObject Player;
    public SkinnedMeshRenderer mesh;
    public Material MaterialAlive;
    public Material MaterialDied;
    public Material MaterialHighlight;
    public Material MaterialMarkHighlight;
    public Animator animator;
    public GameObject Canvas;
    public Image MarkBar;
    public float MarkProgress;
    public bool IsMark;
    Collider col;

    public bool IsSelected;

    public void Start()
    {
        EnemyActive = true;
        IsMark = false;
        col = GetComponent<Collider>();

        Canvas.SetActive(false);
        Player = FindFirstObjectByType<Character>().gameObject;
    }

    private void Update()
    {
        if (EnemyActive)
        {
            if (CanShoot)
            {
                time += Time.deltaTime;
                transform.LookAt(Player.transform, Vector3.up);
                if (time >= shotingCooldowm)
                {
                    time -= shotingCooldowm;
                    animator.SetTrigger("Shoot");
                    Shoot();
                }
            }
            /*if (CanMove)
            {

                transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, speed * Time.deltaTime);
            }*/

        }
    }

    public void Shoot()
    {
        Bullet bullet = Instantiate(Bullet, new Vector3(transform.position.x, transform.position.y + 1.75f, transform.position.z), Quaternion.identity).GetComponent<Bullet>();
        bullet.targetPos = Player.transform;
        bullet.ParentEnemy = this;
        bullet.speed = shotingSpeed;
    }

    public void Selected()
    {
        if (EnemyActive && !IsMark)
        {
            mesh.material = MaterialHighlight;
        }
    }

    public void Deselected()
    {
        if (EnemyActive && !IsMark)
        {
            mesh.material = MaterialAlive;
            MarkProgress = 0;
            MarkBar.transform.localScale = Vector3.one;
            Canvas.SetActive(false);
        }
    }

    public void AddMarkProgress()
    {
        if (IsMark) return;
        MarkProgress += 2* Time.deltaTime;
        Canvas.SetActive(true);
        MarkBar.transform.Rotate(new Vector3(0,0,Rotspeed));
        MarkBar.transform.localScale = Vector3.Lerp(Vector3.one*1.5f, Vector3.one * 0.95f,MarkProgress);

        //MarkBar.fillAmount = MarkProgress;
        if (MarkProgress >= 1)
        {
            if (!Player.GetComponent<Character>().enemiesQueue.Contains(this))
            {
                Player.GetComponent<Character>().enemiesQueue.Add(this);
            }
            mesh.material = MaterialMarkHighlight;
            IsMark = true;
            Canvas.SetActive(false);
            // MarkSign.enabled = true;
        }
    }

    public void Death()
    {
        animator.SetTrigger("Death");
        EnemyActive = false;
        col.enabled = false;
        Canvas.SetActive(false);

        controlPoint.CheckPoint();
        if (Player.GetComponent<Character>().enemiesQueue.Contains(this))
        {
            Player.GetComponent<Character>().enemiesQueue.Remove(this);
        }
        StartCoroutine(DeathColor());
    }

    IEnumerator DeathColor()
    {
        yield return new WaitForSeconds(0.1f);
        mesh.material = MaterialDied;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bullet>() && other.GetComponent<Bullet>().reflected)
        {
            Death();
        }
    }
}
