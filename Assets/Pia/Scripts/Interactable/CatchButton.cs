using System;
using Assets.Pia.Scripts.Interface;
using Assets.Pia.Scripts.StoryMode;
using Assets.Pia.Scripts.UI;
using Default.Scripts.Util;
using DG.Tweening;
using UniRx;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CatchButton : InteractableClass
{
    public OutlineController pressedOutline;
    public bool isPressed;
    public override void OnHover(Player.UpperState state)
    {
        if (isPressed)
        {
            pressedOutline.gameObject.SetActive(true);
        }
        else
        {
            pressedOutline.gameObject.SetActive(false);
            if (Player.UpperState.Dagger == state)
            {
                availableOutline.gameObject.SetActive(true);
            }
            else
            {
                inavailableOutline.gameObject.SetActive(true);
            }
        }
        
    }
    public override void OnInteract(object player)
    {
        var p = (Player)player;
        var daggerKeyReleasedStream = GlobalInputBinder.CreateGetKeyUpStream(p.daggerKey);
        var cartridgeCloseStream = GlobalInputBinder.CreateGetKeyUpStream(p.cartridgeKey);
        var cancelStream = daggerKeyReleasedStream.Amb(cartridgeCloseStream);
        cancelStream.First().Subscribe(_ =>
        {
            pressedOutline.gameObject.SetActive(false);
            isPressed = false;
        }).AddTo(gameObject);
        isPressed = true;
    }
}
