using Default.Scripts.Util;
using DG.Tweening;
using UnityEngine;

namespace Default.Scripts.Printer
{
    [CreateAssetMenu(fileName = "new TextAnimationStyle", menuName = "Printer/Print Style")]

    public class PrintStyle : ScriptableObject
    {
        public enum Unit
        {
            Letter,
            Word,
            Sentence
        }
        public string name = "default";

        public float appearInterval = 0.1f;
        public float repeatInterval = 0.1f;
        public float disappearInterval = 0.1f;

        private bool alwaysDisable = false;

        [Header("단위")]
        public Unit appearAndRepeatUnit;
        public Unit disappearUnit;
#if UNITY_EDITOR
        [Header("나타내기")]
        public bool useAppearAnimation;

        [Header("Scale")]
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useAppearAnimation))]
        public float appearScaleSpeed = 1;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useAppearAnimation))]
        public Vector3 appearBeginScale = Vector3.one;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(alwaysDisable))]
        public Vector3 appearEndScale = Vector3.one;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useAppearAnimation))]
        public Ease appearScaleEase;
        [Header("Position")]
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useAppearAnimation))]
        public float appearPositionSpeed = 1;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useAppearAnimation))]
        public Vector3 appearBeginPosition;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(alwaysDisable))]
        public Vector3 appearEndPosition;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useAppearAnimation))]
        public Ease appearPositionEase;

        [Header("Rotation")]
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useAppearAnimation))]
        public float appearRotationSpeed = 1;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useAppearAnimation))]
        public Vector3 appearBeginRotation;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(alwaysDisable))]
        public Vector3 appearEndRotation;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useAppearAnimation))]
        public Ease appearRotationEase;

        [Header("Color")]
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useAppearAnimation))]
        public float appearColorSpeed = 1;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useAppearAnimation))]
        public Color appearBeginColor = Color.black;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useAppearAnimation))]
        public Color appearEndColor = Color.black;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useAppearAnimation))]
        public Ease appearColorEase;


        [Header("반복")]
        public bool useRepeatAnimation;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useRepeatAnimation))]
        public LoopType repeatLoopType;

        [Header("Scale")]
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useRepeatAnimation))]
        public float repeatScaleSpeed = 1;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useRepeatAnimation))]
        public Vector3 repeatBeginScale = Vector3.one;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useRepeatAnimation))]
        public Vector3 repeatEndScale = Vector3.one;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useRepeatAnimation))]
        public Ease repeatScaleEase;

        [Header("Position")]
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useRepeatAnimation))]
        public float repeatPositionSpeed = 1;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useRepeatAnimation))]
        public Vector3 repeatBeginPosition;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useRepeatAnimation))]
        public Vector3 repeatEndPosition;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useRepeatAnimation))]
        public Ease repeatPositionEase;

        [Header("Rotation")]
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useRepeatAnimation))]
        public float repeatRotationSpeed = 1;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useRepeatAnimation))]
        public Vector3 repeatBeginRotation;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useRepeatAnimation))]
        public Vector3 repeatEndRotation;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useRepeatAnimation))]
        public Ease repeatRotationEase;

        [Header("Color")]
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useRepeatAnimation))]
        public float repeatColorSpeed = 1;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useRepeatAnimation))]
        public Color repeatBeginColor = Color.black;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useRepeatAnimation))]
        public Color repeatEndColor = Color.black;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useRepeatAnimation))]
        public Ease repeatColorEase;

        [Header("사라지기")]
        public bool useDisappearAnimation;

        [Header("Scale")]
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useDisappearAnimation))]
        public float disappearScaleSpeed = 1;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useDisappearAnimation))]
        public Vector3 disappearBeginScale = Vector3.one;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useDisappearAnimation))]
        public Vector3 disappearEndScale = Vector3.one;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useDisappearAnimation))]
        public Ease disappearScaleEase;
        [Header("Position")]
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useDisappearAnimation))]
        public float disappearPositionSpeed = 1;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useDisappearAnimation))]
        public Vector3 disappearBeginPosition;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useDisappearAnimation))]
        public Vector3 disappearEndPosition;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useDisappearAnimation))]
        public Ease disappearPositionEase;

        [Header("Rotation")]
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useDisappearAnimation))]
        public float disappearRotationSpeed = 1;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useDisappearAnimation))]
        public Vector3 disappearBeginRotation;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useDisappearAnimation))]
        public Vector3 disappearEndRotation;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useDisappearAnimation))]
        public Ease disappearRotationEase;

        [Header("Color")]
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useDisappearAnimation))]
        public float disappearColorSpeed = 1;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useDisappearAnimation))]
        public Color disappearBeginColor = Color.black;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useDisappearAnimation))]
        public Color disappearEndColor = Color.black;
        [ShowIf(ShowIfAttribute.ActionOnConditionFail.JustDisable, ShowIfAttribute.ConditionOperator.And, nameof(useDisappearAnimation))]
        public Ease disappearColorEase;

#else
    [Header("나타내기")]
    public bool useAppearAnimation;
    [Header("Scale")]
    public float appearScaleSpeed = 1;
    public Vector3 appearBeginScale = Vector3.one;
    public Vector3 appearEndScale = Vector3.one;
    public Ease appearScaleEase;
    [Header("Position")]
    public float appearPositionSpeed = 1;
    public Vector3 appearBeginPosition;
    public Vector3 appearEndPosition;
    
    public Ease appearPositionEase;

    [Header("Rotation")]
    
    public float appearRotationSpeed = 1;
    
    public Vector3 appearBeginRotation;
    public Vector3 appearEndRotation;
    
    public Ease appearRotationEase;

    [Header("Color")]
    
    public float appearColorSpeed = 1;
    
    public Color appearBeginColor = Color.black;
    
    public Color appearEndColor = Color.black;
    
    public Ease appearColorEase;


    [Header("반복")]
    public bool useRepeatAnimation;
    
    public LoopType repeatLoopType;

    [Header("Scale")]
    
    public float repeatScaleSpeed = 1;
    
    public Vector3 repeatBeginScale = Vector3.one;
    
    public Vector3 repeatEndScale = Vector3.one;
    
    public Ease repeatScaleEase;

    [Header("Position")]
    
    public float repeatPositionSpeed = 1;
    
    public Vector3 repeatBeginPosition;
    
    public Vector3 repeatEndPosition;
    
    public Ease repeatPositionEase;

    [Header("Rotation")]
    
    public float repeatRotationSpeed = 1;
   
    public Vector3 repeatBeginRotation;
  
    public Vector3 repeatEndRotation;
 
    public Ease repeatRotationEase;

    [Header("Color")]
 
    public float repeatColorSpeed = 1;
    public Color repeatBeginColor = Color.black;
  
    public Color repeatEndColor = Color.black;
    
    public Ease repeatColorEase;
      [Header("사라지기")]
        public bool useDisppearAnimation;

        [Header("Scale")]
       public float disappearScaleSpeed = 1;
       public Vector3 disappearBeginScale = Vector3.one;
 
        public Vector3 disappearEndScale = Vector3.one;
       
        public Ease disappearScaleEase;
        [Header("Position")]
      
        public float disappearPositionSpeed = 1;

        public Vector3 disappearBeginPosition;
      
        public Vector3 disappearEndPosition;
   
        public Ease disappearPositionEase;

        [Header("Rotation")]
       
        public float disappearRotationSpeed = 1;
 
        public Vector3 disappearBeginRotation;
    
        public Vector3 disappearEndRotation;
    
        public Ease disappearRotationEase;

        [Header("Color")]
       
        public float disappearColorSpeed = 1;
      
        public Color disappearBeginColor = Color.black;
      
        public Color disappearEndColor = Color.black;
      
        public Ease disappearColorEase;
#endif
    }
}