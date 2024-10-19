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


        public override async Task Appear()
        {
            await Task.Delay((int)(appearDelay * 1000));
           SoundManager.Play(bgmName,channel);
        }
    }
}