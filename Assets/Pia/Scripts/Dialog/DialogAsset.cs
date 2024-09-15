using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pia.Scripts.Dialog
{
#if UNITY_EDITOR
    [CustomEditor(typeof(DialogAsset))]
    public class DialogAssetInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var da = (DialogAsset)target;
            if (GUILayout.Button("Generate Texts From Dialog File"))
            {
                da.GeneratePhrasesTextsFromFile(da.dialogFile);
            }
        }
    }
#endif

    [CreateAssetMenu(fileName = "new Dialog Asset",menuName = "Printer/Dialog Asset")]
    public class DialogAsset : ScriptableObject
    {
        public List<string> texts;
        public TextAsset dialogFile;

        public void GeneratePhrasesTextsFromFile(TextAsset file)
        {
            EditorUtility.SetDirty(this);
            string[] lines = file.text.Split('\n');
            texts = new List<string>();
            for (int i = 0; lines.Length > i; i++)
            {
                texts.Add(lines[i]);
            }
        }
    }
}