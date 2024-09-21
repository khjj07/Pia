using System;
using Assets.Pia.Scripts.Effect;
using Assets.Pia.Scripts.Interface;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Assets.Pia.Scripts.Game.Items
{
    public class Dagger : UsableItem
    {

        public override void OnUse(Player player)
        {
            if (player.target is CatchButton button)
            {
                button.Press();
                player.UpdateAsObservable().Where(_ => !_isHold)
                    .Take(1).Subscribe(_ =>
                    {
                        button.StopPress();
                    });
            }
            else if (player.target is PressurePlate plate)
            {
                if (plate.IsMovable())
                {
                    player.SetCursorLocked();
                    plate.Initialize();
                    var spinStream = Observable.Interval(TimeSpan.FromSeconds(0.01f))
                        .TakeWhile(_ => plate.IsFinish())
                        .Subscribe(_ => plate.MatchBarMove());

                    var useStream = this.UpdateAsObservable()
                        .TakeWhile(_ => plate.IsFinish())
                        .Where(_ => Input.GetKeyDown(useKey))
                        .Subscribe(_ =>
                        {
                            if (plate.CheckMatchBarInBound())
                            {
                                plate.Operate();
                            }
                            else
                            {
                                plate.ResetCurrent();
                            }
                        }, null, () =>
                        {
                            player.SetCursorUnlocked();
                            plate.ResetAll();
                        });

                    player.UpdateAsObservable().Where(_ => !_isHold)
                        .Take(1).Subscribe(_ =>
                        {
                            player.SetCursorUnlocked();
                            plate.ResetAll();
                            spinStream.Dispose();
                            useStream.Dispose();
                        });
                }

            }
        }
    }
}