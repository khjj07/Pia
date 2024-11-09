using System;
using Assets.Pia.Scripts.Game;
using Assets.Pia.Scripts.Game.Items;
using Assets.Pia.Scripts.Interface;
using Assets.Pia.Scripts.StoryMode;
using Default.Scripts.Sound;
using Default.Scripts.Util;
using DG.Tweening;
using UniRx;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Unit = UniRx.Unit;

public class Spring : InteractableClass
{
    public float progress = 0;
    [SerializeField] private float extent = 0.1f;

    [SerializeField] private float decreaseInterval = 0.1f;
    [SerializeField] private float decreaseExtent = 0.02f;

 
    [SerializeField] private Image springUIImage;
    [SerializeField] private Image springProgressImage;
    [SerializeField] private Mesh brokenModel;

    private IDisposable decreaseStream;

    public override void OnHover(Item item)
    {

        if (item is Nipper)
        {
            availableOutline.gameObject.SetActive(true);
        }
        else
        {
            inavailableOutline.gameObject.SetActive(true);
        }
    }

    public void Initialize()
    {
        springUIImage.gameObject.SetActive(true);
        springProgressImage.fillAmount = 0;
        springUIImage.rectTransform.anchoredPosition = Input.mousePosition;
        decreaseStream = Observable.Interval(TimeSpan.FromSeconds(decreaseInterval))
            .TakeWhile(_ => progress < 1)
            .Subscribe(_ => SetProgress(progress - decreaseExtent));
    }

    public void Cancel()
    {
        springUIImage.gameObject.SetActive(false);
        decreaseStream.Dispose();
        progress = 0;
    }

    public void Finish()
    {
        springUIImage.gameObject.SetActive(false);
        GetComponent<MeshFilter>().sharedMesh = brokenModel;
        DOTween.To(() => GetComponent<MeshRenderer>().material.GetFloat("_Alpha"),
            x => GetComponent<MeshRenderer>().material.SetFloat("_Alpha", x), 0, 1).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
        isDead = true;
        _isAvailable = false;
        SoundManager.Play("use_SpringCut", 1);
    }

    public void TryToCut()
    {
        transform.DOShakePosition(0.1f, 0.1f);
        SetProgress(progress + extent);
        SoundManager.Play("use_SpringCut", 1);
    }
    public void SetProgress(float value)
    {
        progress = Mathf.Clamp(value, 0, 1);
        springProgressImage.DOKill();
        springProgressImage.DOFillAmount(progress, 0.1f);
    }
}
