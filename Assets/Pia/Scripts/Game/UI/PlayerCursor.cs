using Default.Scripts.Util;
using UniRx;
using UnityEngine;

public class PlayerCursor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GlobalInputBinder.CreateGetMousePositionStream().Subscribe(v =>
        {
            GetComponent<RectTransform>().anchoredPosition = v;
        }).AddTo(gameObject);
    }

}
