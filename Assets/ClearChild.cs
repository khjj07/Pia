using UnityEngine;


[ExecuteAlways]
public class ClearChild : MonoBehaviour
{
    private void Update()
    {
        if (!Application.isPlaying)
        {
            var children = GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child != transform)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }
        else
        {
            Destroy(this);
        }
    }
}
