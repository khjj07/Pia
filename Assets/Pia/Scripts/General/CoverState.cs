using Pia.Scripts.StoryMode;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class CoverState : LandMineState
{
    [SerializeField] private Cover[] covers;

    public override bool IsClear()
    {
        foreach (var cover in covers)
        {
            if (!cover.isDead)
            {
                return false;
            }
        }
        return true;
    }
}
