using System;
using System.Threading.Tasks;
using Default.Scripts.Util;
using DG.Tweening;
using Pia.Scripts.Dialog;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Pia.Scripts.Synopsis
{
    public class SynopsisTextbookState : SynopsisState
    {
        [SerializeField] private Sprite[] pages;

        [SerializeField] private Image book;

        [SerializeField] private Image arrow;


        private int _currentPage = 0;

        public override bool CanGoNext()
        {
            return !_isAppearing && _currentPage >= pages.Length-1;
        }

        public override async Task OnEnter()
        {
            await base.OnEnter();
            CreateReadingBookStream();
        }

        private void CreateReadingBookStream()
        {
            GlobalInputBinder.CreateGetKeyDownStream(KeyCode.E).TakeWhile(_=> _currentPage < pages.Length - 1).Subscribe(_=>NextPage()).AddTo(gameObject);
        }

        private void NextPage()
        {
            if (_currentPage < pages.Length - 1)
            {
                _currentPage++;
                book.sprite = pages[_currentPage];
                if (_currentPage == pages.Length - 1)
                {
                    arrow.DOColor(new Color(), 1.0f).OnComplete(() =>
                    {
                        arrow.gameObject.SetActive(false);
                    });
                }
               
            }
        }
    }
}