using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtController : MonoBehaviour
{
    [Range(0.1f,2.0f)]
    public float dirtHeight;

    private MeshRenderer _dirt;

    public Transform top;
    public Transform bottom;
    public float worldHeight;
    private float initialDirtHeight;
    public void Start()
    {
        _dirt = GetComponent<MeshRenderer>();
        dirtHeight =  _dirt.sharedMaterial.GetFloat("_Height" );
        worldHeight = top.position.y - bottom.position.y;
        initialDirtHeight=dirtHeight;
    }

    public void Update()
    {
        top.position = bottom.position + Vector3.up * worldHeight * dirtHeight/initialDirtHeight; 
        _dirt.material.SetFloat("_Height",dirtHeight);
    }
}
