using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Pia.Scripts.Path
{
    public class GuideViewer : MonoBehaviour
    {
        private Canvas _canvas;
        public float appearDuration;
        public float disappearDuration;
        private bool _isAppeared;
        private bool _isAppearing;

        private Image current;

        public void Start()
        {
            _canvas = GetComponent<Canvas>();
        }
        public async Task Print(Image image)
        {
            _isAppeared = true;
            _isAppearing = true;
            current = Instantiate(image, _canvas.transform);
            current.color = new Color();
            current.DOColor(Color.white, appearDuration).OnComplete(() =>
            {
                _isAppearing = false;
            });
            await Task.Delay(TimeSpan.FromSeconds(appearDuration));
        }

        public async Task Disappear()
        {
            current.DOColor(new Color(), disappearDuration).OnComplete(() =>
            {
                _isAppeared = false;
                Destroy(current.gameObject);
            });
            await Task.Delay(TimeSpan.FromSeconds(disappearDuration));
        }

        public bool IsAppeared()
        {
            return _isAppeared;
        }
        public bool IsAppearing()
        {
            return _isAppearing;
        }


    }
}