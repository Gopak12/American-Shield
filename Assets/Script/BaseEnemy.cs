using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RayFire;




public class BaseEnemy : Enemy
{
    public bool HasShield;
    public GameObject EnemyShield;
    public RayfireRigid ShieldDestract;
    public bool CanShoot;
    public bool CanMove;
    public float speed;
    public float shotingSpeed;
    public float shotingCooldowm;
    public float deathColorSpeed;
    public GameObject Bullet;

    float actifationSpeed;

    public bool IsSelected;

    public void Start()
    {
        IsMark = false;
        Canvas.SetActive(false);
        Player = FindFirstObjectByType<Character>().gameObject;
        transform.LookAt(Player.transform, Vector3.up);
        if (HasShield)
        {
            EnemyShield.SetActive(true);
            CanMove = false;
            CanShoot = false;
            animator.Play("Crouched Walking");
        }
        else
        {
            EnemyShield.SetActive(false);
            CanShoot = false;
            CanMove = false;
            actifationSpeed = Random.Range(0.4f, 1.4f);
            if (actifationSpeed < 1)
            {
                animator.Play("KipUpZeroPos");
            }
            else
            {
                animator.Play("Crouch Idle");
            }
        }
        puppet.mode = PuppetMaster.Mode.Kinematic;
    }

    private void Update()
    {
        if (State == EnemyState.Alive)
        {
            transform.LookAt(Player.transform, Vector3.up);
            if (CanShoot)
            {
                time += Time.deltaTime;
                if (time >= shotingCooldowm)
                {
                    time -= shotingCooldowm;
                    animator.speed = 1f;
                    animator.SetTrigger("Shoot");
                    Shoot();
                }
            }
            if (CanMove)
            {

                transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, speed * Time.deltaTime);
            }

        }
    }

    public void Shoot()
    {
        Bullet bullet = Instantiate(Bullet, new Vector3(transform.position.x, transform.position.y + 1.75f, transform.position.z), Quaternion.identity).GetComponent<Bullet>();
        bullet.targetPos = Player.transform;
        bullet.ParentEnemy = this;
        bullet.speed = shotingSpeed;
    }


    public void TakeDamage(float damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            Death();
        }
    }

    public override bool ShieldCheck()
    {
        if (HasShield)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void TakeDamage()
    {

        animator.enabled = true;
        if (HasShield)
        {
            HasShield = false;
            EnemyShield.transform.SetParent(null);
            ShieldDestract.Demolish();
        }
        else
        {
            HP -= 1f;

            puppet.mode = PuppetMaster.Mode.Active;

            if (HP <= 0)
            {
                Death();
            }
        }
    }

    IEnumerator LoseConsciousness()
    {
        if (puppet)
        {
            float p = 1;
            puppet.state = PuppetMaster.State.Dead;
            State = EnemyState.Unconscious;
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
                State = EnemyState.Alive;
            }
        }
    }

    public override void Activate()
    {
        animator.enabled = true;
        if (HasShield)
        {
            EnemyShield.SetActive(true);
            CanMove = true;
            CanShoot = false;

            animator.Play("Crouched Walking");
        }
        else
        {
            CanShoot = true;
            CanMove = false;
            StartCoroutine(OpenAnimationPlay());
        }

        puppet.mode = PuppetMaster.Mode.Active;
        State = EnemyState.Alive;
    }

    IEnumerator OpenAnimationPlay()
    {
        yield return new WaitForSeconds(actifationSpeed/10f);

        animator.speed = 2f - actifationSpeed;

        if (actifationSpeed < 1)
        {
            animator.Play("Kip Up");
        }
        else
        {
            animator.Play("Activation");
        }
    }

    public override void Deactivate()
    {
        gameObject.SetActive(true);
        CanShoot = false;
        CanMove = false;

        StartCoroutine(AnimatorDeactivate());
        State = EnemyState.Diactivate;
    }
    public override void Death()
    {

        HP = 0f;
        if (puppet)
        {

            puppet.state = PuppetMaster.State.Dead;
        }
        else
        {
            animator.SetTrigger("Death");
        }
        State = EnemyState.Dead;
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
