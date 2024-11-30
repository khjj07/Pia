using System;
using Assets.Pia.Scripts.Game;
using Assets.Pia.Scripts.Game.Items;
using Assets.Pia.Scripts.Interface;
using Assets.Pia.Scripts.StoryMode;
using Default.Scripts.Sound;
using UniRx;
using UnityEngine;

public class Cover : InteractableClass
{
    private Rigidbody _rigidbody;

    [SerializeField] private Vector3 standardDirection;
    [SerializeField] private float force = 2.0f;
    [SerializeField] private Vector3 throwDirection;
    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
   
    public override void OnHover(Item item)
    {
        if (item == null)
        {
            availableOutline.gameObject.SetActive(true);
        }
        else
        {
            inavailableOutline.gameObject.SetActive(true);
        }
    }

    public void Throw()
    {
        isDead = true;
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        _rigidbody.AddForce(throwDirection * force, ForceMode.Impulse);
        SoundManager.Play("use_coverRemove", 1);
    }
}
