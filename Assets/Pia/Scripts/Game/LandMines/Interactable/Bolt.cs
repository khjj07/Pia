using System;
using Assets.Pia.Scripts.Game;
using Assets.Pia.Scripts.Game.Items;
using Assets.Pia.Scripts.Interface;
using Assets.Pia.Scripts.StoryMode;
using Default.Scripts.Sound;
using DG.Tweening;
using Knife.HDRPOutline.Core;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

public class Bolt : InteractableClass
{
    [SerializeField] private float maxRelativeHeight;
    [SerializeField] private float progress=0;
    [SerializeField] private float extent = 0.1f;
    [SerializeField] private float rotateAngle = 40;

    [Header("사라지기 효과")]
    [SerializeField] private Vector3 throwDirection;
    [SerializeField] private float force = 1.0f;

    private float initialHeight=0;

    private Rigidbody _rigidbody;


    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        initialHeight = transform.localPosition.y;

    }
    public override void OnHover(Item item)
    {
        if (item is Driver)
        {
            availableOutline.gameObject.SetActive(true);
        }
        else
        {
            inavailableOutline.gameObject.SetActive(true);
        }
    }

    public void Screw(float interval)
    {
        if (progress > 0.9f && !isDead)
        {
            Throw();
        }
        else if(!isDead)
        {
            progress = Mathf.Clamp(progress + extent, 0, 1);
            transform.DOLocalMoveY(initialHeight + maxRelativeHeight * progress, interval);
            transform.DOLocalRotate(Vector3.up * rotateAngle, interval).SetRelative();
        }
    }

    public void Throw()
    {
        transform.DOKill();
        isDead = true;
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        _rigidbody.AddForce(throwDirection*force,ForceMode.Impulse);
        _rigidbody.AddTorque(throwDirection*force,ForceMode.Impulse);
    }

}
