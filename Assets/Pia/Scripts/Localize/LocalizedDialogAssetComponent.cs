using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace Assets.Pia.Scripts.StoryMode.StoryBoard
{
    [Serializable]
    public class LocalizedDialogAsset : LocalizedAsset<DialogAsset> { }
    public class LocalizedDialogAssetComponent : LocalizedAssetBehaviour<DialogAsset, LocalizedDialogAsset>
    {
        protected override void UpdateAsset(DialogAsset localizedAsset)
        {
            throw new NotImplementedException();
        }
    }
}