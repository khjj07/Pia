using UnityEngine;

public class PressurePlateState : LandMineState
{
    [SerializeField]
    private PressurePlate pressurePlate;
    public override bool IsClear()
    {
        if (pressurePlate.isDead)
        {
            return true;
        }
        else
        {
            return false;
        }
       
    }
}
