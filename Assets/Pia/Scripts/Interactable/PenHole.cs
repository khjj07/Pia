using Assets.Pia.Scripts.Interface;
using Assets.Pia.Scripts.StoryMode;
using DG.Tweening;
using Pia.Scripts.StoryMode;
using UnityEngine;

namespace Assets.Pia.Scripts.Interactable
{
    public class PenHole : InteractableClass
    {
        [SerializeField] private Transform pen;
        public override void OnHover(Player.UpperState state)
        {
            if (Player.UpperState.Pen == state)
            {
                availableOutline.gameObject.SetActive(true);
            }
            else
            {
                inavailableOutline.gameObject.SetActive(true);
            }
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnInteract(object player)
        {
            pen.gameObject.SetActive(true);
            pen.DOMove(transform.position, 1.0f)
                .OnComplete(StoryModeManager.GameClear);
        }
    }
}
