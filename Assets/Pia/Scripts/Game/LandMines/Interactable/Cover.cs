using System;
using Assets.Pia.Scripts.Game;
using Assets.Pia.Scripts.Game.Items;
using Assets.Pia.Scripts.Interface;
using Assets.Pia.Scripts.StoryMode;
using UniRx;
using UnityEngine;

public class Cover : InteractableClass
{
    private Rigidbody _rigidbody;

    [SerializeField] private Vector3 standardDirection;
    [SerializeField] private float force = 2.0f;

    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    public void TryThrow(object direction)
    {
        Debug.Log((Vector3)direction);
        if (Vector3.Dot(standardDirection, (Vector3)direction) > 0.5f)
        {
            Throw((Vector3)direction+Vector3.up);
        }
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

    private void Throw(Vector3 direction)
    {
        isDead = true;
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        _rigidbody.AddForce(direction * force, ForceMode.Impulse);
    }
}
