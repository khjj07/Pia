using System;
using System.Threading;
using System.Threading.Tasks;
using Default.Scripts.Sound;
using UnityEngine;

namespace Assets.Pia.Scripts.StoryMode.Synopsis.Sub
{
    public class StoryBoardSubSoundPlayState : StoryBoardSubState
    {
        [SerializeField]
        private int channel=0;
        [SerializeField]
        private string bgmName;


        public override async Task Appear(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                await Task.Delay((int)(appearDelay * 1000), cancellationTokenSource.Token);
                SoundManager.Play(bgmName, channel);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Async task was canceled.");
            }
        }
    }
}