using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Assets.Pia.Scripts.StoryMode.StoryBoard
{
    [CustomEditor(typeof(DialogAsset))]
    public class DialogAssetInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var asset = (DialogAsset)target;
            if (GUILayout.Button("Generate All Dialog From Resource File"))
            {
                asset.GenerateDiaglogsFromResourceFile();
            }
            if (GUILayout.Button("Reset All Dialog"))
            {
                asset.ResetAllDialog();
            }
        }
    }

    [CreateAssetMenu(fileName = "Story Board Dialog Asset",menuName = "Story Board/Dialog Asset")]
    public class DialogAsset : ScriptableObject
    {
        [SerializeField] TMP_FontAsset fontAsset;
        [SerializeField] private string[] dialogs;
        [SerializeField] TextAsset resourceFile;
        public string GetDialog(int index)
        {
            return dialogs[index];
        }
        public TMP_FontAsset GetFontAsset()
        {
            return fontAsset;
        }

        public void GenerateDiaglogsFromResourceFile()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
            string[] lines = resourceFile.text.Split('#');
            var list = new List<string>();
            for (int i = 0; lines.Length > i; i++)
            {
                list.Add(lines[i]);
            }
            dialogs=list.ToArray();
        }

        public void ResetAllDialog()
        {

        }
    }
}
