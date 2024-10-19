using System.Threading.Tasks;
using Default.Scripts.Util;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Pia.Scripts.Manager
{
    public class StoryModeLoadingManager : GlobalLoadingManager
    {
        protected override void OnLoadBegin()
        {
            GlobalFadeManager.FadeOut();
        }
        protected override void OnLoadEnd()
        {
            GlobalFadeManager.FadeIn();
        }
    }
}