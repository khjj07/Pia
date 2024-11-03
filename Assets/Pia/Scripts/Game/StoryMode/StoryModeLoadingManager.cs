using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Default.Scripts.Util;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Pia.Scripts.Manager
{
    public class StoryModeLoadingManager : GlobalLoadingManager
    {
        protected override IEnumerator OnLoadBegin()
        {
            GlobalFadeManager.FadeOut();
            yield return new WaitForSeconds(GlobalFadeManager.Instance.fadeDuration);
        }
        protected override IEnumerator OnLoadEnd()
        {
            GlobalFadeManager.FadeIn();
            yield return new WaitForSeconds(GlobalFadeManager.Instance.fadeDuration);
        }
    }
}