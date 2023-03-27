using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyAtate { Diactivate, Alive, Unconscious, Dead }

public class Enemy : MonoBehaviour
{
    public ControlPoint controlPoint;
    public EnemyAtate State;
    public bool CanShoot;
    public bool CanMove;
    public float HP = 2;
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
    public PuppetMaster puppet;
    public GameObject Canvas;
    public Image MarkBar;
    public float MarkProgress;
    public bool IsMark;
    Collider col;


    public bool IsSelected;

    public void Start()
    {
        State = EnemyAtate.Alive;
        IsMark = false;
        col = GetComponent<Collider>();
        Canvas.SetActive(false);
        Player = FindFirstObjectByType<Character>().gameObject;
        transform.LookAt(Player.transform, Vector3.up);

    }

    private void Update()
    {
        if (State == EnemyAtate.Alive)
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
        if (State == EnemyAtate.Alive && !IsMark)
        {
            mesh.material = MaterialHighlight;
        }
    }

    public void Deselected()
    {
        if (State == EnemyAtate.Alive && !IsMark)
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
        MarkProgress += 2 * Time.deltaTime;
        Canvas.SetActive(true);
        MarkBar.transform.Rotate(new Vector3(0, 0, Rotspeed));
        MarkBar.transform.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one * 0.95f, MarkProgress);

        if (MarkProgress >= 1)
        {
            if (!Player.GetComponent<Character>().enemiesQueue.Contains(this))
            {
                Player.GetComponent<Character>().enemiesQueue.Add(this);
            }
            mesh.material = MaterialMarkHighlight;
            IsMark = true;
            Canvas.SetActive(false);
        }
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            Death();
        }
    }

    public void TakeDamage()
    {
        HP -= 1f;
        if (State == EnemyAtate.Alive)
        {
            StartCoroutine(LoseConsciousness());
        }

        if (HP <= 0)
        {
            Death();
        }
    }

    IEnumerator LoseConsciousness()
    {
        if (puppet)
        {
            float p = 1;
            puppet.state = PuppetMaster.State.Dead;
            State = EnemyAtate.Unconscious;
            yield return new WaitForSeconds(10.93f);
            if (HP > 0)
            {
                puppet.state = PuppetMaster.State.Alive;
                puppet.pinWeight = 0;
                for (float t = 0; t < 1;)
                {


                    t = Mathf.MoveTowards(t, 1, 1f * Time.deltaTime);
                    p = Mathf.Lerp(0, 1, t);
                    puppet.pinWeight = p;
                    yield return null;
                }
                puppet.pinWeight = 1f;
                State = EnemyAtate.Alive;
            }
        }
    }

    public void Death()
    {
        if (puppet)
        {
            puppet.state = PuppetMaster.State.Dead;
        }
        else
        {
            animator.SetTrigger("Death");
        }
        State = EnemyAtate.Dead;
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
            TakeDamage();
        }
    }
}
