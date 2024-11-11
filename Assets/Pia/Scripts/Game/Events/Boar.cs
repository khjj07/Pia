using System;
using System.Globalization;
using Default.Scripts.Sound;
using DG.Tweening;
using Pia.Scripts.StoryMode;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Assets.Pia.Scripts.Game.Events
{
    public class Boar : EventActor
    {
        public enum State
        {
            Move,
            AttackSprint,
            Attack,
            End,
        }

        [SerializeField] EventPathManager pathManager;
        [SerializeField] EventPathNode attackPointNode;

        public float eventTime = 18.0f;
        private float speed = 1;
        [SerializeField] private float sprintSpeed = 2;
        [SerializeField] private float tolerance = 0.1f;
        [SerializeField] private float delay = 2f;

        [SerializeField] private CanvasGroup boarUI;
        [SerializeField] private RectTransform boarPositionUI;
        [SerializeField] private RectTransform boarDirectionArrowOrigin;
        [SerializeField] private RectTransform boarDirectionArrow;
        [SerializeField] private TMP_Text boarDistanceText;

        private bool _attackFlag = false;
        private bool _finishFlag = false;

        public void Start()
        {
            speed = pathManager.GetTotalDistance() / eventTime;
        }
        public void TriggerAttackFlag()
        {
            _attackFlag = true;
        }

        public State Evaluate()
        {
            if (_attackFlag)
            {
                return State.AttackSprint;
            }
            else
            {
                if (pathManager.GetNext())
                {
                    return State.Move;
                }
                else
                {
                    return State.End;
                }
            }
            
        }

        public void FollowingPath()
        {
            var next = pathManager.GetNext();
            if (next)
            {
                Move(GetDirection(next));
            }
        }

        public void Move(Vector3 direction)
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
        public void Sprint(Vector3 direction)
        {
            transform.Translate(direction * sprintSpeed * Time.deltaTime);
        }
        public Vector3 GetDirection(EventPathNode node)
       {
           return Vector3.Normalize(node.transform.position - transform.position);
       }

       public override void Act()
       {
           var player = StoryModeManager.Instance.GetPlayer();

           boarUI.gameObject.SetActive(true);
           boarUI.DOFade(1,2.0f);
           SoundManager.Play("event_startBoar", 6);
            this.UpdateAsObservable()
               .SkipUntil(Observable.Timer(TimeSpan.FromSeconds(delay)))
               .Where(_ => !player.IsCrouch() || player.IsLightOn()).Take(1)
               .Subscribe(_=>TriggerAttackFlag());

           var boarStream = this.UpdateAsObservable()
               .Select(_ => Evaluate())
               .TakeWhile(x => x != State.End);

           boarStream.Where(x => x == State.AttackSprint).Take(1)
               .Subscribe(_ => SoundManager.Play("boar_death", 7))
               .AddTo(gameObject);

          boarStream.Subscribe(_ =>
          {
              var direction = Vector3.Normalize(transform.position - player.transform.position);
              var distance = Vector3.Distance(transform.position , player.transform.position);

              if (Math.Abs(direction.x / direction.z) >= 1920.0f / 1080.0f)
              {
                 boarPositionUI.anchoredPosition = Vector2.Lerp(boarPositionUI.anchoredPosition,new Vector2(800*Math.Sign(direction.x), direction.z * 400), Time.deltaTime);
              }
              else
              {
                  boarPositionUI.anchoredPosition = Vector2.Lerp(boarPositionUI.anchoredPosition, new Vector2(direction.x * 800, 400 * Math.Sign(direction.z)), Time.deltaTime);
              }


              float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
              boarDirectionArrowOrigin.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));

              boarDistanceText.SetText(distance.ToString("F2") + "m");
          },null, () =>
          {
              SoundManager.Stop(6);
              boarUI.DOFade(0, 2.0f);
              //boarUI.gameObject.SetActive(false);
          }).AddTo(gameObject);

            boarStream.Subscribe(x =>
               {
                   switch (x)
                   {
                       case State.Move:
                           pathManager.UpdateCurrentNode(transform.position);
                           FollowingPath();
                           break;
                       case State.AttackSprint:
                       {
                           if (GetDistance(attackPointNode) > tolerance)
                           {
                               Sprint(GetDirection(attackPointNode));
                           }
                           else if(!_finishFlag)
                           {
                               _finishFlag=true;
                               StoryModeManager.GameOver(StoryModeManager.GameOverType.Boar);
                           }
                       }
                          
                           break;
                   }
               }).AddTo(gameObject);

        }

       private float GetDistance(EventPathNode eventPathNode)
       {
          return Vector3.Distance(eventPathNode.transform.position, transform.position);
       }
    }
}