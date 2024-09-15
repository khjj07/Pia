using System;
using Assets.Pia.Scripts.StoryMode.Walking;
using Assets.Pia.Scripts.UI;
using Default.Scripts.Util;
using DG.Tweening;
using Pia.Scripts.StoryMode;
using UniRx;
using UniRx.Triggers;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using GlobalInputBinder = Default.Scripts.Util.GlobalInputBinder;
using Unit = UniRx.Unit;

namespace Assets.Pia.Scripts.StoryMode
{
    public class Player : MonoBehaviour
    {
        public enum LowerAnimationState
        {
            Idle,
            Walk,
            Crouch
        }
        public enum UpperState
        {
            None,
            Bandage,
            Bottle,
            Dagger,
            Driver,
            Nipper,
            Pen,
            Shovel,
            ShovelUse
        }

        [SerializeField] private PlayerUI _playerUI;
        [Header("키 세팅")]
        public KeyCode walkKey = KeyCode.W;
        public KeyCode crouchKey = KeyCode.LeftControl;
        public KeyCode bagKey = KeyCode.Tab;
        public KeyCode guideBookKey = KeyCode.R;
        public KeyCode guideBookPreviousKey = KeyCode.Q;
        public KeyCode guideBookNextKey = KeyCode.E;
        public KeyCode cartridgeKey = KeyCode.F;
        public KeyCode flashlightKey = KeyCode.C;
        public KeyCode letterKey = KeyCode.T;
        public KeyCode secondaryBagKey = KeyCode.G;
        public KeyCode bandageKey = KeyCode.V;


        public KeyCode driverKey = KeyCode.E;
        public KeyCode nipperKey = KeyCode.D;
        public KeyCode penKey = KeyCode.X;
        public KeyCode daggerKey = KeyCode.Y;
        public KeyCode shovelKey = KeyCode.H;
        public KeyCode bottleKey = KeyCode.B;

        public KeyCode stepKey = KeyCode.S;
        public KeyCode stepPedalKey = KeyCode.F7;

        [Header("Tools")]
        [SerializeField] private Light flashLight;
        [SerializeField] private Image letter;
        [SerializeField] private GuideBook guideBook;


        private bool _isCrouching;
        private bool _isMove = false;
        private bool _ableToCrouch = true;

        private Vector3 initialLocalPosition;
        private Quaternion initialLocalRotation;

        [Header("Body Part")]
        [SerializeField] private Transform head;
        [SerializeField] private Transform body;
        [SerializeField] private Transform upperBody;
        [SerializeField] private Transform arm;
        [SerializeField] private Camera mainCamera;
        public Transform foot;

        [Header("Common")]
        [SerializeField] private float sensitiveX = 1.0f;
        [SerializeField] private float sensitiveY = 1.0f;
        [SerializeField] private float limitRotationY = 0;
        [SerializeField] private float defaultFov = 60;


        [Header("Walk")]
        [SerializeField] private float walkMinRotationX = 0;
        [SerializeField] private float walkMaxRotationX = 0;
        [SerializeField] private float standUpHeight = 1.0f;
        [SerializeField] private float standUpDuration = 1.0f;

        [Header("Idle")]
        [SerializeField] private float minRotationX = 0;
        [SerializeField] private float maxRotationX = 0;

        [Header("Crouch")]
        [SerializeField] private Transform crouchArmTransform;
        [SerializeField] private Transform standArmTransform;
        [SerializeField] private float crouchMinRotationX = 0;
        [SerializeField] private float crouchMaxRotationX = 0;
        [SerializeField] private float crouchLimitRotationY = 0;
        [SerializeField] private float crouchHeight = 1.0f;
        [SerializeField] private float crouchDuration = 1.0f;
        [SerializeField] private float crouchFov = 50;

        [Header("Path")]
        [SerializeField] private float speed = 1.0f;
        [SerializeField] PathManager pathManager;


        private float rotationDirectionX = 0;
        private float rotationX = 0;

        private float rotationDirectionY = 0;
        private float rotationY = 0;

        private float previousRotationX;


        public Subject<LowerAnimationState> lowerStateSubject;
        public Subject<UpperState> upperStateSubject;
        private LowerAnimationState currentLowerState;
        private UpperState currentUpperState;

        [Header("Animator")]
        [SerializeField] private Animator lowerAnimator;
        [SerializeField] private Animator bandageAnimator;
        [SerializeField] private Animator bottleAnimator;
        [SerializeField] private Animator daggerAnimator;
        [SerializeField] private Animator driverAnimator;
        [SerializeField] private Animator nippleAnimator;
        [SerializeField] private Animator penAnimator;
        [SerializeField] private Animator shovelAnimator;

        private bool _isPlayActive = false;


        public void RepositioningThroughFoot(Transform mineTransform)
        {
            var diffrence = mineTransform.position - foot.position;
            transform.position += diffrence;
        }


        void Start()
        {
            initialLocalPosition = mainCamera.transform.localPosition;
            initialLocalRotation = mainCamera.transform.localRotation;
            mainCamera.fieldOfView = defaultFov;
            SetCursorOption();
            CreateAnimationSubject();
            CreateLowerBodyStream();
            CreateUpperBodyStream();
            CreateCameraStream();
        }

        private void CreateUpperBodyStream()
        {
            //가방(Tab)
            GlobalInputBinder.CreateGetKeyDownStream(bagKey).SkipWhile(_ => !_isPlayActive).Where(_ => !_playerUI.IsBagOpen()).Subscribe(_ =>
            {
                _playerUI.OpenBag();
            });
            GlobalInputBinder.CreateGetKeyDownStream(bagKey).SkipWhile(_ => !_isPlayActive).Where(_ => _playerUI.IsBagOpen()).Subscribe(_ =>
            {
                _playerUI.CloseBag();
            });

            //탄띠(F)
            GlobalInputBinder.CreateGetKeyDownStream(cartridgeKey).SkipWhile(_ => !_isPlayActive).Subscribe(_ => OnCartridgeKeyPressed());

            //보조가방(G)
            GlobalInputBinder.CreateGetKeyDownStream(secondaryBagKey).SkipWhile(_ => !_isPlayActive).Subscribe(_ => OnSecondaryBagKeyPressed());

            //손전등(C)
            GlobalInputBinder.CreateGetKeyDownStream(flashlightKey).SkipWhile(_ => !_isPlayActive).Subscribe(_ => OnFlashLightKeyPressed());

            //편지(T)
            GlobalInputBinder.CreateGetKeyDownStream(letterKey).SkipWhile(_ => !_isPlayActive).Subscribe(_ => OnLetterKeyPressed());

            //교본(R)
            GlobalInputBinder.CreateGetKeyDownStream(guideBookKey).SkipWhile(_ => !_isPlayActive).Subscribe(_ => OnGuideBookKeyPressed());

            //붕대(V)
            GlobalInputBinder.CreateGetKeyDownStream(bandageKey).SkipWhile(_ => !_isPlayActive).Subscribe(_ => OnBandageKeyPressed());

        }

        #region Cartridge

        private void OnCartridgeKeyPressed()
        {
            var cartridgeCloseStream = GlobalInputBinder.CreateGetKeyUpStream(cartridgeKey);

            cartridgeCloseStream.First().Subscribe(_ => OnCartridgeKeyReleased());
            GlobalInputBinder.CreateGetKeyDownStream(driverKey).Where(_ => currentUpperState == UpperState.None).TakeUntil(cartridgeCloseStream)
                .Subscribe(_ => OnDriverKeyPressed()).AddTo(gameObject);
            GlobalInputBinder.CreateGetKeyDownStream(nipperKey).Where(_ => currentUpperState == UpperState.None).TakeUntil(cartridgeCloseStream)
                .Subscribe(_ => OnNipperKeyPressed()).AddTo(gameObject);
            GlobalInputBinder.CreateGetKeyDownStream(penKey).Where(_ => currentUpperState == UpperState.None).TakeUntil(cartridgeCloseStream)
                .Subscribe(_ => OnPenKeyPressed()).AddTo(gameObject);
            _playerUI.OpenCartridge();
        }

        private void OnCartridgeKeyReleased()
        {
            _playerUI.CloseCartridge();
        }

        private void OnPenKeyPressed()
        {
            var penReleaseStream = GlobalInputBinder.CreateGetKeyUpStream(penKey);
            var cartridgeCloseStream = GlobalInputBinder.CreateGetKeyUpStream(cartridgeKey);

            penReleaseStream.Amb(cartridgeCloseStream).First().Subscribe(_ => OnPenKeyRelaese());
            upperStateSubject.OnNext(UpperState.Pen);
            _playerUI.SetSlotActive(_playerUI.penSlot);
        }

        private void OnPenKeyRelaese()
        {
            upperStateSubject.OnNext(UpperState.None);
            _playerUI.SetSlotInactive(_playerUI.penSlot);
        }

        private void OnNipperKeyPressed()
        {
            var nipperReleaseStream = GlobalInputBinder.CreateGetKeyUpStream(nipperKey);
            var cartridgeCloseStream = GlobalInputBinder.CreateGetKeyUpStream(cartridgeKey);

            nipperReleaseStream.Amb(cartridgeCloseStream)
                .First().Subscribe(_ => OnNipperKeyRelaese());
            upperStateSubject.OnNext(UpperState.Nipper);
            _playerUI.SetSlotActive(_playerUI.nipperSlot);
        }

        private void OnNipperKeyRelaese()
        {
            upperStateSubject.OnNext(UpperState.None);
            _playerUI.SetSlotInactive(_playerUI.nipperSlot);
        }

        private void OnDriverKeyPressed()
        {
            var driverReleaseStream = GlobalInputBinder.CreateGetKeyUpStream(driverKey);
            var cartridgeCloseStream = GlobalInputBinder.CreateGetKeyUpStream(cartridgeKey);

            driverReleaseStream.Amb(cartridgeCloseStream)
                .First().Subscribe(_ => OnDriverKeyRelaese());
            upperStateSubject.OnNext(UpperState.Driver);
            _playerUI.SetSlotActive(_playerUI.driverSlot);
        }

        private void OnDriverKeyRelaese()
        {
            upperStateSubject.OnNext(UpperState.None);
            _playerUI.SetSlotInactive(_playerUI.driverSlot);
        }




        #endregion

        #region SecondaryBag;
        private void OnSecondaryBagKeyPressed()
        {
            var secondaryBagCloseStream = GlobalInputBinder.CreateGetKeyUpStream(secondaryBagKey);

            secondaryBagCloseStream.First().Subscribe(_ => OnSecondaryBagKeyReleased());

            GlobalInputBinder.CreateGetKeyDownStream(daggerKey).Where(_ => currentUpperState == UpperState.None).TakeUntil(secondaryBagCloseStream)
                .Subscribe(_ => OnDaggerKeyPressed()).AddTo(gameObject);
            GlobalInputBinder.CreateGetKeyDownStream(shovelKey).Where(_ => currentUpperState == UpperState.None).TakeUntil(secondaryBagCloseStream)
                .Subscribe(_ => OnShovelKeyPressed()).AddTo(gameObject);
            GlobalInputBinder.CreateGetKeyDownStream(bottleKey).Where(_ => currentUpperState == UpperState.None).TakeUntil(secondaryBagCloseStream)
                .Subscribe(_ => OnBottleKeyPressed()).AddTo(gameObject);
            _playerUI.OpenSecondaryBag();
        }
        private void OnSecondaryBagKeyReleased()
        {
            _playerUI.CloseSecondaryBag();
        }

        private void OnDaggerKeyPressed()
        {
            var daggerKeyReleasedStream = GlobalInputBinder.CreateGetKeyUpStream(daggerKey);
            var secondaryBagCloseStream = GlobalInputBinder.CreateGetKeyUpStream(secondaryBagKey);

            daggerKeyReleasedStream.Amb(secondaryBagCloseStream)
                .First().Subscribe(_ => OnDaggerKeyReleased());
            upperStateSubject.OnNext(UpperState.Dagger);
            _playerUI.SetSlotActive(_playerUI.daggerSlot);
        }

        private void OnDaggerKeyReleased()
        {
            upperStateSubject.OnNext(UpperState.None);
            _playerUI.SetSlotInactive(_playerUI.daggerSlot);
        }

        private void OnShovelKeyPressed()
        {
            var shovelKeyReleasedStream = GlobalInputBinder.CreateGetKeyUpStream(shovelKey);
            var secondaryBagCloseStream = GlobalInputBinder.CreateGetKeyUpStream(secondaryBagKey);

            shovelKeyReleasedStream.Amb(secondaryBagCloseStream)
                .First().Subscribe(_ => OnShovelKeyReleased());
            upperStateSubject.OnNext(UpperState.Shovel);
            _playerUI.SetSlotActive(_playerUI.shovelSlot);
        }

        private void OnShovelKeyReleased()
        {
            upperStateSubject.OnNext(UpperState.None);
            _playerUI.SetSlotInactive(_playerUI.shovelSlot);
        }

        #endregion
        private void OnBottleKeyPressed()
        {
            var bottleKeyReleasedStream = GlobalInputBinder.CreateGetKeyUpStream(bottleKey);
            var secondaryBagCloseStream = GlobalInputBinder.CreateGetKeyUpStream(secondaryBagKey);

            bottleKeyReleasedStream.Amb(secondaryBagCloseStream)
                .First().Subscribe(_ => OnBottleKeyReleased());
            upperStateSubject.OnNext(UpperState.Bottle);
            _playerUI.SetSlotActive(_playerUI.bottleSlot);
        }
        private void OnBottleKeyReleased()
        {
            upperStateSubject.OnNext(UpperState.None);
            _playerUI.SetSlotInactive(_playerUI.bottleSlot);
        }


        private void OnLetterKeyPressed()
        {
            var letterKeyReleasedStream = GlobalInputBinder.CreateGetKeyUpStream(letterKey);
            letterKeyReleasedStream.First().Subscribe(_ => OnLetterKeyReleased());
            _playerUI.SetSlotActive(_playerUI.letterSlot);
            letter.gameObject.SetActive(true);
        }

        private void OnLetterKeyReleased()
        {
            _playerUI.SetSlotInactive(_playerUI.letterSlot);
            letter.gameObject.SetActive(false);
        }

        private void OnFlashLightKeyPressed()
        {
            var flashLightKeyReleasedStream = GlobalInputBinder.CreateGetKeyUpStream(flashlightKey);
            flashLightKeyReleasedStream.First().Subscribe(_ => OnFlashLightKeyReleased());
            _playerUI.SetSlotActive(_playerUI.flashLightSlot);
            flashLight.enabled = true;
        }

        private void OnFlashLightKeyReleased()
        {
            _playerUI.SetSlotInactive(_playerUI.flashLightSlot);
            flashLight.enabled = false;
        }

        private void OnGuideBookKeyPressed()
        {
            var guideBookKeyReleasedStream = GlobalInputBinder.CreateGetKeyUpStream(guideBookKey);
            guideBookKeyReleasedStream.First().Subscribe(_ => OnGuideBookKeyReleased());
            _playerUI.SetSlotActive(_playerUI.guideBookSlot);
            guideBook.gameObject.SetActive(true);

            GlobalInputBinder.CreateGetKeyDownStream(guideBookPreviousKey)
                .TakeUntil(guideBookKeyReleasedStream)
                .Subscribe(_ =>
                {
                    guideBook.Previous();
                }).AddTo(gameObject);

            GlobalInputBinder.CreateGetKeyDownStream(guideBookNextKey)
                .TakeUntil(guideBookKeyReleasedStream)
                .Subscribe(_ =>
                {
                    guideBook.Next();
                }).AddTo(gameObject);
        }

        private void OnGuideBookKeyReleased()
        {
            _playerUI.SetSlotInactive(_playerUI.guideBookSlot);
            guideBook.gameObject.SetActive(false);
        }

        private void OnBandageKeyPressed()
        {
            var bandageKeyReleasedStream = GlobalInputBinder.CreateGetKeyUpStream(bandageKey);
            bandageKeyReleasedStream.First().Subscribe(_ => OnBandageKeyReleased());
            _playerUI.SetSlotActive(_playerUI.bandageSlot);
            upperStateSubject.OnNext(UpperState.Bandage);
        }

        private void OnBandageKeyReleased()
        {
            _playerUI.SetSlotInactive(_playerUI.bandageSlot);
            upperStateSubject.OnNext(UpperState.None);
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

            GlobalInputBinder.CreateGetKeyDownStream(crouchKey)
                .SkipWhile(_ => !_isPlayActive)
                .Where(_ => !_isCrouching && _ableToCrouch).Subscribe(_ =>
                {
                    _ableToCrouch = false;
                    _isCrouching = true;
                    body.DOLocalMoveY(crouchHeight, crouchDuration).OnComplete(() =>
                    {
                        _ableToCrouch = true;
                    });
                    arm.DOLocalMove(crouchArmTransform.localPosition, crouchDuration);
                    mainCamera.DOFieldOfView(crouchFov, crouchDuration);
                    DOTween.To(() => rotationX, x => rotationX = x, crouchMaxRotationX, crouchDuration);
                }).AddTo(gameObject);

            GlobalInputBinder.CreateGetKeyDownStream(crouchKey)
                .SkipWhile(_ => !_isPlayActive)
                .Where(_ => _isCrouching && _ableToCrouch).Subscribe(_ =>
                {
                    _ableToCrouch = false;
                    _isCrouching = false;
                    body.DOLocalMoveY(standUpHeight, standUpDuration).OnComplete(() =>
                    {
                        _ableToCrouch = true;
                    });
                    arm.DOLocalMove(standArmTransform.localPosition, standUpDuration);
                    mainCamera.DOFieldOfView(defaultFov, standUpDuration);
                    DOTween.To(() => rotationX, x => rotationX = x, (maxRotationX + minRotationX) / 2, standUpDuration);
                }).AddTo(gameObject);
        }

        private void RotateHead()
        {
            head.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
            head.localPosition = new Vector3(rotationY * 0.003f, head.localPosition.y, 0.14f + rotationX * 0.003f);

            DOTween.Kill("followHeadPos");
            upperBody.localRotation = head.localRotation;
            upperBody.DOLocalMove(head.localPosition, 0.2f).SetId("followHeadPos"); ;
        }

        private void CreateAnimationSubject()
        {
            lowerStateSubject = new Subject<LowerAnimationState>();
            upperStateSubject = new Subject<UpperState>();
            lowerStateSubject.Subscribe(AnimateLowerBody);
            upperStateSubject.Subscribe(AnimateUpperBody);
            lowerStateSubject.OnNext(LowerAnimationState.Idle);
            upperStateSubject.OnNext(UpperState.None);
        }

        private void AnimateUpperBody(UpperState state)
        {
            currentUpperState = state;
            bandageAnimator.gameObject.SetActive(false);
            bottleAnimator.gameObject.SetActive(false);
            daggerAnimator.gameObject.SetActive(false);
            driverAnimator.gameObject.SetActive(false);
            nippleAnimator.gameObject.SetActive(false);
            penAnimator.gameObject.SetActive(false);
            shovelAnimator.gameObject.SetActive(false);
            switch (state)
            {
                case UpperState.None:
                    break;
                case UpperState.Bandage:
                    bandageAnimator.gameObject.SetActive(true);
                    break;
                case UpperState.Bottle:
                    bottleAnimator.gameObject.SetActive(true);
                    break;
                case UpperState.Dagger:
                    daggerAnimator.gameObject.SetActive(true);
                    break;
                case UpperState.Driver:
                    driverAnimator.gameObject.SetActive(true);
                    break;
                case UpperState.Nipper:
                    nippleAnimator.gameObject.SetActive(true);
                    break;
                case UpperState.Pen:
                    penAnimator.gameObject.SetActive(true);
                    break;
                case UpperState.Shovel:
                    shovelAnimator.gameObject.SetActive(true);
                    break;
                case UpperState.ShovelUse:
                    shovelAnimator.gameObject.SetActive(true);
                    shovelAnimator.SetBool("Use", true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
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
        private void SetCursorOption()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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

        public void ActivePlayerUI()
        {
            _isPlayActive = true;
        }
        public void InactivePlayerUI()
        {
            _isPlayActive = false;
        }
        public void ActiveBagSlot()
        {
            _playerUI.bagSlot.gameObject.SetActive(true);
        }

        public void ActiveHealthBar()
        {
            _playerUI.healthBar.gameObject.SetActive(true);
        }
    }
}

