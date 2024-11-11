using System;
using Default.Scripts.Sound;
using DG.Tweening;
using Pia.Scripts.StoryMode;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Assets.Pia.Scripts.Game.Events
{
    public class Enemy : EventActor
    {
        public enum State
        {
            Move,
            End,
        }
        [SerializeField] EventPathManager pathManager;
        private float speed = 1;
        public float eventTime = 18.0f;
        [SerializeField] private float tolerance = 0.1f;
        [SerializeField] private float delay = 2f;

        [SerializeField] private CanvasGroup enemyUI;
        [SerializeField] private RectTransform enemyPositionUI;
        [SerializeField] private RectTransform enemyDirectionArrowOrigin;
        [SerializeField] private RectTransform enemyDirectionArrow;
        [SerializeField] private TMP_Text enemyDistanceText;

        public void Start()
        {
            speed = pathManager.GetTotalDistance() / eventTime;
        }
        public State Evaluate()
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
        public Vector3 GetDirection(EventPathNode node)
        {
            return Vector3.Normalize(node.transform.position - transform.position);
        }
        public override void Act()
        {
            var player = StoryModeManager.Instance.GetPlayer();
            enemyUI.gameObject.SetActive(true);
            enemyUI.DOFade(1,2.0f);

            SoundManager.Play("event_enemyStart", 6);
            SoundManager.Play("event_enemy", 7);
            var enemyStream = this.UpdateAsObservable()
                .Select(_ => Evaluate())
                .TakeWhile(x => x != State.End);

            enemyStream.SkipUntil(Observable.Timer(TimeSpan.FromSeconds(delay)))
                .Where(_ => !player.IsCrouch() || player.IsLightOn()).Take(1)
                .Subscribe(_ =>
                {
                    StoryModeManager.GameOver(StoryModeManager.GameOverType.Enemy);
                });

            enemyStream.Subscribe(_ =>
            {
                var direction = Vector3.Normalize(transform.position - player.transform.position);
                var distance = Vector3.Distance(transform.position, player.transform.position);

                if (Math.Abs(direction.x / direction.z) >= 1920.0f / 1080.0f)
                {
                    enemyPositionUI.anchoredPosition = Vector2.Lerp(enemyPositionUI.anchoredPosition, new Vector2(800 * Math.Sign(direction.x), direction.z * 400), Time.deltaTime);
                }
                else
                {
                    enemyPositionUI.anchoredPosition = Vector2.Lerp(enemyPositionUI.anchoredPosition, new Vector2(direction.x * 800, 400 * Math.Sign(direction.z)), Time.deltaTime);
                }

                float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
                enemyDirectionArrowOrigin.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));

                enemyDistanceText.SetText(distance.ToString("F2") + "m");
            }, null, () =>
            {
                SoundManager.Stop(6);
                SoundManager.Stop(7);
                enemyUI.DOFade(0, 2.0f);
            }).AddTo(gameObject);
            enemyStream.Subscribe(x =>
            {
                switch (x)
                {
                    case State.Move:
                        pathManager.UpdateCurrentNode(transform.position);
                        FollowingPath();
                        break;
                }
            }).AddTo(gameObject);
        }
    }
}