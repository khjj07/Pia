using Default.Scripts.Util;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class PlayerCursor : MonoBehaviour
{
    private Canvas canvas;
    private RectTransform rectTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        GlobalInputBinder.CreateGetMousePositionStream().Subscribe(v =>
        {
            rectTransform.anchoredPosition = v;
        }).AddTo(gameObject);
    }

}
