namespace Assets.Pia.Scripts.Game.Items
{
    public class Bandage : UsableItem
    {
        public override void OnUse(Player player)
        {
            if (player.IsBleeeding())
            {
                //붕대 미니게임
            }
        }
    }
}