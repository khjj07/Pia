using System.Threading;
using UnityEngine;

namespace Default.Scripts.Printer
{
    public class PrintTester : MonoBehaviour
    {
        Printer printer;
        void Start()
        {
            printer = GetComponent<Printer>();
            printer.SetOriginalText("hello world");
            printer.Print(new CancellationTokenSource());
        }

    }
}
