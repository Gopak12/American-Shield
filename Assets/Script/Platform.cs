using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public bool resixe;
    public Transform[] Boreders;
    public Transform[] SideBoreders;
    public float widht;
    public float height;
    public float length;
    public void Resize()
    {
        float parentWight = transform.localScale.x;
        float parentHeight = transform.localScale.y;
        float parentLength = transform.localScale.z;
        for (int i = 0; i < SideBoreders.Length; i++)
        {
            SideBoreders[i].transform.localScale = new Vector3(widht/ parentWight, height / parentHeight, length);
        }
        for (int i = 0; i < Boreders.Length; i++)
        {
            Boreders[i].transform.localScale = new Vector3(length, height / parentHeight, widht / parentLength);
        }
    }


    private void OnValidate()
    {
        widht += 0;
        Resize();
    }


}