using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Character : MonoBehaviour
{

    public float speed;
    public float ShieldRotSpeed;
    public Vector3 targetPos;
    public Transform cam;
    public Transform Hand;
    public Joystick joystick;
    public Animator HandAnimator;
    public GameObject SpeedParticle;
    public Shield shield;
    public LayerMask layer;
    [Range(0, 1)] public float shieldReadyCof;
    public float CameraSpeed;
    public float CameraDamping;
    public float CameraMinSensivity;
    public float angle;
    public bool InPoint;
    public bool InJumping;
    Enemy CurentSelectedEnemy;
    Camera _camera;
    public Vector2 JD;
    public Vector2 CJD;
    Quaternion HandStartRot;
    Vector3 HandStartPos;
    public List<Enemy> enemiesQueue;

    private void Start()
    {
        shield = GetComponentInChildren<Shield>();
        HandStartRot = Hand.localRotation;
        HandStartPos = Hand.localPosition;
        _camera = Camera.main;
    }

    void Update()
    {
        if (!InPoint)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, speed * Time.deltaTime);
            SpeedParticle.SetActive(true);
        }
        else { SpeedParticle.SetActive(false); }

        
        if (joystick.Direction != Vector2.zero)
        {
            Vector3 rot = transform.rotation.eulerAngles;

            cam.localRotation = Quaternion.Lerp(cam.localRotation, Quaternion.Euler(Mathf.Clamp( -(CameraSpeed * (CJD.y + joystick.Direction.y)), -angle, angle), Mathf.Clamp( (CameraSpeed * (CJD.x + joystick.Direction.x)), -angle, angle), 0), CameraDamping * Time.deltaTime);
            //float CurrentX = joystick.Direction.x * CameraSpeed;
            //float CurrentY = joystick.Direction.y*CameraSpeed/2;
            //CurrentX = Mathf.Clamp( CurrentX, -angle, angle);
            //CurrentY = Mathf.Clamp(CurrentY, -angle, angle);

            /*if (new Vector2(CurrentX, CurrentY).magnitude > CameraMinSensivity)
            {

                Vector3 rot = transform.rotation.eulerAngles;
                //cam.localRotation = Quaternion.Lerp(cam.localRotation, Quaternion.Euler(ClampAngle(cam.localRotation.eulerAngles.x - CurrentY, -angle, angle), ClampAngle(cam.localRotation.eulerAngles.y + CurrentX, -angle, angle), 0), CameraDamping * Time.deltaTime);
                cam.localRotation = Quaternion.Euler(ClampAngle(cam.localRotation.eulerAngles.x - CurrentY, -angle, angle), ClampAngle(cam.localRotation.eulerAngles.y + CurrentX, -angle, angle), 0);
                //cam.localRotation = Quaternion.Lerp(cam.localRotation, Quaternion.Euler(Mathf.Clamp(-(CameraSpeed * joystick.Direction.y), -angle, angle), Mathf.Clamp((CameraSpeed * joystick.Direction.x), -angle, angle), 0), CameraDamping * Time.deltaTime);
            }*/
        }
        else
        {
           // cam.localRotation = Quaternion.Lerp(cam.localRotation, Quaternion.Euler(0,0, 0), CameraDamping/50 * Time.deltaTime);
        }

        


        if (Input.GetMouseButton(0) && shield.state == Shield.ShieldState.InHand)
        {

            shieldReadyCof = Mathf.Clamp01(shieldReadyCof + (ShieldRotSpeed * Time.deltaTime));
            _camera.fieldOfView = Mathf.Lerp(60, 48, shieldReadyCof);
            Hand.transform.localRotation = Quaternion.Lerp(HandStartRot,Quaternion.Euler(0, 0, 0), shieldReadyCof);
            Hand.transform.localPosition = Vector3.Lerp(HandStartPos, Vector3.zero, shieldReadyCof);

            Ray ray = new Ray(cam.position, cam.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, layer.value))
            {
                Enemy selectebleEnemy = hit.collider.gameObject.GetComponent<Enemy>();
                if (selectebleEnemy)
                {
                    if (CurentSelectedEnemy && CurentSelectedEnemy != selectebleEnemy)
                    {
                        CurentSelectedEnemy.Deselected();
                        CurentSelectedEnemy = null;
                    }
                    CurentSelectedEnemy = selectebleEnemy;
                    hit.collider.gameObject.GetComponent<Enemy>().Selected();
                    hit.collider.gameObject.GetComponent<Enemy>().AddMarkProgress();
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
            InPoint = true;
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

    IEnumerator Jump(Transform pos,float jumpHeight,float jumpSpeed)
    {
        InJumping = true;
        Vector3 StartPos = transform.position;
        Vector3 A = new Vector3((transform.position + cam.position).x / 2 , (transform.position + cam.position).y / 2 + jumpHeight, (transform.position + cam.position).z / 2);
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
                if (shield.state == Shield.ShieldState.InHand && other.GetComponent<Bullet>().ParentEnemy.EnemyActive)
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
                other.GetComponent<Bullet>().reflected = true;
                other.GetComponent<Bullet>().speed *= 1.4f;
                Destroy(other.gameObject, 2f);
            }
        }
    }

}
