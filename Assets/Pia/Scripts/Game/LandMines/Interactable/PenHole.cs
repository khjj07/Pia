using Assets.Pia.Scripts.Game;
using Assets.Pia.Scripts.Game.Items;
using Assets.Pia.Scripts.Interface;
using Assets.Pia.Scripts.StoryMode;
using Default.Scripts.Sound;
using DG.Tweening;
using Pia.Scripts.StoryMode;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Pia.Scripts.Interactable
{
    public class PenHole : InteractableClass
    {
        [SerializeField] private Transform pen;

        private bool inserted = false;
        public override void OnHover(Item item)
        {
            if (item is Pen)
            {
                availableOutline.gameObject.SetActive(true);
            }
            else
            {
                inavailableOutline.gameObject.SetActive(true);
            }
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public void InsertPen()
        {
            if (!inserted)
            {
                pen.gameObject.SetActive(true);
                pen.DOMove(transform.position, 1.0f)
                    .OnComplete(StoryModeManager.Instance.GameClear);
                inserted= true;
                SoundManager.Play("use_penInsert", 1); ;

            }
        }
    }
}
