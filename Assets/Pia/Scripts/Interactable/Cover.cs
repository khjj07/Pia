using System;
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
    public override void OnInteract(object direction)
    {
        Debug.Log((Vector3)direction);
        if (Vector3.Dot(standardDirection, (Vector3)direction) > 0.5f)
        {
            Throw((Vector3)direction);
        }
    }

    public override void OnHover(Player.UpperState state)
    {
        if (Player.UpperState.None == state)
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
