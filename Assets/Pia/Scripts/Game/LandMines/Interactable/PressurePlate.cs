using System;
using Assets.Pia.Scripts.Game;
using Assets.Pia.Scripts.Game.Items;
using Assets.Pia.Scripts.Interface;
using Assets.Pia.Scripts.StoryMode;
using Default.Scripts.Util;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class PressurePlate : InteractableClass
{
    [SerializeField]
    private CatchButton catchButton;

    [SerializeField] private Canvas pressurePlateCanvas;
    [SerializeField] private Image boundingArea;
    [SerializeField] private Image matchBar;
    private float _currentBoundAngle = 0;
    private float _matchBarAngle = 0;
    private int _currentLevel = 0;
    private int _currentSpeed = 0;

    public int targetCount = 3;
    public int count = 0;

    private Vector3 initialLocalRotation;
    public Transform origin;

    public override void Start()
    {
        base.Start();
        initialLocalRotation = transform.localRotation.eulerAngles;
    }
    public override void OnHover(Item item)
    {
        if (item is Dagger && catchButton.isPressed)
        {
            availableOutline.gameObject.SetActive(true);
        }
        else
        {
            inavailableOutline.gameObject.SetActive(true);
        }

    }


    public void ResetCurrent()
    {
        origin.DOLocalRotateQuaternion(Quaternion.Euler(0, 90 * (float)count / targetCount, 0), 1.0f);
        _currentLevel = 0;
        SetLevel(_currentLevel);
    }

    public void ResetAll()
    {
        pressurePlateCanvas.gameObject.SetActive(false);
        catchButton.isPressed = false;
        catchButton.pressedOutline.gameObject.SetActive(false);
      
        if (!isDead)
        {
            _currentLevel = 0;
            _matchBarAngle = 0;
            count = 0;
            origin.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 0), 1.0f);
        }
        else
        {
            GetComponent<MeshCollider>().enabled=false;
        }
        
    }

    public void Operate()
    {
        if (_currentLevel == 2)
        {
            count++;
            origin.DOLocalRotateQuaternion(Quaternion.Euler(0, 90 * (float)count / targetCount, 0), 1.0f).SetEase(Ease.InExpo);
            if (count == targetCount)
            {
                isDead = true;
                _isAvailable = false;
                EventManager.InvokeEvent(EventManager.Event.AirBomb);
            }
            else
            {
                _currentLevel = 0;
            }
        }
        else
        {
            _currentLevel++;
            origin.DOShakeRotation(0.1f, Vector3.up * 5).SetEase(Ease.OutBack);
        }
        SetLevel(_currentLevel);
    }

    public bool CheckMatchBarInBound()
    {
        if (_currentBoundAngle > _matchBarAngle || 360 - _currentBoundAngle < _matchBarAngle)
        {
            return true;
        }

        return false;
    }

    private void SetMatchBarAngle(float angle)
    {
        _matchBarAngle = angle % 360;
        matchBar.rectTransform.DOLocalRotate(new Vector3(0, 0, _matchBarAngle), 0.01f);
    }

    public void Initialize()
    {
        pressurePlateCanvas.gameObject.SetActive(true);
        _currentLevel = 0;
        _matchBarAngle = 0;
        SetLevel(_currentLevel);
    }

    public void SetLevel(int level)
    {
        switch (level)
        {
            case 0:
                boundingArea.fillAmount = 120f / 360f;
                _currentBoundAngle = 60f;
                _currentSpeed = 3;
                break;
            case 1:
                boundingArea.fillAmount = 80f / 360f;
                _currentBoundAngle = 40f;
                _currentSpeed = 6;
                break;
            case 2:
                boundingArea.fillAmount = 40f / 360f;
                _currentBoundAngle = 20f;
                _currentSpeed = 9;
                break;
        }
        boundingArea.rectTransform.localEulerAngles = new Vector3(0f, 0f, _currentBoundAngle);
    }

    public bool IsFinish()
    {
        return count < targetCount;
    }

    public void MatchBarMove()
    {
        SetMatchBarAngle(_matchBarAngle - _currentSpeed);
    }

    public bool IsMovable()
    {
        return catchButton.isPressed;
    }
}
