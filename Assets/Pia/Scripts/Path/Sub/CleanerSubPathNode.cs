using System.Threading.Tasks;

namespace Assets.Pia.Scripts.Path.Sub
{
    public class CleanerSubPathNode : SubPathNode
    {

        private void Start()
        {
      
        }
        public override Task Appear()
        {
            return Task.CompletedTask;
        }
        public override Task Disappear()
        {
           return Task.CompletedTask;
        }
    }
}