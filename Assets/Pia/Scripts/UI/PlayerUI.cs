using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Pia.Scripts.UI
{
    public class PlayerUI : MonoBehaviour
    {
        public RectTransform bagSlot;
        [Header("HP")] 
        public RectTransform healthBar;
        [Header("Bag")]
        public RectTransform bag;
        public RectTransform cartridge;
        public RectTransform cartridgeSlot;
        public RectTransform secondaryBag;
        public RectTransform secondaryBagSlot;

        [Header("Tools")]
        public RectTransform guideBookSlot;
        public RectTransform driverSlot;
        public RectTransform nipperSlot;
        public RectTransform penSlot;
        public RectTransform flashLightSlot;
        public RectTransform letterSlot;
        public RectTransform daggerSlot;
        public RectTransform shovelSlot;
        public RectTransform bottleSlot;
        public RectTransform bandageSlot;


        public Color activateColor = Color.yellow;
        public Color inactivateColor = Color.white;

        private bool _isBagOpen = false;
        private bool _isCartridgeOpen = false;
        private bool _isSecondaryBagOpen = false;


        private Tween _secondaryBagScaleTween;
        private Sequence _cartridgeScaleTween;
        private Tween _bagScaleTween;

        public void Start()
        {
 
        }

        public void OpenSecondaryBag()
        {
            CancelTween(_secondaryBagScaleTween);

            secondaryBag.gameObject.SetActive(true);
            secondaryBag.localScale = Vector3.zero;
            ChangeSlotColor(secondaryBagSlot, activateColor);
            _secondaryBagScaleTween = secondaryBag.DOScale(Vector3.one, 1f).SetEase(Ease.OutElastic).OnComplete(() =>
            {
                _isSecondaryBagOpen = true;
            });
        }

        public void CloseSecondaryBag()
        {
            CancelTween(_secondaryBagScaleTween);
            _secondaryBagScaleTween = secondaryBag.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                secondaryBag.gameObject.SetActive(false);
                ChangeSlotColor(secondaryBagSlot, inactivateColor);
                _isSecondaryBagOpen = false;
            });
        }

        private void CancelTween(Tween tween)
        {
            if (tween != null)
            {
                tween.Kill();
            }
        }

        public void OpenCartridge()
        {
            cartridge.gameObject.SetActive(true);
            ChangeSlotColor(cartridgeSlot, activateColor);
            //cartridge.localScale = Vector3.zero;
            //CancelTween(_cartridgeScaleTween);
            //_cartridgeScaleTween = cartridge.DOScale(Vector3.one, 1f).SetEase(Ease.OutElastic).OnComplete(() =>
            //{
            //    _isCartridgeOpen = true;
            //});
            driverSlot.localScale=Vector3.zero;
            nipperSlot.localScale=Vector3.zero;
            penSlot.localScale=Vector3.zero;
            if (_cartridgeScaleTween !=null)
            {
                _cartridgeScaleTween.Kill();
                _cartridgeScaleTween= null;
            }
            _cartridgeScaleTween = DOTween.Sequence();
            _cartridgeScaleTween.Append(driverSlot.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
            _cartridgeScaleTween.Join(nipperSlot.DOScale(Vector3.one, 0.5f).SetDelay(0.1f).SetEase(Ease.OutBack));
            _cartridgeScaleTween.Join(penSlot.DOScale(Vector3.one, 0.5f).SetDelay(0.2f).SetEase(Ease.OutBack));
            _cartridgeScaleTween.AppendCallback(() => _isCartridgeOpen = true);
            _cartridgeScaleTween.Play();
        }
        public void CloseCartridge()
        {
            if (_cartridgeScaleTween != null)
            {
                _cartridgeScaleTween.Kill();
                _cartridgeScaleTween = null;
            }
            _cartridgeScaleTween = DOTween.Sequence();
            _cartridgeScaleTween.Append(penSlot.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack));
            _cartridgeScaleTween.Join(nipperSlot.DOScale(Vector3.zero, 0.5f).SetDelay(0.1f).SetEase(Ease.InBack));
            _cartridgeScaleTween.Join(driverSlot.DOScale(Vector3.zero, 0.5f).SetDelay(0.2f).SetEase(Ease.InBack));
            _cartridgeScaleTween.AppendCallback(() =>
            {
                cartridge.gameObject.SetActive(false);
                ChangeSlotColor(cartridgeSlot, inactivateColor);
                _isCartridgeOpen = false;
            });
            _cartridgeScaleTween.Play();
            //_cartridgeScaleTween = cartridge.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            //{
            //    cartridge.gameObject.SetActive(false);
            //    ChangeSlotColor(cartridgeSlot, inactivateColor);
            //    _isCartridgeOpen = false;
            //});
        }

        public void OpenBag()
        {

            bag.gameObject.SetActive(true);
            bag.localScale = Vector3.zero;
            ChangeSlotColor(bagSlot, activateColor);
            CancelTween(_bagScaleTween);
            _bagScaleTween = bag.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutElastic).OnComplete(() =>
            {
                _isBagOpen = true;
            });
        }

        public void CloseBag()
        {
            CancelTween(_bagScaleTween);
            _bagScaleTween = bag.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                bag.gameObject.SetActive(false);
                ChangeSlotColor(bagSlot, inactivateColor);
                _isBagOpen = false;
            });
        }

        public void SetSlotActive(RectTransform rectTransform)
        {
            ChangeSlotColor(rectTransform, activateColor);
        }
        public void SetSlotInactive(RectTransform rectTransform)
        {
            ChangeSlotColor(rectTransform, inactivateColor);
        }

        public void ChangeSlotColor(RectTransform rectTransform, Color color)
        {
            foreach (var image in rectTransform.GetComponentsInChildren<Image>())
            {
                image.color = color;
            }
        }

        public bool IsBagOpen()
        {
            return _isBagOpen;
        }
        public bool IsCartridgeOpen()
        {
            return _isCartridgeOpen;
        }
        public bool IsSecondaryBagOpen()
        {
            return _isSecondaryBagOpen;
        }
    }
}
