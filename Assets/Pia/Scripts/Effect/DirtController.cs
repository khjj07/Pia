using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtController : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float holeDepth;

    private MeshRenderer _dirt;

    public Transform top;
    public Transform bottom;
    public float worldHeight;
    private float initialDirtHeight;
    public void Start()
    {
        _dirt = GetComponent<MeshRenderer>();
        holeDepth =  _dirt.sharedMaterial.GetFloat("_HoleDepth" );
        worldHeight = top.position.y - bottom.position.y;
        initialDirtHeight= holeDepth;
    }

    public void Update()
    {
        top.position = bottom.position + Vector3.down * worldHeight * holeDepth; 
        _dirt.material.SetFloat("_HoleDepth", holeDepth);
    }
}
