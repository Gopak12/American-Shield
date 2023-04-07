using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Character : MonoBehaviour
{

    public float speed;
    public float ShieldRotSpeed;
    public float RotSpeed;
    public float SlowMotionTime;
    public float SlowMotionForce;
    public Vector3 targetPos;
    public Transform cam;
    public Transform Hand;
    public Transform LaserTarget;
    public Transform ReflectedLaserStartPos;
    public GameObject ReflectedLaserHitEffect;
    public Joystick joystick;
    public Animator HandAnimator;
    public GameObject SpeedParticle;
    public Shield shield;
    public LineRenderer ReflectedLaser;
    public bool ReflectedLaserCanHit;
    public LayerMask EnemyMarkLayer;
    public LayerMask ReflectedLaserLayer;
    [Range(0, 1)] public float shieldReadyCof;
    public float CameraSpeed;
    public float CameraDamping;
    public float CameraMinSensivity;
    public float angle;
    public bool InPoint;
    public bool InJumping;
    public bool IsReflectedLaser;
    public List<Enemy> enemiesQueue;
    ControlPoint CurrentPoint;
    Enemy CurentSelectedEnemy;
    Camera _camera;
    Quaternion HandStartRot;
    Vector3 HandStartPos;

    Vector3 previousPosition;

    Transform targRot;

    private void Start()
    {
        shield = GetComponentInChildren<Shield>();
        HandStartRot = Hand.localRotation;
        HandStartPos = Hand.localPosition;
        _camera = Camera.main;
        ReflectedLaserHitEffect.SetActive(false);

        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
        targRot = new GameObject().transform;
    }

    private void LateUpdate()
    {

        if (Input.GetMouseButtonDown(0))
        {
            previousPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 direction = previousPosition - Camera.main.ScreenToViewportPoint(Input.mousePosition);


            targRot.Rotate(new Vector3(direction.y * RotSpeed, 0, 0));
            targRot.Rotate(new Vector3(0, -direction.x * RotSpeed, 0), Space.World);
            targRot.localRotation = Quaternion.Euler(ClampAngle(targRot.rotation.eulerAngles.x, -angle, angle), ClampAngle(targRot.rotation.eulerAngles.y, -angle, angle), 0f);



            previousPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }
        cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, targRot.localRotation, CameraDamping);
    }

    void Update()
    {
        if (!InPoint)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, speed * Time.deltaTime);
            SpeedParticle.SetActive(true);
        }
        else { SpeedParticle.SetActive(false); }




        if (Input.GetMouseButton(0) && shield.state == Shield.ShieldState.InHand)
        {

            shieldReadyCof = Mathf.Clamp01(shieldReadyCof + (ShieldRotSpeed * Time.deltaTime));
            _camera.fieldOfView = Mathf.Lerp(60, 48, shieldReadyCof);
            Hand.transform.localRotation = Quaternion.Lerp(HandStartRot, Quaternion.Euler(0, 0, 0), shieldReadyCof);
            Hand.transform.localPosition = Vector3.Lerp(HandStartPos, Vector3.zero, shieldReadyCof);

            Ray ray = new Ray(cam.position, cam.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 40, EnemyMarkLayer.value))
            {
                Enemy selectebleEnemy = hit.collider.gameObject.GetComponentInParent<Enemy>();
                if (selectebleEnemy)
                {
                    if (CurentSelectedEnemy && CurentSelectedEnemy != selectebleEnemy)
                    {
                        CurentSelectedEnemy.Deselected();
                        CurentSelectedEnemy = null;
                    }
                    if (selectebleEnemy.State == EnemyState.Alive)
                    {
                        CurentSelectedEnemy = selectebleEnemy;
                        selectebleEnemy.Selected();
                        selectebleEnemy.AddMarkProgress();
                    }
                }
                else
                {
                    if (CurentSelectedEnemy && CurentSelectedEnemy != selectebleEnemy)
                    {
                        CurentSelectedEnemy.Deselected();
                        CurentSelectedEnemy = null;
                    }

                }
            }
            else
            {
                if (CurentSelectedEnemy)
                {
                    CurentSelectedEnemy.Deselected();
                    CurentSelectedEnemy = null;
                }

            }
        }
        else
        {
            shieldReadyCof = Mathf.Clamp01(shieldReadyCof - (ShieldRotSpeed * Time.deltaTime));
            Hand.transform.localRotation = Quaternion.Lerp(HandStartRot, Quaternion.Euler(0, 0, 0), shieldReadyCof);
            Hand.transform.localPosition = Vector3.Lerp(HandStartPos, Vector3.zero, shieldReadyCof);
            _camera.fieldOfView = 60;
            if (CurentSelectedEnemy)
            {
                CurentSelectedEnemy.Deselected();
                CurentSelectedEnemy = null;
            }
        }

        if (IsReflectedLaser)
        {
            ReflectedLaser.enabled = true;
            ReflectedLaser.SetPosition(0, LaserTarget.transform.position);

            
            Ray ray = new Ray(cam.position, cam.forward);
            RaycastHit hit;
            ReflectedLaserHitEffect.SetActive(true);
            if (Physics.Raycast(ray, out hit, 40, ReflectedLaserLayer))
            {
                ReflectedLaser.SetPosition(1, hit.point);
                ReflectedLaserHitEffect.transform.position = hit.point;
                Enemy selectebleEnemy = hit.collider.gameObject.GetComponentInParent<Enemy>();
                if (selectebleEnemy && ReflectedLaserCanHit)
                {
                    selectebleEnemy.TakeDamage();
                }
            }
            else
            {
                if (Physics.Raycast(ray, out hit, 40, ReflectedLaserLayer))
                {
                    ReflectedLaser.SetPosition(1, hit.point);
                    ReflectedLaserHitEffect.transform.position = hit.point;
                }
                else
                {
                    ReflectedLaserHitEffect.SetActive(false);
                    ReflectedLaser.SetPosition(1, cam.transform.position + cam.forward * 40);
                    ReflectedLaserHitEffect.transform.position = cam.transform.position + cam.forward * 40;
                }
            }
        }
        else
        {
            ReflectedLaser.enabled = false;
            ReflectedLaserHitEffect.SetActive(false);
        }



        if (Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }



    private float ClampAngle(float angle, float min, float max)
    {
        do
        {
            if (angle < -360) angle += 360;
            if (angle > 180) angle -= 360;
        } while (angle < -360 || angle > 360);
        return Mathf.Clamp(angle, min, max);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ControlPoint>() && other.GetComponent<ControlPoint>().PointActive)
        {
            SetPoint(other.GetComponent<ControlPoint>());
            other.GetComponent<ControlPoint>().PointActivation();
            other.GetComponent<ControlPoint>().player = this;
        }

        if (other.GetComponent<JumpPoint>() && !InJumping)
        {
            InPoint = true;
            StartCoroutine(Jump(other.GetComponent<JumpPoint>().JumpTargetPoint, other.GetComponent<JumpPoint>().height, other.GetComponent<JumpPoint>().speed));
        }
        if (other.GetComponent<FinishPoint>())
        {
            InPoint = true;
            other.GetComponent<FinishPoint>().ParticleActivate();
        }
    }

    IEnumerator Jump(Transform pos, float jumpHeight, float jumpSpeed)
    {
        InJumping = true;
        Vector3 StartPos = transform.position;
        Vector3 A = new Vector3((transform.position + cam.position).x / 2, (transform.position + cam.position).y / 2 + jumpHeight, (transform.position + cam.position).z / 2);
        Vector3 B;
        Vector3 C;
        Quaternion Rotat = transform.rotation;
        //ShieldModel.rotation;
        for (float t = 0; t < 1;)
        {


            t = Mathf.MoveTowards(t, 1, jumpSpeed * Time.deltaTime);
            B = Vector3.Lerp(StartPos, A, t);
            C = Vector3.Lerp(A, pos.position, t);
            transform.position = Vector3.Lerp(B, C, t);
            transform.rotation = Quaternion.Lerp(Rotat, pos.rotation, t);
            //ShieldModel.rotation = Quaternion.Lerp(Rotat, Quaternion.Euler(270f, 350f, 340f),t*2-1);
            yield return null;
        }
        InJumping = false;
        InPoint = false;

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Bullet>())
        {
            if (Input.GetMouseButton(0) && shieldReadyCof < 0.2f)
            {
                if (shield.state == Shield.ShieldState.InHand && other.GetComponent<Bullet>().ParentEnemy.State == EnemyState.Alive)
                {
                    other.GetComponent<Bullet>().targetPos = other.GetComponent<Bullet>().ParentEnemy.transform;
                    other.GetComponent<Bullet>().reflected = true;
                    other.GetComponent<Bullet>().speed *= 1.4f;
                }
                else
                {
                    Destroy(other.gameObject);
                }
            }
            else if (Input.GetMouseButton(0) && shieldReadyCof > 0.2f && !other.GetComponent<Bullet>().reflected)
            {
                HandAnimator.Play("ShieldShake");
                other.GetComponent<Bullet>().blockedVector = other.transform.position + Vector3.forward * 100 + Vector3.up * Random.Range(30, 50) + Vector3.left * Random.Range(-100, 100);
                other.GetComponent<Bullet>().blocked = true;
                other.GetComponent<Bullet>().speed *= 1.4f;
                Destroy(other.gameObject, 2f);
            }
        }
    }

    public void Death()
    {
        Debug.Log("Player die");
    }

    public void SetPoint(ControlPoint controlPoint)
    {
        CurrentPoint = controlPoint;
        InPoint = true;
    }

    public void CheckPoint(ControlPoint controlPoint)
    {
        if (controlPoint == CurrentPoint)
        {
            InPoint = false;
        }
    }

}
