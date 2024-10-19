using Assets.Pia.Scripts.StoryMode;
using Pia.Scripts.StoryMode;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class SpringState : LandMineState
{
    [SerializeField] private Spring[] springs;

    public void Start()
    {
        int boltIndex = Random.Range(0, springs.Length - 1);

        this.UpdateAsObservable()
            .SkipWhile(_ => !springs[boltIndex].isDead)
            .Take(1)
            .Subscribe(_ => EventManager.InvokeEvent(EventManager.Event.Enemy));
    }
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
