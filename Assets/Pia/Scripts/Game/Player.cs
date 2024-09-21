using System;
using Assets.Pia.Scripts.Game.Items;
using Assets.Pia.Scripts.Interface;
using Assets.Pia.Scripts.StoryMode.Walking;
using Assets.Pia.Scripts.UI;
using DG.Tweening;
using Pia.Scripts.StoryMode;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using GlobalInputBinder = Default.Scripts.Util.GlobalInputBinder;
using Random = UnityEngine.Random;
using Unit = UniRx.Unit;

namespace Assets.Pia.Scripts.Game
{
    public class Player : MonoBehaviour
    {
        public enum LowerAnimationState
        {
            Idle,
            Walk,
            Crouch
        }
        [SerializeField] PathManager pathManager;

        [Header("Å° ¼¼ÆÃ")]
        public KeyCode walkKey = KeyCode.W;
        public KeyCode crouchKey = KeyCode.LeftControl;
        public KeyCode stepKey = KeyCode.S;
        public KeyCode stepPedalKey = KeyCode.F7;

        public Bag bag;

        [Header("Body Part")]
        [SerializeField] private Transform head;
        [SerializeField] private Transform body;
        [SerializeField] private Transform upperBody;
        [SerializeField] private Transform arm;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform foot;
        [SerializeField] private Transform crouchArmTransform1;
        [SerializeField] private Transform crouchArmTransform2;
        [SerializeField] private Transform standArmTransform;
        [SerializeField] private Transform crouchHeadTransform1;
        [SerializeField] private Transform crouchHeadTransform2;
        [SerializeField] private Transform standHeadTransform;

        [Header("Property")]
        [SerializeField] private float sensitiveX = 1.0f;
        [SerializeField] private float sensitiveY = 1.0f;
        [SerializeField] private float defaultFov = 60;

        [Header("   Default")]
        [SerializeField] private float limitRotationY = 0;
        [SerializeField] private float minRotationX = 0;
        [SerializeField] private float maxRotationX = 0;

        [SerializeField] private float walkMinRotationX = 0;
        [SerializeField] private float walkMaxRotationX = 0;

        [SerializeField] private float standUpHeight = 1.0f;
        [SerializeField] private float standUpDuration = 1.0f;

        [SerializeField] private float crouchMinRotationX = 0;
        [SerializeField] private float crouchMaxRotationX = 0;
        [SerializeField] private float crouchLimitRotationY = 0;
        [SerializeField] private float crouchHeight = 1.0f;
        [SerializeField] private float crouchDuration = 1.0f;
        [SerializeField] private float crouchFov = 50;
        [SerializeField] private float speed = 1.0f;


        private bool _isCrouching;
        private bool _isMove = false;
        private bool _isInteractable = false;
        private bool _ableToCrouch = true;

        private Vector3 initialLocalPosition;
        private Quaternion initialLocalRotation;

        private float rotationDirectionX = 0;
        private float rotationX = 0;

        private float rotationDirectionY = 0;
        private float rotationY = 0;

        private float previousRotationX;


        public Subject<LowerAnimationState> lowerStateSubject;
        private LowerAnimationState currentLowerState;

        [Header("Animator")]
        [SerializeField] private Animator lowerAnimator;

        [Header("Dig")]

        [Header("Interaction")]
        [SerializeField] private float checkTargetRayDistance = 1.0f;
        [SerializeField] private PlayerCursor playerCursor;
        public float shovelInterval = 1.0f;
        public float driverInterval = 1.0f;

        [Header("HP")]
        [SerializeField] private RectTransform hpUI;
        [SerializeField] private Image hpBar;
        public int initialHp = 600;
        public float hpDecreaseInterval = 1;
        public int hpReduction = 1;
        private int hp;
        private bool _isBleeding=false;



        public InteractableClass target;
        private UsableItem hand;

        void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            initialLocalPosition = mainCamera.transform.localPosition;
            initialLocalRotation = mainCamera.transform.localRotation;
            mainCamera.fieldOfView = defaultFov;

            SetCursorLocked();
            CreateAnimationSubject();
            CreateLowerBodyStream();
            CreateCameraStream();
            CreateHandStream();
            CreateHoverStream();
            CreateHPStream();
            CreateRandomBleedEvent(0.95f,0.87f);
            CreateRandomBleedEvent(0.65f, 0.50f);
        }

        private void CreateHandStream()
        {
            GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Mouse0)
                .Where(_ => hand == null)
                .Subscribe(_ => UseHand(target))
                .AddTo(gameObject);
        }

        private void CreateRandomBleedEvent(float p0, float p1)
        {
           float random =  Random.Range(p1, p0);
           Observable.Interval(TimeSpan.FromSeconds(hpDecreaseInterval))
                .Where(_ => random * initialHp >= hp).Take(1)
               .Subscribe(_ => Bleed())
               .AddTo(gameObject);
        }

        private void CreateHPStream()
        {
            hp = initialHp;
            Observable.Interval(TimeSpan.FromSeconds(hpDecreaseInterval)).Subscribe(_ => SetHP(hp- hpReduction));
        }

        private void SetHP(int value)
        {
            hp = Math.Max(value,0);
            hpBar.DOFillAmount((float)hp / initialHp, hpDecreaseInterval);
        }
        public bool IsBleeeding()
        {
            return _isBleeding;
        }
        private void Bleed()
        {
            hpReduction = 2;
            _isBleeding = true;
        }

        private void CureBleed()
        {
            hpReduction = 1;
            _isBleeding = false;
        }
        private void CreateHoverStream()
        {
            this.LateUpdateAsObservable()
                .Where(t => _isInteractable)
                .Select(_ => CheckInteractable())
                .Subscribe(t =>
                {
                    if (target != null)
                    {
                        target.OnExit();
                    }
                    if (t != null)
                    {
                        t.OnHover(hand);
                    }
                    target = t;
                }, null, () =>
                {
                    if (target != null)
                    {
                        target.OnExit();
                    }
                    target = null;
                }).AddTo(gameObject);
        }

        private void CreateCameraStream()
        {
            GlobalInputBinder.CreateGetAxisStream("Mouse X").Subscribe(RotateCameraY);
            GlobalInputBinder.CreateGetAxisStream("Mouse Y").Subscribe(RotateCameraX);
            this.UpdateAsObservable().Subscribe(_ => RotateHead());
            this.UpdateAsObservable().Select(_ => currentLowerState)
                .DistinctUntilChanged().Subscribe(ShakeCamera);
        }
        private void ShakeCamera(LowerAnimationState state)
        {
            DOTween.Kill("shakeCamera");
            DOTween.Kill("shakeCameraRot");
            mainCamera.transform.localPosition = initialLocalPosition;
            mainCamera.transform.localRotation = initialLocalRotation;
            if (state == LowerAnimationState.Walk)
            {
                mainCamera.transform.DOShakePosition(0.5f, Vector3.up * 0.1f, 0).SetLoops(-1).SetId("shakeCamera").SetEase(Ease.InOutBounce).Restart();
                mainCamera.transform.DOShakeRotation(1.0f, Vector3.forward, 0).SetLoops(-1).SetId("shakeCameraRot").SetEase(Ease.InOutBounce).Restart();
            }
            else
            {
                mainCamera.transform.DOShakeRotation(2.0f, Vector3.forward, 0).SetLoops(-1).SetId("shakeCameraRot").SetEase(Ease.InOutCirc).Restart();
            }
        }
        private void CreateLowerBodyStream()
        {
            this.LateUpdateAsObservable().Subscribe(_ =>
            {
                if (_isMove)
                {
                    lowerStateSubject.OnNext(LowerAnimationState.Walk);
                }
                else if (_isCrouching)
                {
                    lowerStateSubject.OnNext(LowerAnimationState.Crouch);
                }
                else
                {
                    lowerStateSubject.OnNext(LowerAnimationState.Idle);
                }
                _isMove = false;
            });
            CreateWalkingStream();
            CreateCrouchingStream();
        }
        private void CreateCrouchingStream()
        {
            GlobalInputBinder.CreateGetKeyDownStream(crouchKey)
                .SkipWhile(_ => !StoryModeManager.IsInteractionActive())
                .Where(_ => !_isCrouching && _ableToCrouch).Subscribe(_ => Crouch()).AddTo(gameObject);

            GlobalInputBinder.CreateGetKeyDownStream(crouchKey)
                .SkipWhile(_ => !StoryModeManager.IsInteractionActive())
                .Where(_ => _isCrouching && _ableToCrouch).Subscribe(_ => StandUp()).AddTo(gameObject);
        }
        private void CreateWalkingStream()
        {
            this.UpdateAsObservable()
                .Where(_ => StoryModeManager.GetState() == StoryModeManager.State.Walking)
                .Select(_ => pathManager.CheckNode(this))
                .Subscribe(_ =>
                {
                    DOTween.Kill("changeDirection");
                    transform.DORotate(Quaternion.LookRotation(pathManager.GetPlayerDirection(this)).eulerAngles, 1.0f).SetId("changeDirection");
                });

            GlobalInputBinder.CreateGetKeyStream(walkKey)
                .Where(_ => StoryModeManager.GetState() == StoryModeManager.State.Walking)
                .Where(_ => !_isCrouching && pathManager.PlayerIsMovable())
                .Subscribe(_ =>
                {
                    MoveForward();
                    _isMove = true;
                }).AddTo(gameObject);
        }

        private void RotateHead()
        {
            head.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
            if (!_isCrouching)
            {
                head.localPosition = new Vector3(rotationY * 0.003f, head.localPosition.y, 0.14f + rotationX * 0.003f);
            }

            DOTween.Kill("followHeadPos");
            upperBody.localRotation = head.localRotation;
            upperBody.DOLocalMove(head.localPosition, 0.2f).SetId("followHeadPos"); ;
        }

        private void CreateAnimationSubject()
        {
            lowerStateSubject = new Subject<LowerAnimationState>();
            lowerStateSubject.Subscribe(AnimateLowerBody);
            lowerStateSubject.OnNext(LowerAnimationState.Idle);
        }

        private void AnimateLowerBody(LowerAnimationState state)
        {
            currentLowerState = state;
            lowerAnimator.SetBool("Idle", false);
            lowerAnimator.SetBool("Walk", false);
            lowerAnimator.SetBool("Crouch", false);
            switch (state)
            {
                case LowerAnimationState.Idle:
                    lowerAnimator.SetBool("Idle", true);
                    break;
                case LowerAnimationState.Walk:
                    lowerAnimator.SetBool("Walk", true);
                    break;
                case LowerAnimationState.Crouch:
                    lowerAnimator.SetBool("Crouch", true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void MoveForward()
        {
            transform.Translate(transform.forward * speed * Time.deltaTime);
        }
        public void SetCursorLocked()
        {
            playerCursor.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _isInteractable = false;
        }
        public void SetCursorUnlocked()
        {
            playerCursor.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            _isInteractable = true;
        }
        public void RotateCameraX(float direction)
        {
            if (_ableToCrouch)
            {
                rotationDirectionX = direction;
                rotationX -= direction * sensitiveY;

                if (currentLowerState == LowerAnimationState.Walk)
                {
                    rotationX = Mathf.Clamp(rotationX, walkMinRotationX, walkMaxRotationX);
                }
                else if (currentLowerState == LowerAnimationState.Crouch)
                {
                    rotationX = Mathf.Clamp(rotationX, crouchMinRotationX, crouchMaxRotationX);
                }
                else
                {
                    rotationX = Mathf.Clamp(rotationX, minRotationX, maxRotationX);
                }
            }
        }
        public void RotateCameraY(float direction)
        {
            rotationDirectionY = direction;
            rotationY += direction * sensitiveY;
            if (currentLowerState == LowerAnimationState.Crouch)
            {
                rotationY = Mathf.Clamp(rotationY, -crouchLimitRotationY, crouchLimitRotationY);
            }
            else
            {
                rotationY = Mathf.Clamp(rotationY, -limitRotationY, limitRotationY);
            }

        }
        public void RepositioningThroughFoot(Transform mineTransform)
        {
            var diffrence = mineTransform.position - foot.position;
            transform.position += diffrence;
        }
      
        private InteractableClass CheckInteractable()
        {
            RaycastHit hit;
            var center = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(center);
#if UNITY_EDITOR
            Debug.DrawRay(ray.origin, ray.direction * checkTargetRayDistance,Color.red);
#endif
            var isContact = Physics.Raycast(ray.origin, ray.direction, out hit, checkTargetRayDistance);
            if (isContact && hit.transform.GetComponent<InteractableClass>().IsAvailable())
            {
                return hit.transform.GetComponent<InteractableClass>();
            }
            else
            {
                return null;
            }
        }
        private InteractableClass CheckTarget(string tag)
        {
            RaycastHit hit;
            var center = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(center);
#if UNITY_EDITOR
            Debug.DrawRay(ray.origin, ray.direction * checkTargetRayDistance,Color.red);
#endif
            var isContact = Physics.Raycast(ray.origin, ray.direction, out hit, checkTargetRayDistance);
            if (isContact && hit.collider.CompareTag(tag) && hit.transform.GetComponent<InteractableClass>().IsAvailable())
            {
                return hit.transform.GetComponent<InteractableClass>();
            }
            else
            {
                return null;
            }
        }

        private void UseHand(InteractableClass t)
        {
            var mouseUpStream = GlobalInputBinder.CreateGetKeyUpStream(KeyCode.Mouse0);
            var beginPosition = Input.mousePosition;
            mouseUpStream.Select(_ => Input.mousePosition)
                .Where(_ => t != null)
                .Take(1)
                .Subscribe(v =>
                {
                    if (t is Cover cover)
                    {
                        cover.TryThrow(Vector3.Normalize(v - beginPosition));
                    }
                });
        }

        public void Crouch()
        {
            _ableToCrouch = false;
            _isCrouching = true;
            body.DOLocalMoveY(crouchHeight, crouchDuration).OnComplete(() =>
            {
                _ableToCrouch = true;
                SetCursorUnlocked();
            });
            
            if (StoryModeManager.GetState() == StoryModeManager.State.LandMineDirt)
            {
                head.DOLocalMove(crouchHeadTransform1.localPosition, crouchDuration);
                arm.DOLocalMove(crouchArmTransform1.localPosition, crouchDuration);
            }
            else if (StoryModeManager.GetState() == StoryModeManager.State.LandMine)
            {
                head.DOLocalMove(crouchHeadTransform2.localPosition, crouchDuration);
                arm.DOLocalMove(crouchArmTransform2.localPosition, crouchDuration);
            }
            mainCamera.DOFieldOfView(crouchFov, crouchDuration);
            DOTween.To(() => rotationX, x => rotationX = x, crouchMaxRotationX, crouchDuration);

        }
        public void StandUp()
        {
            _ableToCrouch = false;
            _isCrouching = false;
            body.DOLocalMoveY(standUpHeight, standUpDuration).OnComplete(() =>
            {
                _ableToCrouch = true;
                SetCursorLocked();
            });
            arm.DOLocalMove(standArmTransform.localPosition, standUpDuration);
            head.DOLocalMove(standHeadTransform.localPosition, crouchDuration);
            mainCamera.DOFieldOfView(defaultFov, standUpDuration);
            DOTween.To(() => rotationX, x => rotationX = x, (maxRotationX + minRotationX) / 2, standUpDuration);

        }

        public void ActiveBagSlot()
        {
            bag.Activate();
            bag.Initialize(this);
        }

        public void ActiveHealthBar()
        {
            hpUI.gameObject.SetActive(true);
        }

        public UsableItem Hold(UsableItem item)
        {
            if (item == null)
            {
                if (hand != null)
                {
                    hand.gameObject.SetActive(false);
                }
                hand = null;
            }
            else if(hand == null)
            {
                hand = item;
                hand.gameObject.SetActive(true);
            }

            return hand;
        }


        public bool IsInteractable()
        {
            return _isInteractable;
        }
    }
}

