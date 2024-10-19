using System;
using System.Collections;
using System.Threading.Tasks;
using Pia.Scripts.StoryMode;
using TMPro;
using UniRx;
using UniRx.Triggers;

namespace Assets.Pia.Scripts.Path
{
    public class MineNode : SubPathNode
    {
        public override async Task Appear()
        {
            StoryModeManager.SetState(StoryModeManager.State.LandMineDirt);
        }

        public override Task Disappear()
        {
            throw new System.NotImplementedException();
        }


    }

}