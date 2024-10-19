using System.Threading.Tasks;
using Default.Scripts.Sound;

namespace Assets.Pia.Scripts.StoryMode.Synopsis.Sub
{
    public class StoryBoardSubSoundStopState : StoryBoardSubState
    {
        public override async Task Appear()
        {
            await Task.Delay((int)(appearDelay * 1000));
            SoundManager.Stop(0);
        }
    }
}