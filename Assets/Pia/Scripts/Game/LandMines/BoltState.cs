using Assets.Pia.Scripts.StoryMode;
using Pia.Scripts.StoryMode;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class BoltState : LandMineState
{
    [SerializeField] private Bolt[] bolts;

    public void Start()
    {
        int boltIndex = Random.Range(0, bolts.Length-1);

        this.UpdateAsObservable()
            .SkipWhile(_ => !bolts[boltIndex].isDead)
            .Take(1)
            .Subscribe(_ => EventManager.InvokeEvent(EventManager.Event.Boar));
    }


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
