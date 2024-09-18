using System;
using Assets.Pia.Scripts.Game;
using Assets.Pia.Scripts.Game.Items;
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
    public override void OnHover(Item item)
    {
        if (isPressed)
        {
            pressedOutline.gameObject.SetActive(true);
        }
        else
        {
            pressedOutline.gameObject.SetActive(false);
            if (item is Dagger)
            {
                availableOutline.gameObject.SetActive(true);
            }
            else
            {
                inavailableOutline.gameObject.SetActive(true);
            }
        }
        
    }
    public void Press()
    {
        isPressed = true;
    }

    public void StopPress()
    {
        pressedOutline.gameObject.SetActive(false);
        isPressed = false;
    }
}
