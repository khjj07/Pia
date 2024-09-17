using System;
using Assets.Pia.Scripts.Interface;
using Assets.Pia.Scripts.StoryMode;
using Default.Scripts.Util;
using DG.Tweening;
using UniRx;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Spring : InteractableClass
{
    [SerializeField] private float progress = 0;
    [SerializeField] private float extent = 0.1f;

    [SerializeField] private float decreaseInterval = 0.1f;
    [SerializeField] private float decreaseExtent = 0.02f;

    [SerializeField] private Canvas springCanvas;
    [SerializeField] private Image springUIImage;
    [SerializeField] private Image springProgressImage;
    [SerializeField] private Mesh brokenModel;
    public override void OnHover(Player.UpperState state)
    {
        if (Player.UpperState.Nipper == state)
        {
            availableOutline.gameObject.SetActive(true);
        }
        else
        {
            inavailableOutline.gameObject.SetActive(true);
        }
    }
    public override void OnInteract(object player)
    {
        var p = (Player)player;

        var nipperKeyReleasedStream = GlobalInputBinder.CreateGetKeyUpStream(p.nipperKey);
        var cartridgeCloseStream = GlobalInputBinder.CreateGetKeyUpStream(p.cartridgeKey);
        var cancelStream = nipperKeyReleasedStream.Amb(cartridgeCloseStream);

        springCanvas.gameObject.SetActive(true);
        springProgressImage.fillAmount = 0;
        springUIImage.rectTransform.anchoredPosition = Input.mousePosition;
        p.SetCursorLocked();
        GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Mouse0)
            .TakeUntil(cancelStream)
            .TakeWhile(_ => progress < 1)
            .Subscribe(_ =>
            {
                transform.DOShakePosition(0.1f, 0.1f);
                SetProgress(progress + extent);
            },null,()=>
            {
                p.SetCursorUnlocked();
                RemoveCheck();
            });

        Observable.Interval(TimeSpan.FromSeconds(decreaseInterval))
            .TakeUntil(cancelStream)
            .TakeWhile(_ => progress < 1)
            .Subscribe(_ => SetProgress(progress - decreaseExtent));

    }

    private void RemoveCheck()
    {
        if (progress >= 1)
        {
            springCanvas.gameObject.SetActive(false);
            GetComponent<MeshFilter>().sharedMesh = brokenModel;
            DOTween.To(() => GetComponent<MeshRenderer>().material.GetFloat("_Alpha"),
                x => GetComponent<MeshRenderer>().material.SetFloat("_Alpha", x),0, 1).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
            isDead = true;
            _isAvailable = false;
        }
        else
        {
            springCanvas.gameObject.SetActive(false);
            progress = 0;
        }
       
    }

    public void SetProgress(float value)
    {
        progress = Mathf.Clamp(value, 0, 1);
        springProgressImage.DOKill();
        springProgressImage.DOFillAmount(progress,0.1f);
    }
}
