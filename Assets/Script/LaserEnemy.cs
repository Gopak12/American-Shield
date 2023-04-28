using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LaserState { Cooldowm, Preparation, Aiming, Shooting }

public class LaserEnemy : Enemy
{

    public LayerMask LaserAttackLayer;
    public LayerMask LaserparticleLayer;
    public float shotingCooldowm;
    public float preparationTime;
    public float aimingTime = 1;
    public float shootingTime;
    public LaserState laserState;

    public Character player;
    public GameObject FirePoint;
    public GameObject PreparationParticle;
    public GameObject HitEffect;
    public float HitOffset = 0;
    public bool useLaserRotation = false;
    public float MaxLength;
    public LineRenderer Laser;
    public float MainTextureLength = 1f;
    public float NoiseTextureLength = 1f;
    private Vector4 Length = new Vector4(1, 1, 1, 1);
    RaycastHit hit;
    Vector3 Direction;
    public float retreat;
    public Vector3 offSet;
    //private ParticleSystem[] Effects;
    public ParticleSystem[] Hit;




    void Start()
    {
        //Effects = GetComponentsInChildren<ParticleSystem>();
        //Hit = HitEffect.GetComponentsInChildren<ParticleSystem>();
        PreparationParticle.SetActive(false);
        State = EnemyState.Alive;
        IsMark = false;
        Canvas.SetActive(false);
        HitEffect.SetActive(false);
        Player = FindFirstObjectByType<Character>().gameObject;
        transform.LookAt(Player.transform, Vector3.up);
        puppet.mode = PuppetMaster.Mode.Kinematic;
        State = EnemyState.Diactivate;
        laserState = LaserState.Cooldowm;
        animator.Play("FocusZeroPos");
    }

    public void Update()
    {

        if (State != EnemyState.Alive) return;
        transform.LookAt(Player.transform, Vector3.up);

        //Direction = (player.cam.transform.position - new Vector3(0, 0.2f, 0)) - turretBody.transform.position;
        //turretBody.transform.rotation = Quaternion.LookRotation(Direction);

        if (laserState == LaserState.Cooldowm)
        {
            time += Time.deltaTime;
            if (time >= shotingCooldowm)
            {
                time -= shotingCooldowm;
                StartCoroutine(LaserShoot());
            }
        }

        if (laserState == LaserState.Aiming)
        {

            Laser.SetPosition(0, FirePoint.transform.position);
            player.IsReflectedLaser = false;
            Direction = (player.LaserTarget.transform.position) - FirePoint.transform.position + offSet;
            Laser.SetPosition(1, player.LaserTarget.transform.position + Direction);
            if (laserState == LaserState.Shooting)
            {
                player.Death();
            }
            if (Physics.Raycast(FirePoint.transform.position, (player.LaserTarget.transform.position + Direction) - FirePoint.transform.position , out hit, MaxLength, LaserparticleLayer))
            {
                HitEffect.transform.position = hit.point;
                HitEffect.transform.rotation = transform.rotation;
                HitEffect.transform.LookAt(hit.point + hit.normal);

            }

        }

        if (laserState == LaserState.Shooting)
        {
            if (player.shield.state.Equals(Shield.ShieldState.InHand) && player.shieldReadyCof > 0f)
            {
                player.IsReflectedLaser = true;

                Laser.SetPosition(0, FirePoint.transform.position);
                Direction = (player.LaserTarget.transform.position) - FirePoint.transform.position;
                if (Physics.Raycast(FirePoint.transform.position, Direction, out hit, MaxLength, LaserAttackLayer))
                {
                    Laser.SetPosition(1, hit.point);
                    player.ReflectedLaserStartPos.position = hit.point;
                    Length[0] = MainTextureLength * (Vector3.Distance(transform.position, hit.point));
                    Length[2] = NoiseTextureLength * (Vector3.Distance(transform.position, hit.point));
                }
                else
                {
                    //End laser position if doesn't collide with object
                    var EndPos = FirePoint.transform.position + Direction * MaxLength;
                    Laser.SetPosition(1, EndPos);
                    //HitEffect.transform.position = EndPo
                    Length[0] = MainTextureLength * (Vector3.Distance(transform.position, EndPos));
                    Length[2] = NoiseTextureLength * (Vector3.Distance(transform.position, EndPos));
                }
            }
            else
            {
                Laser.SetPosition(0, FirePoint.transform.position);
                player.IsReflectedLaser = false;
                Direction = (player.LaserTarget.transform.position) - FirePoint.transform.position;
                Laser.SetPosition(1, player.LaserTarget.transform.position + Direction);
                player.Death();
            }
        }
        else
        {
            player.IsReflectedLaser = false;
        }
    }

    IEnumerator LaserShoot()
    {
        laserState = LaserState.Preparation;
        animator.Play("Focus");
        PreparationParticle.SetActive(true);

        yield return new WaitForSeconds(preparationTime);
        laserState = LaserState.Aiming;
        player.ReflectedLaserCanHit = false;
        HitEffect.SetActive(true);
        Laser.enabled = true;
        float t = 0;
        while (t < aimingTime)
        {
            float y = Mathf.Lerp(1, 0f, t / aimingTime);
            offSet.Set(0, y * retreat, 0);
            yield return null;
            t += Time.deltaTime;
        }
        laserState = LaserState.Shooting;
        player.ReflectedLaserCanHit = true;
        Laser.enabled = true;

        HitEffect.SetActive(false);
        yield return new WaitForSeconds(shootingTime);
        animator.SetTrigger("LaserShootEnd");
        PreparationParticle.SetActive(false);
        laserState = LaserState.Cooldowm;

        HitEffect.SetActive(false);
        Laser.enabled = false;
    }

    void RotateToMouseDirection(GameObject obj, Vector3 destination)
    {
        Vector3 direction = destination - obj.transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        //obj.transform.localRotation = rotation;
        obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, rotation, 1);
    }

    public override void Activate()
    {
        State = EnemyState.Alive;

        animator.enabled = true;

    }

    public override void Deactivate()
    {
        State = EnemyState.Diactivate;

        StartCoroutine(AnimatorDeactivate());
    }

    public override void TakeDamage()
    {

        animator.enabled = true;
        HP -= 1f;

        puppet.mode = PuppetMaster.Mode.Active;
        if (State == EnemyState.Alive)
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
        if (Player.GetComponent<Character>().enemiesQueue.Contains(this))
        {
            Player.GetComponent<Character>().enemiesQueue.Remove(this);
        }




       
        Canvas.SetActive(false);
        laserState = LaserState.Cooldowm;
        Laser.enabled = false;
        player.IsReflectedLaser = false;
        controlPoint.CheckPoint();


        State = EnemyState.Dead;
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
