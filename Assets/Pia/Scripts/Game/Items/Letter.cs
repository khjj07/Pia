namespace Assets.Pia.Scripts.Game.Items
{
    public class Letter : Item
    {
        public override void OnHold(Player player)
        {
            base.OnHold(player);
            gameObject.SetActive(true);
        }
        public override void OnStopHold()
        {
            base.OnStopHold();
            gameObject.SetActive(false);
        }
    }
}