using System;
using Assets.Pia.Scripts.Game.Items;
using Assets.Pia.Scripts.Game.UI;
using Assets.Pia.Scripts.Interface;
using Assets.Pia.Scripts.StoryMode;
using Assets.Pia.Scripts.StoryMode.Walking;
using Assets.Pia.Scripts.UI;
using Default.Scripts.Printer;
using Default.Scripts.Sound;
using Default.Scripts.Util;
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
    public class Player : Singleton<Player>
    {
        public enum LowerAnimationState
        {
            Idle,
            Walk,
            Crouch
        }
        PathManager _pathManager;

        [Header("Å° ¼¼ÆÃ")]
        public KeyCode walkKey = KeyCode.W;
        public KeyCode crouchKey = KeyCode.LeftControl;
        public KeyCode stepKey = KeyCode.S;
        public KeyCode stepPedalKey = KeyCode.F7;

        public Bag bag;
        public Flashlight flashlight;

        [Header("Body Part")]
        [SerializeField] private Transform head;
        [SerializeField] private Transform body;
        [SerializeField] private Transform upperBody;
        [SerializeField] private Transform arm;
        public Camera mainCamera;
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
        public bool _ableToCrouch = true;

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
        [SerializeField] private int hp;
        private bool _isBleeding=false;
        [SerializeField]
        private Image bleedUI;
        [SerializeField]
        private Image bandageGuideImage;
        [SerializeField]
        private Image dyingUI;

        public InteractableClass target;
        private HoldableItem hand;
        private bool _isMovable = true;

        public bool IsMovable()
        {
           return _pathManager.PlayerIsMovable() && _isMovable;
        }
        public void SetMovable(bool value)
        {
             _isMovable = value;
        }
        public void Initialize(PathManager pathManager)
        {
            _pathManager = pathManager;
            initialLocalPosition = mainCamera.transform.localPosition;
            initialLocalRotation = mainCamera.transform.localRotation;
            mainCamera.fieldOfView = defaultFov;
            SetCursorLocked();
            CreateAnimationSubject();
            CreateLowerBodyStream();
            CreateCameraStream();
            CreateFlashLightStream();
            CreateHandStream();
            CreateHoverStream();
            this.UpdateAsObservable().Select(_=> GlobalConfiguration.Instance.GetMouseSensitive())
                .DistinctUntilChanged().Subscribe(_ => {
                sensitiveY = GlobalConfiguration.Instance.GetMouseSensitive();
                sensitiveX = GlobalConfiguration.Instance.GetMouseSensitive();
            }).AddTo(gameObject);
        }

        private void CreateFlashLightStream()
        {
            this.UpdateAsObservable().Subscribe(_ =>
            {
                flashlight.Follow(mainCamera.transform);
            }).AddTo(gameObject);
        }

        public void OnStepMine()
        {
            if (!bag.IsActivated())
            {
                ActiveBagSlot();
            }
          
            ActiveHealthBar();
            CreateHPStream();
            CreateRandomBleedEvent(0.95f, 0.87f, EventManager.Event.Bleed1);
            CreateRandomBleedEvent(0.65f, 0.50f, EventManager.Event.Bleed2);
            CreateHPEventStream();
        }

        private void CreateHPEventStream()
        {
            var hpStream = this.UpdateAsObservable()
                .Select(_ => (float)hp / initialHp);

            hpStream.Where(hpRate => hpRate <= 0.8f).Take(1).Subscribe(_ =>
            {
                EventManager.InvokeEvent(EventManager.Event.HP80);
                SoundManager.Play("300hz_noise", 5);
            }).AddTo(gameObject);
            hpStream.Where(hpRate => hpRate <= 0.6f).Take(1).Subscribe(_ =>
            {
                EventManager.InvokeEvent(EventManager.Event.HP60);
                SoundManager.Play("600hz_noise", 5);
            }).AddTo(gameObject);
            hpStream.Where(hpRate => hpRate <= 0.4f).Take(1).Subscribe(_ =>
            {
                EventManager.InvokeEvent(EventManager.Event.HP40);
                SoundManager.Play("1000hz_noise", 5);
            }).AddTo(gameObject);
            hpStream.Where(hpRate => hpRate <= 0.2f).Take(1).Subscribe(_ =>
            {
                EventManager.InvokeEvent(EventManager.Event.HP20);
                SoundManager.Play("1500hz_noise", 5);
            }).AddTo(gameObject);
            hpStream.Where(hpRate => hpRate <= 0.1f).Take(1).Subscribe(hpRate =>
            {
                EventManager.InvokeEvent(EventManager.Event.HP10);
                dyingUI.gameObject.SetActive(true);
                dyingUI.CrossFadeAlpha((0.1f - hpRate) / 0.1f, 0.1f, false);
                SoundManager.Play("2400hz_noise", 5);
            }).AddTo(gameObject);

        }

        private void CreateHandStream()
        {
            GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Mouse0)
                .Where(_ => hand == null)
                .Subscribe(_ => UseHand(target))
                .AddTo(gameObject);
        }

        private void CreateRandomBleedEvent(float p0, float p1, EventManager.Event e)
        {
           float random =  Random.Range(p1, p0);
           Observable.Interval(TimeSpan.FromSeconds(hpDecreaseInterval))
                .Where(_ => random * initialHp >= hp).Take(1)
               .Subscribe(_ =>
               {
                   Bleed();
                   SoundManager.Play("event_startEmergency", 8);
                   EventManager.InvokeEvent(e);
               })
               .AddTo(gameObject);
        }

        private void CreateHPStream()
        {
            hp = initialHp;
            Observable.Interval(TimeSpan.FromSeconds(hpDecreaseInterval))
                .Subscribe(_ => SetHP(hp- hpReduction))
                .AddTo(gameObject);
        }

        private void SetHP(int value)
        {
            Debug.Log(hp);
            hp = Math.Max(value,0);
            hpBar.DOFillAmount((float)hp / initialHp, hpDecreaseInterval);
            if (hp == 0)
            {
                StoryModeManager.GameOver(StoryModeManager.GameOverType.Bleed);
            }
        }

        public bool IsBleeeding()
        {
            return _isBleeding;
        }
        private void Bleed()
        {
            hpReduction = 2;
            _isBleeding = true;
            bleedUI.gameObject.SetActive(true);
            bleedUI.DOFade(0.04f, 0.5f).SetLoops(-1,LoopType.Yoyo).SetId("BleedTween");
            bandageGuideImage.DOFade(0.5f, 0.5f).SetLoops(-1,LoopType.Yoyo).SetId("BandageGuideTween");
        }

        public void CureBleed()
        {
            hpReduction = 1;
            _isBleeding = false;
            DOTween.Kill("BleedTween");
            DOTween.Kill("BandageGuideTween");
            SoundManager.Stop( 8);
            bandageGuideImage.DOFade(0, 1).OnComplete(() =>
            {
                bleedUI.gameObject.SetActive(false);
            });
            bleedUI.DOFade(0, 1).OnComplete(() =>
            {
                bleedUI.gameObject.SetActive(false);
            });
        }
        private void CreateHoverStream()
        {
            this.LateUpdateAsObservable()
                .Select(_ => CheckInteractable())
                .Subscribe(t =>
                {
                    if (_isInteractable)
                    {
                        if (target != null)
                        {
                            target.OnExit();
                        }
                        if (t != null && IsLightOn())
                        {
                            t.OnHover(hand);
                        }

                        if (IsLightOn())
                        {
                            target = t;
                        }
                        else
                        {
                            target = null;
                        }
                    }
                    else
                    {
                        if (target != null)
                        {
                            target.OnExit();
                        }
                        target = null;
                    }
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
            GlobalInputBinder.CreateGetAxisStream("Mouse X").Subscribe(RotateCameraY).AddTo(gameObject); ;
            GlobalInputBinder.CreateGetAxisStream("Mouse Y").Subscribe(RotateCameraX).AddTo(gameObject);
            this.UpdateAsObservable().Subscribe(_ => RotateHead()).AddTo(gameObject);
            this.UpdateAsObservable().Where(_=>GlobalConfiguration.Instance.GetHeadBob()).Select(_ => currentLowerState)
                .DistinctUntilChanged().Subscribe(ShakeCamera).AddTo(gameObject);
        }
        private void ShakeCamera(LowerAnimationState state)
        {
            DOTween.Kill("shakeCamera");
            DOTween.Kill("shakeCameraRot");
            mainCamera.transform.localPosition = initialLocalPosition;
            mainCamera.transform.localRotation = initialLocalRotation;
            if (state == LowerAnimationState.Walk)
            {
                mainCamera.transform.DOShakePosition(1f, new Vector3(1, 0.5f, 0) * 0.06f, 0).SetLoops(-1, LoopType.Yoyo).SetId("shakeCamera").SetEase(Ease.InOutSine);
            }
            else if(state == LowerAnimationState.Idle)
            {
                mainCamera.transform.DOShakePosition(4.0f, new Vector3(1,1,0) * 0.02f,1).SetLoops(-1, LoopType.Yoyo).SetId("shakeCamera").SetEase(Ease.InOutSine);
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
                .Where(_ => StoryModeManager.GetState() != StoryModeManager.State.Walking && StoryModeManager.IsInteractionActive() && !_isMove)
                .Where(_ => !_isCrouching && _ableToCrouch).Subscribe(_ => Crouch()).AddTo(gameObject);

            GlobalInputBinder.CreateGetKeyDownStream(crouchKey)
                .Where(_ => StoryModeManager.GetState() != StoryModeManager.State.Walking && StoryModeManager.IsInteractionActive() && !_isMove)
                .Where(_ => _isCrouching && _ableToCrouch).Subscribe(_ => StandUp()).AddTo(gameObject);
        }
        private void CreateWalkingStream()
        {
            this.UpdateAsObservable()
                .Where(_ => StoryModeManager.GetState() == StoryModeManager.State.Walking)
                .Subscribe(_ =>
                {
                    DOTween.Kill("changeDirection");
                    transform.DORotate(Quaternion.LookRotation(GetCurrentDirection()).eulerAngles, 1.0f).SetId("changeDirection");
                }).AddTo(gameObject);

            this.UpdateAsObservable()
                .Where(_ => StoryModeManager.GetState() == StoryModeManager.State.Walking)
                .Subscribe(_ =>
                {
                    _pathManager.UpdateCurrentNode(transform.position);
                }).AddTo(gameObject);

            GlobalInputBinder.CreateGetKeyStream(walkKey)
                .Where(_ => StoryModeManager.GetState() == StoryModeManager.State.Walking)
                .Where(_ => !_isCrouching && _ableToCrouch && IsMovable())
                .Subscribe(_ =>
                {
                    MoveForward();
                    _isMove = true;
                }).AddTo(gameObject);
        }


        public Vector3 GetCurrentDirection()
        {
            var next = _pathManager.GetNext();
            if (next != null)
            {
                return Vector3.Normalize(next.transform.position - transform.position);
            }
            else
            {
                return Vector3.forward;
            }
        }
        private void RotateHead()
        {
           
            head.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
            if (!_isCrouching&& _ableToCrouch)
            {
                head.localPosition = new Vector3(rotationY * 0.005f, head.localPosition.y, 0.14f + rotationX * 0.003f);
            }
            DOTween.Kill("followHeadPos");
            upperBody.localRotation = head.localRotation;
            upperBody.DOLocalMove(head.localPosition, 0.2f).SetId("followHeadPos");
        }

        private void CreateAnimationSubject()
        {
            lowerStateSubject = new Subject<LowerAnimationState>();
            lowerStateSubject.Subscribe(AnimateLowerBody).AddTo(gameObject);
            lowerStateSubject.OnNext(LowerAnimationState.Idle);

            lowerStateSubject.DistinctUntilChanged()
                .Where(x => x == LowerAnimationState.Walk)
                .Subscribe(_ =>SoundManager.Play("char_walk", 4)).AddTo(gameObject);
            lowerStateSubject.DistinctUntilChanged()
                .Where(x => x == LowerAnimationState.Idle)
                .Subscribe(_ => SoundManager.Stop(4)).AddTo(gameObject);
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
            _isInteractable = false;
        }
        public void SetCursorUnlocked()
        {
            playerCursor.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
            _isInteractable = true;
        }
        public void SetCursorLockedAndInteractable()
        {
            playerCursor.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            _isInteractable = true;
        }
        public void RotateCameraX(float direction)
        {
            if (!_isCrouching && _ableToCrouch && !_isInteractable)
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
            if (_ableToCrouch && !_isInteractable)
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

            if (t is Cover cover)
            {
                cover.Throw();
            }
        }

        public void Crouch()
        {
            _ableToCrouch = false;
            _isCrouching = true;
            SoundManager.Play("char_standCrouch", 1);
            body.DOLocalMoveY(crouchHeight, crouchDuration).OnComplete(() =>
            {
                _ableToCrouch = true;
                SetCursorUnlocked();
            });
            
            if (StoryModeManager.GetState() == StoryModeManager.State.LandMineDirt)
            {
                head.DOLocalMove(crouchHeadTransform1.localPosition, crouchDuration).SetEase(Ease.InOutQuad);
                arm.DOLocalMove(crouchArmTransform1.localPosition, crouchDuration).SetEase(Ease.InOutQuad);
            }
            else if (StoryModeManager.GetState() == StoryModeManager.State.LandMine)
            {
                head.DOLocalMove(crouchHeadTransform2.localPosition, crouchDuration).SetEase(Ease.InOutQuad);
                arm.DOLocalMove(crouchArmTransform2.localPosition, crouchDuration).SetEase(Ease.InOutQuad);
            }
            mainCamera.DOFieldOfView(crouchFov, crouchDuration);
            DOTween.To(() => rotationX, x => rotationX = x, crouchMaxRotationX, crouchDuration).SetEase(Ease.InOutQuad);
            DOTween.To(() => rotationY, x => rotationY = x, 0, crouchDuration).SetEase(Ease.InOutQuad);

        }
        public void StandUp(bool ableToCrouch = true)
        {
            _ableToCrouch = false;
            _isCrouching = false;
            SoundManager.Play("char_crouchStand", 1);
            SetCursorLocked();
            body.DOLocalMoveY(standUpHeight, standUpDuration).OnComplete(() =>
            {
                _ableToCrouch = ableToCrouch;
            }).SetEase(Ease.InOutQuad);
            arm.DOLocalMove(standArmTransform.localPosition, standUpDuration).SetEase(Ease.InOutQuad);
            head.DOLocalMove(standHeadTransform.localPosition, standUpDuration).SetEase(Ease.InOutQuad);
            mainCamera.DOFieldOfView(defaultFov, standUpDuration).SetEase(Ease.InOutQuad);
            DOTween.To(() => rotationX, x => rotationX = x, (maxRotationX + minRotationX) / 2, standUpDuration).SetEase(Ease.InOutQuad);
            DOTween.To(() => rotationY, x => rotationY = x, 0, standUpDuration).SetEase(Ease.InOutQuad);
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

        public HoldableItem Hold(HoldableItem item)
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

        public bool IsCrouch()
        {
            return _isCrouching;
        }

        public bool IsLightOn()
        {
            return flashlight.isActiveAndEnabled;
        }
    }
}

