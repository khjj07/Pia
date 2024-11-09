using System;
using System.Threading;
using System.Threading.Tasks;
using Default.Scripts.Sound;
using Default.Scripts.Util;
using DG.Tweening;
using Pia.Scripts.Dialog;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Pia.Scripts.Synopsis
{
    public class StoryBoardTextbookState : StoryBoardState
    {
        [SerializeField] private Sprite[] pages;

        [SerializeField] private Image book;

        [SerializeField] private Image arrow;


        private int _currentPage = 0;
        private bool _lastPageFlag;
    
        public override bool CanGoNext()
        {
            return !_isAppearing && _lastPageFlag;
        }

        public override async Task OnEnter(CancellationTokenSource tokenSource)
        {
            CreateReadingBookStream();
            await base.OnEnter(tokenSource);
        }

        private void CreateReadingBookStream()
        {
            var nextSream = GlobalInputBinder.CreateGetKeyDownStream(KeyCode.E).Where(_=> _currentPage < pages.Length - 1).Subscribe(_=>NextPage()).AddTo(gameObject);
            var prevSream = GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Q).Where(_=> _currentPage >0).Subscribe(_=>PreviousPage()).AddTo(gameObject);
            GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Mouse0).Where(_ => CanGoNext()).Take(1).Subscribe(_ =>
            {
                nextSream.Dispose();
                prevSream.Dispose();
            }).AddTo(gameObject);
        }

        private void NextPage()
        {
            if (_currentPage < pages.Length - 1)
            {
                SoundManager.Play("use_book", 1);
                _currentPage++;
                book.sprite = pages[_currentPage];
                arrow.gameObject.SetActive(true);
            }

            if (_currentPage >= pages.Length - 1)
            {
                _lastPageFlag = true;
                arrow.gameObject.SetActive(false);
            }
        }
        private void PreviousPage()
        {
            if (_currentPage > 0)
            {
                SoundManager.Play("use_book", 1);
                _currentPage--;
                book.sprite = pages[_currentPage];
                arrow.gameObject.SetActive(true);
            }
        }
    }
}