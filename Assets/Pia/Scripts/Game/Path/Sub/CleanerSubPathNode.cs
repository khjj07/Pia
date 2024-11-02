using System.Threading;
using System.Threading.Tasks;

namespace Assets.Pia.Scripts.Path.Sub
{
    public class CleanerSubPathNode : SubPathNode
    {

        private void Start()
        {
      
        }
        public override Task Appear(CancellationTokenSource cancellationTokenSource)
        {
            return Task.CompletedTask;
        }
        public override Task Disappear(CancellationTokenSource cancellationTokenSource)
        {
           return Task.CompletedTask;
        }
    }
}