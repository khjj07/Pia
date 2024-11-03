using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Default.Scripts.Printer
{
#if UNITY_EDITOR
    [CustomEditor(typeof(GlobalPrinterSetting))]
    public class GlobalPrinterSettingEditor : Editor
    {
        public void OnValidate()
        {
            var setting = target as GlobalPrinterSetting;
#if UNITY_EDITOR
            EditorUtility.SetDirty(target);
#endif
        }
    }
#endif

    [CreateAssetMenu(fileName = "new GlobalPrinterSetting", menuName = "Printer/Global Printer Setting")]
    public class GlobalPrinterSetting : Util.ScriptableSingleton<GlobalPrinterSetting>
    {
        public PrintStyle defaultTextAnimationStyle;

        public PrintStyle skipTextAnimationStyle;

        public PrintStyle[] dialogStyles;

        public PrintStyle FindDialogStyle(string name)
        {
            return dialogStyles.ToList().Find((x) => x.name == name);
        }
    }
}