using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Assets.Pia.Scripts.Game.UI
{ 
    public class GlobalConfiguration : MonoBehaviour
    {
        [SerializeField] private Volume volume;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public void SetExposure(float value)
        {
            VolumeProfile profile = volume.sharedProfile;
        
            if (profile.TryGet<Exposure>(out var exposure))
            {
                exposure.fixedExposure.value = value;
            }
        }

        public float GetExposure()
        {
            VolumeProfile profile = volume.sharedProfile;

            if (profile.TryGet<Exposure>(out var exposure))
            {
                return exposure.fixedExposure.value;
            }

            return 0;
        }
    }
}
