using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Events;


namespace UnityEngine.Localization.Components
{
    /// <summary>
    /// Component that can be used to Localize a [Prefab](https://docs.unity3d.com/Manual/Prefabs.html).
    /// When the Locale is changed the prefab will be instantiated as a child of the gameobject this component is attached to, the instance will then be sent through <see cref="LocalizedAssetEvent{TObject, TReference, TEvent}.OnUpdateAsset"/>.
    /// </summary>
    [AddComponentMenu("Localization/Asset/Localize Custom Prefab Event")]
    public class LocalizedCustomGameObjectEvent : LocalizedAssetEvent<GameObject, LocalizedGameObject, UnityEventGameObject>
    {
        GameObject m_Current;

        /// <inheritdoc/>
        protected override void UpdateAsset(GameObject localizedAsset)
        {
            Debug.Log(m_Current);
            if (m_Current != null)
            {
                Destroy(m_Current);
                m_Current = null;
            }

            if (localizedAsset != null)
            {
                m_Current = Instantiate(localizedAsset, transform);
                m_Current.hideFlags = HideFlags.NotEditable;
            }

            OnUpdateAsset.Invoke(m_Current);
        }
    }
}


