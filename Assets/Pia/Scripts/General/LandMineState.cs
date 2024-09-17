using System.Threading.Tasks;
using Assets.Pia.Scripts.Interface;
using Default.Scripts.Util.StatePattern;
using Unity.VisualScripting;
using UnityEngine;

public class LandMineState : State<LandMineState>
{
    public override Task OnEnter()
    {
        foreach (var interatable in GetComponentsInChildren<InteractableClass>())
        {
            interatable.SetAvailable(true);
            interatable.GetComponent<Collider>().enabled = true;
        }
        return Task.CompletedTask;
    }

    public override Task OnExit()
    {
        foreach (var interatable in GetComponentsInChildren<InteractableClass>())
        {
            interatable.SetAvailable(false);
        }
        return Task.CompletedTask;
    }

    public virtual bool IsClear()
    {
        return false;
    }
}
