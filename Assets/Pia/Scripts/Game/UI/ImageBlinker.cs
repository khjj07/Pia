using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ImageBlinker : MonoBehaviour
{
    private Image[] _image;
    void Start()
    {
        _image = GetComponentsInChildren<Image>();
        foreach (var img in _image)
        {
            img.DOFade(0.5f,1).SetLoops(-1, LoopType.Yoyo);
        }
    }

    
}
