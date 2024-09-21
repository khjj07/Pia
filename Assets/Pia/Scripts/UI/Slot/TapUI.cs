using Assets.Pia.Scripts.Game.Items;
using Assets.Pia.Scripts.UI.Slot;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Pia.Scripts.UI
{
    public class TapUI : SlotUI
    {
        public RectTransform tap;

        public override Tween SetActive(bool value)
        {
            if (value)
            {
                return Open();
            }
            else
            {
                return Close();
            }
        }
        private Tween Open()
        {
            tap.gameObject.SetActive(true);
            tap.localScale = Vector3.zero;
            ChangeSlotColor(activateColor);
            CancelTween(_tween);
            _tween = tap.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutElastic);
            return _tween;
        }

        private Tween Close()
        {
            CancelTween(_tween);
            _tween = tap.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                tap.gameObject.SetActive(false);
                ChangeSlotColor(inactivateColor);
            });
            return _tween;
        }
    }
}
