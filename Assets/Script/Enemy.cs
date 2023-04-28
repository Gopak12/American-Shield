using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum EnemyState { Diactivate, Alive, Unconscious, Dead }

public class Enemy : MonoBehaviour
{


    public EnemyState State;
    public ControlPoint controlPoint;
    public GameObject Player;

    public float Rotspeed;
    public float HP = 1;
    public float time;

    public SkinnedMeshRenderer mesh;
    public Material MaterialAlive;
    public Material MaterialDied;
    public Material MaterialHighlight;
    public Material MaterialMarkHighlight;
    public Animator animator;
    public PuppetMaster puppet;
    public GameObject Canvas;
    public Image Mark;
    public float MarkProgress;
    public bool IsMark;

    public virtual void Activate()
    {

    }

    public virtual void TakeDamage()
    {

    }

    public virtual void Deactivate()
    {
    }
    public virtual void Death()
    {

    }

    public virtual bool ShieldCheck()
    {
        return false;
    }
    public virtual void Selected()
    {
        if (State == EnemyState.Alive && !IsMark)
        {
            mesh.material = MaterialHighlight;
        }
    }

    public virtual void Deselected()
    {
        if (State == EnemyState.Alive && !IsMark)
        {
            mesh.material = MaterialAlive;
            MarkProgress = 0;
            Mark.transform.localScale = Vector3.one;
            Canvas.SetActive(false);
        }
    }

    public virtual void AddMarkProgress()
    {
        if (IsMark) return;
        MarkProgress += 2 * Time.deltaTime;
        Canvas.SetActive(true);
        Mark.transform.Rotate(new Vector3(0, 0, Rotspeed));
        Mark.transform.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one * 0.95f, MarkProgress);

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

    protected IEnumerator AnimatorDeactivate()
    {
        yield return new WaitForSeconds(1f);
        if (State == EnemyState.Diactivate) animator.enabled = false;
    }
}
