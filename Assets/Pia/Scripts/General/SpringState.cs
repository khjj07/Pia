using Pia.Scripts.StoryMode;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class SpringState : LandMineState
{
    [SerializeField] private Spring[] springs;

    public override bool IsClear()
    {
        foreach (var spring in springs)
        {
            if (!spring.isDead)
            {
                return false;
            }
        }
        return true;
    }
}
