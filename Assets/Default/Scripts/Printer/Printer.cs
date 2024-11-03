using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Default.Scripts.Printer
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class Printer : MonoBehaviour
    {
        [SerializeField] private bool stayPosition = true;

        private string originalText;
        private TMP_Text _textComponent;
        private List<string> _parsedText;
        private StringBuilder _currentText;
        private int _parsedTextIndex;

        private Vector3[] _textMeshPosition;
        private Vector3[] _textMeshScale;
        private Vector3[] _textMeshRotation;
        private Color[] _textMeshColor;

        private Tween[] _textMeshScaleAppearTween;
        private Tween[] _textMeshPositionAppearTween;
        private Tween[] _textMeshRotationAppearTween;
        private Tween[] _textMeshColorAppearTween;

        private Tween[] _textMeshScaleRepeatTween;
        private Tween[] _textMeshPositionRepeatTween;
        private Tween[] _textMeshRotationRepeatTween;
        private Tween[] _textMeshColorRepeatTween;  
        
        private Tween[] _textMeshScaleDisappearTween;
        private Tween[] _textMeshPositionDisappearTween;
        private Tween[] _textMeshRotationDisappearTween;
        private Tween[] _textMeshColorDisappearTween;

        private List<PrintStyle> _dialogStyle;

        private bool _isPrinting = false;
        private bool _isAppeared = false;
        private IEnumerator _printRoutine;

    

        //event
        public UnityEvent onAppearEvent;
        public UnityEvent onDisappearEvent;
        public UnityEvent onBeginPrintEvent;
        public UnityEvent onEndPrintEvent;

        private void Awake()
        {
            _textComponent = GetComponent<TextMeshProUGUI>();
        }

        public bool IsPrinting()
        {
            return _isPrinting;
        } 
        public bool IsAppeared()
        {
            return _isAppeared;
        }

        public void SetOriginalText(string newText)
        {
            originalText = newText;
            _parsedText = new List<string>();
            _dialogStyle = new List<PrintStyle>();

            string pattern = @"<(?<tag>\w+)>(?<value>.*?)<\/\w+>|([^<>]+)";
            string pattern1 = @"<(?<tag>\w+)>(?<value>.*?)<\/\w+>";

            MatchCollection matches = Regex.Matches(originalText, pattern);
            foreach (Match match in matches)
            {
                var realMatch = Regex.Match(match.Value, pattern1);
                if (realMatch.Success)
                {
                    string sytleName = match.Groups["tag"].Value;
                    string value = match.Groups["value"].Value;

                    PrintStyle style = GlobalPrinterSetting.instance.FindDialogStyle(sytleName);
                    _parsedText.Add(value);
                    if (style != null)
                    {
                        _dialogStyle.Add(style);
                    }
                    else
                    {
                        _dialogStyle.Add(GlobalPrinterSetting.instance.defaultTextAnimationStyle);
                    }
                }
                else
                {
                    _parsedText.Add(match.Value);
                    _dialogStyle.Add(GlobalPrinterSetting.instance.defaultTextAnimationStyle);
                }
            }
            ResetCurrentText();
        }

        public int GetFullParsedTextLength()
        {
            int sum = 0;
            foreach (var x in _parsedText)
            {
                sum += x.Count();
            }
            return sum;
        }

        public void ResetCurrentText()
        {
            int count = GetFullParsedTextLength();
            _currentText = new StringBuilder(count);
            _textMeshPosition = new Vector3[count];
            _textMeshScale = new Vector3[count];
            _textMeshRotation = new Vector3[count];
            _textMeshColor = new Color[count];

            for (int i = 0; i < count; i++)
            {
                _textMeshScale[i] = Vector3.one;
                _textMeshColor[i] = new Color();
            }

            _textMeshScaleAppearTween = new Tween[count];
            _textMeshPositionAppearTween = new Tween[count];
            _textMeshRotationAppearTween = new Tween[count];
            _textMeshColorAppearTween = new Tween[count];

            _textMeshScaleRepeatTween = new Tween[count];
            _textMeshPositionRepeatTween = new Tween[count];
            _textMeshRotationRepeatTween = new Tween[count];
            _textMeshColorRepeatTween = new Tween[count];

            _textMeshScaleDisappearTween = new Tween[count];
            _textMeshPositionDisappearTween = new Tween[count];
            _textMeshRotationDisappearTween = new Tween[count];
            _textMeshColorDisappearTween = new Tween[count];

            _textComponent.text=_currentText.ToString();
        }

        public void PrintCurrentLetter(char c)
        {
            _currentText.Append(c);
            _textComponent.text = _currentText.ToString();
        }
        public Tween DisappearScaleTween(int index, Vector3 startScale, Vector3 endScale, float duration, Ease ease)
        {
            _textMeshScale[index] = startScale;
            return DOTween.To(() => _textMeshScale[index], x => _textMeshScale[index] = x, endScale, duration).SetEase(ease);
        }

        public Tween DisappearPositionTween(int index, Vector3 startPosition, Vector3 endPosition, float duration, Ease ease)
        {
            _textMeshPosition[index] = startPosition;
            return DOTween.To(() => _textMeshPosition[index], x => _textMeshPosition[index] = x, endPosition, duration).SetEase(ease);
        }

        public Tween DisappearRotationTween(int index, Vector3 startRotation, Vector3 endRotation, float duration, Ease ease)
        {
            _textMeshRotation[index] = startRotation;
            return DOTween.To(() => _textMeshRotation[index], x => _textMeshRotation[index] = x, endRotation, duration).SetEase(ease);
        }

        public Tween DisappearColorTween(int index, Color startColor, Color endColor, float duration, Ease ease)
        {
            _textMeshColor[index] = startColor;
            return DOTween.To(() => _textMeshColor[index], x => _textMeshColor[index] = x, endColor, duration).SetEase(ease);
        }

        public Tween AppearScaleTween(int index, Vector3 startScale, Vector3 endScale, float duration, Ease ease)
        {
            _textMeshScale[index] = startScale;
            return DOTween.To(() => _textMeshScale[index], x => _textMeshScale[index] = x, endScale, duration).SetEase(ease);
        }

        public Tween AppearPositionTween(int index, Vector3 startPosition, Vector3 endPosition, float duration, Ease ease)
        {
            _textMeshPosition[index] = startPosition;
            return DOTween.To(() => _textMeshPosition[index], x => _textMeshPosition[index] = x, endPosition, duration).SetEase(ease);
        }

        public Tween AppearRotationTween(int index, Vector3 startRotation, Vector3 endRotation, float duration, Ease ease)
        {
            _textMeshRotation[index] = startRotation;
            return DOTween.To(() => _textMeshRotation[index], x => _textMeshRotation[index] = x, endRotation, duration).SetEase(ease);
        }

        public Tween AppearColorTween(int index, Color startColor, Color endColor, float duration, Ease ease)
        {
            _textMeshColor[index] = startColor;
            return DOTween.To(() => _textMeshColor[index], x => _textMeshColor[index] = x, endColor, duration).SetEase(ease);
        }

        public Tween RepeatScaleTween(int index, Vector3 startScale, Vector3 endScale, float duration, Ease ease, LoopType loopType, int loopTime = -1)
        {
            _textMeshScale[index] = startScale;
            return DOTween.To(() => _textMeshScale[index], x => _textMeshScale[index] = x, endScale, duration).SetEase(ease).SetLoops(loopTime, loopType);
        }

        public Tween RepeatPositionTween(int index, Vector3 startPosition, Vector3 endPosition, float duration, Ease ease, LoopType loopType, int loopTime = -1)
        {
            _textMeshPosition[index] = startPosition;
            return DOTween.To(() => _textMeshPosition[index], x => _textMeshPosition[index] = x, endPosition, duration).SetEase(ease).SetLoops(loopTime, loopType);
        }

        public Tween RepeatRotationTween(int index, Vector3 startRotation, Vector3 endRotation, float duration, Ease ease, LoopType loopType, int loopTime = -1)
        {
            _textMeshRotation[index] = startRotation;
            return DOTween.To(() => _textMeshRotation[index], x => _textMeshRotation[index] = x, endRotation, duration).SetEase(ease).SetLoops(loopTime, loopType);
        }

        public Tween RepeatColorTween(int index, Color startColor, Color endColor, float duration, Ease ease, LoopType loopType, int loopTime = -1)
        {
            _textMeshColor[index] = startColor;
            return DOTween.To(() => _textMeshColor[index], x => _textMeshColor[index] = x, endColor, duration).SetEase(ease).SetLoops(loopTime, loopType);
        }

        private void LateUpdate()
        {
            if (_textComponent.text.Length > 0)
            {
                _textComponent.ForceMeshUpdate();
                var mesh = _textComponent.mesh;
                var textInfo = _textComponent.textInfo;
                Vector3[] vertices = mesh.vertices;
                Color[] colors = mesh.colors;
                for (int i = 0; i < textInfo.characterCount; i++)
                {
                    var characterInfo = textInfo.characterInfo[i];
                    if (!characterInfo.isVisible)
                    {
                        continue;
                    }

                    Vector3 center = Vector3.zero;
                    float halfHeight, halfWidth;

                    halfHeight = Vector3.Distance(vertices[characterInfo.vertexIndex], vertices[characterInfo.vertexIndex + 1]) / 2;
                    halfWidth = Vector3.Distance(vertices[characterInfo.vertexIndex + 1], vertices[characterInfo.vertexIndex + 2]) / 2;

                    for (int j = 0; j < 4; j++)
                    {
                        var origin = vertices[characterInfo.vertexIndex + j];

                        center += origin;
                    }
                    center /= 4;

                    vertices[characterInfo.vertexIndex] = center + _textMeshPosition[i] + Quaternion.Euler(_textMeshRotation[i]) * new Vector3(-halfWidth * _textMeshScale[i].x, -halfHeight * _textMeshScale[i].y, 0);
                    vertices[characterInfo.vertexIndex + 1] = center + _textMeshPosition[i] + Quaternion.Euler(_textMeshRotation[i]) * new Vector3(-halfWidth * _textMeshScale[i].x, halfHeight * _textMeshScale[i].y, 0);
                    vertices[characterInfo.vertexIndex + 2] = center + _textMeshPosition[i] + Quaternion.Euler(_textMeshRotation[i]) * new Vector3(halfWidth * _textMeshScale[i].x, halfHeight * _textMeshScale[i].y, 0);
                    vertices[characterInfo.vertexIndex + 3] = center + _textMeshPosition[i] + Quaternion.Euler(_textMeshRotation[i]) * new Vector3(halfWidth * _textMeshScale[i].x, -halfHeight * _textMeshScale[i].y, 0);
                    colors[characterInfo.vertexIndex] = _textMeshColor[i];
                    colors[characterInfo.vertexIndex + 1] = _textMeshColor[i];
                    colors[characterInfo.vertexIndex + 2] = _textMeshColor[i];
                    colors[characterInfo.vertexIndex + 3] = _textMeshColor[i];
                }

                mesh.colors = colors;
                mesh.vertices = vertices;
                _textComponent.canvasRenderer.SetMesh(mesh);
            }
        }

        public void AppearTween(PrintStyle style, char letter, int letterCount)
        {
            if (!stayPosition)
            {
                PrintCurrentLetter(letter);
            }
            
            if (style.useAppearAnimation)
            {
                _textMeshScaleAppearTween[letterCount] = AppearScaleTween(letterCount, style.appearBeginScale, style.appearEndScale, style.appearInterval / style.appearScaleSpeed, style.appearScaleEase);
                _textMeshPositionAppearTween[letterCount] = AppearPositionTween(letterCount, style.appearBeginPosition, style.appearEndPosition, style.appearInterval / style.appearPositionSpeed, style.appearPositionEase);
                _textMeshRotationAppearTween[letterCount] = AppearRotationTween(letterCount, style.appearBeginRotation, style.appearEndRotation, style.appearInterval / style.appearRotationSpeed, style.appearRotationEase);
                _textMeshColorAppearTween[letterCount] = AppearColorTween(letterCount, style.appearBeginColor, style.appearEndColor, style.appearInterval / style.appearColorSpeed, style.appearColorEase);
            }
        }

        public void DisappearTween(PrintStyle style, char letter, int letterCount)
        {
            if (style.useDisappearAnimation)
            {
                _textMeshScaleDisappearTween[letterCount] = DisappearScaleTween(letterCount, style.disappearBeginScale, style.disappearEndScale, style.disappearInterval / style.disappearScaleSpeed, style.disappearScaleEase);
                _textMeshPositionDisappearTween[letterCount] = DisappearPositionTween(letterCount, style.disappearBeginPosition, style.disappearEndPosition, style.disappearInterval / style.disappearPositionSpeed, style.disappearPositionEase);
                _textMeshRotationDisappearTween[letterCount] = DisappearRotationTween(letterCount, style.disappearBeginRotation, style.disappearEndRotation, style.disappearInterval / style.disappearRotationSpeed, style.disappearRotationEase);
                _textMeshColorDisappearTween[letterCount] = DisappearColorTween(letterCount, style.disappearBeginColor, style.disappearEndColor, style.disappearInterval / style.disappearColorSpeed, style.disappearColorEase);
            }
        }

        public async Task RepeatTween(PrintStyle style, int letterCount, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay((int)(style.appearInterval * 1000), cancellationToken);
                if (style.useRepeatAnimation)
                {
                    _textMeshScaleRepeatTween[letterCount] = RepeatScaleTween(letterCount, style.repeatBeginScale,
                        style.repeatEndScale, style.repeatInterval / style.repeatScaleSpeed,
                        style.repeatScaleEase, style.repeatLoopType);
                    _textMeshPositionRepeatTween[letterCount] = RepeatPositionTween(letterCount, style.repeatBeginPosition,
                        style.repeatEndPosition, style.repeatInterval / style.repeatPositionSpeed,
                        style.repeatPositionEase, style.repeatLoopType);
                    _textMeshRotationRepeatTween[letterCount] = RepeatRotationTween(letterCount, style.repeatBeginRotation,
                        style.repeatEndRotation, style.repeatInterval / style.repeatRotationSpeed,
                        style.repeatRotationEase, style.repeatLoopType);
                    _textMeshColorRepeatTween[letterCount] = RepeatColorTween(letterCount, style.repeatBeginColor,
                        style.repeatEndColor, style.repeatInterval / style.repeatColorSpeed,
                        style.repeatColorEase, style.repeatLoopType);
                }
            }
            catch (OperationCanceledException)
            {
                UnityEngine.Debug.Log("Async task was canceled.");
            }
          
        }

        public void StopPrinting()
        {
            _isPrinting = false;
            _isAppeared = false;
            int length = GetFullParsedTextLength();
            for (int i = 0; i < length; i++)
            {
                _textMeshScaleAppearTween[i].Kill();
                _textMeshPositionAppearTween[i].Kill();
                _textMeshColorAppearTween[i].Kill();
                _textMeshRotationAppearTween[i].Kill();
                _textMeshScaleRepeatTween[i].Kill();
                _textMeshPositionRepeatTween[i].Kill();
                _textMeshRotationRepeatTween[i].Kill();
                _textMeshColorRepeatTween[i].Kill();
                _textMeshScaleDisappearTween[i].Kill();
                _textMeshPositionDisappearTween[i].Kill();
                _textMeshRotationDisappearTween[i].Kill();
                _textMeshColorDisappearTween[i].Kill();
            }
            _textComponent.ClearMesh();
            _textComponent.text ="";
            StopAllCoroutines();
        }

        public void PrintImmediately()
        {
            int letterCount = 0;
            SetOriginalText(originalText);
            while (_parsedTextIndex < _parsedText.Count)
            {
                PrintStyle style = _dialogStyle[_parsedTextIndex];
                for (int i = 0; i < _parsedText[_parsedTextIndex].Length; i++)
                {
                    AppearTween(style, _parsedText[_parsedTextIndex][i], letterCount);
                    RepeatTween(style, letterCount,new CancellationTokenSource().Token);
                    letterCount++;
                }
                _parsedTextIndex++;
            }

            _isAppeared = true;
        }
       
        public void Skip()
        {
            StopPrinting();
            PrintImmediately();
        }

        public async Task Disappear(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                int letterCount = 0;
                _parsedTextIndex = 0;
                _isPrinting = true;

                while (_parsedTextIndex < _parsedText.Count)
                {
                    PrintStyle style = _dialogStyle[_parsedTextIndex];
                    switch (style.disappearUnit)
                    {
                        case PrintStyle.Unit.Letter:
                            for (int i = 0; i < _parsedText[_parsedTextIndex].Length; i++)
                            {
                                DisappearTween(style, _parsedText[_parsedTextIndex][i], letterCount);
                                onDisappearEvent.Invoke();
                                letterCount++;
                                await Task.Delay((int)(style.disappearInterval * 1000), cancellationTokenSource.Token);
                            }
                            break;
                        case PrintStyle.Unit.Word:
                            for (int i = 0; i < _parsedText[_parsedTextIndex].Length; i++)
                            {
                                DisappearTween(style, _parsedText[_parsedTextIndex][i], letterCount);
                                onDisappearEvent.Invoke();
                                letterCount++;
                                if (_parsedText[_parsedTextIndex][i] == ' ')
                                {
                                    await Task.Delay((int)(style.disappearInterval * 1000), cancellationTokenSource.Token);
                                }
                            }
                            break;
                        case PrintStyle.Unit.Sentence:
                            for (int i = 0; i < _parsedText[_parsedTextIndex].Length; i++)
                            {
                                DisappearTween(style, _parsedText[_parsedTextIndex][i], letterCount);
                                onDisappearEvent.Invoke();
                                letterCount++;
                            }
                            await Task.Delay((int)(style.disappearInterval * 1000), cancellationTokenSource.Token);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    _parsedTextIndex++;
                }

                StopPrinting();
            }
            catch (OperationCanceledException)
            {
                UnityEngine.Debug.Log("Async task was canceled.");
            }
          
        }

        public async Task Print(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                onBeginPrintEvent.Invoke();
                int letterCount = 0;
                _parsedTextIndex = 0;
                _isPrinting = true;

                if (stayPosition)
                {
                    while (_parsedTextIndex < _parsedText.Count)
                    {
                        for (int i = 0; i < _parsedText[_parsedTextIndex].Length; i++)
                        {
                            _currentText.Append(_parsedText[_parsedTextIndex][i]);
                        }

                        _parsedTextIndex++;
                    }

                    _textComponent.text=_currentText.ToString();
                }

                _parsedTextIndex = 0;

                while (_parsedTextIndex < _parsedText.Count)
                {
                    PrintStyle style = _dialogStyle[_parsedTextIndex];
                    switch (style.appearAndRepeatUnit)
                    {
                        case PrintStyle.Unit.Letter:
                            for (int i = 0; i < _parsedText[_parsedTextIndex].Length; i++)
                            {
                                AppearTween(style, _parsedText[_parsedTextIndex][i], letterCount);
                                onAppearEvent.Invoke();
                                RepeatTween(style, letterCount, cancellationTokenSource.Token);
                                letterCount++;
                                await Task.Delay((int)(style.appearInterval * 1000), cancellationTokenSource.Token);
                            }

                            break;
                        case PrintStyle.Unit.Word:
                            for (int i = 0; i < _parsedText[_parsedTextIndex].Length; i++)
                            {
                                AppearTween(style, _parsedText[_parsedTextIndex][i], letterCount);
                                onAppearEvent.Invoke();
                                RepeatTween(style, letterCount, cancellationTokenSource.Token);
                                letterCount++;
                                if (_parsedText[_parsedTextIndex][i] == ' ')
                                {
                                    await Task.Delay((int)(style.appearInterval * 1000), cancellationTokenSource.Token);
                                }
                            }

                            await Task.Delay((int)(style.appearInterval * 1000), cancellationTokenSource.Token);
                            break;
                        case PrintStyle.Unit.Sentence:
                            for (int i = 0; i < _parsedText[_parsedTextIndex].Length; i++)
                            {
                                AppearTween(style, _parsedText[_parsedTextIndex][i], letterCount);
                                onAppearEvent.Invoke();
                                RepeatTween(style, letterCount, cancellationTokenSource.Token);
                                letterCount++;
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    _parsedTextIndex++;
                }

                _isPrinting = false;
                _isAppeared = true;
                onEndPrintEvent.Invoke();
                await Task.CompletedTask;
            }
            catch (OperationCanceledException)
            { 
                UnityEngine.Debug.Log("Async task was canceled.");
            }
        }
    }
}