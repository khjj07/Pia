namespace Assets.Pia.Scripts.Game.Items
{
    public class GuideBook : Item
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