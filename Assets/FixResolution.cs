using UnityEngine;
using UnityEngine.UI;

public class FixResolution : MonoBehaviour
{
    private CanvasScaler canvasScaler;

    public void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();
    }
    private void Update()
    {
        canvasScaler.referenceResolution = new Vector2(Screen.width,Screen.height);
        canvasScaler.matchWidthOrHeight = 0;
    }

   
}