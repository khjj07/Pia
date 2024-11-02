using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Pia.Scripts.StoryMode;
using TMPro;
using UniRx;
using UniRx.Triggers;

namespace Assets.Pia.Scripts.Path
{
    public class MineNode : SubPathNode
    {
        public override async Task Appear(CancellationTokenSource cancellationTokenSource)
        {
            StoryModeManager.SetState(StoryModeManager.State.LandMineDirt);
        }

        public override Task Disappear(CancellationTokenSource cancellationTokenSource)
        {
            return Task.CompletedTask;
        }


    }

}