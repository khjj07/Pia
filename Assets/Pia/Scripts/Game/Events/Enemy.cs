using System;
using Default.Scripts.Sound;
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
        [SerializeField] private float speed = 1;
        [SerializeField] private float sprintSpeed = 2;
        [SerializeField] private float tolerance = 0.1f;
        [SerializeField] private float delay = 2f;

        [SerializeField] private RectTransform enemyUI;
        [SerializeField] private RectTransform enemyPositionUI;
        [SerializeField] private RectTransform enemyDirectionArrowOrigin;
        [SerializeField] private RectTransform enemyDirectionArrow;
        [SerializeField] private TMP_Text enemyDistanceText;
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
                enemyPositionUI.anchoredPosition = new Vector2(direction.x * 800, direction.z * 400);

                float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
                enemyDirectionArrowOrigin.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));

                enemyDistanceText.SetText(distance.ToString("F2") + "m");
            }, null, () =>
            {
                SoundManager.Stop(6);
                SoundManager.Stop(7);
                enemyUI.gameObject.SetActive(false);
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