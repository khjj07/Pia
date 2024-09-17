using Pia.Scripts.StoryMode;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class BoltState : LandMineState
{
    [SerializeField] private Bolt[] bolts;

    public override bool IsClear()
    {
        foreach (var bolt in bolts)
        {
            if (!bolt.isDead)
            {
                return false;
            }
        }
        return true;
    }

}
