using System.Linq;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    public void On()
    {
        gameObject.SetActive(true);
    }
    public void Off()
    {
        gameObject.SetActive(false);
    }
}
