using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Pia.Scripts.UI.Slot
{
    public class SlotUI : MonoBehaviour
    {
        public Color activateColor = Color.yellow;
        public Color inactivateColor = Color.white;

        protected Tween _tween;

      
        public virtual Tween SetActive(bool value)
        {
            if (value)
            {
                ChangeSlotColor(activateColor);
            }
            else
            {
                ChangeSlotColor(inactivateColor);
            }

            return null;
        }
     

        public void ChangeSlotColor(Color color)
        {
            foreach (var image in GetComponentsInChildren<Image>())
            {
                image.color = color;
            }
        }
        public void CancelTween(Tween tween)
        {
            if (tween != null)
            {
                tween.Kill();
            }
        }
    }
}