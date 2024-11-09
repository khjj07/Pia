using Assets.Pia.Scripts.Effect;
using Assets.Pia.Scripts.Interactable;
using Assets.Pia.Scripts.Interface;
using Default.Scripts.Sound;

namespace Assets.Pia.Scripts.Game.Items
{
    public class Pen : UsableItem
    {

        public override void OnUse(Player player)
        {
            if (player.target is PenHole penHole)
            {
                penHole.InsertPen();
            }
        }
    }
}