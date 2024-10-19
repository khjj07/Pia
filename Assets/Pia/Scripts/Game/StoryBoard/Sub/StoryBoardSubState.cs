using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Pia.Scripts.StoryMode.Synopsis.Sub
{
    public class StoryBoardSubState : MonoBehaviour
    {
        public float appearDelay = 0.0f;
        public virtual async Task Appear()
        {
            await Task.Delay((int)(appearDelay * 1000));
            gameObject.SetActive(true);
        }
    }
}
