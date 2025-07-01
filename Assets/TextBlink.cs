using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextBlink : MonoBehaviour
{
    public Text textToBlink; // G�n Text t? Canvas v�o ?�y
    public float blinkSpeed = 1f; // T?c ?? nh?p nh�y (gi�y)

    private bool isBlinking = false;
    private void Update()
    {
        if (textToBlink != null)
        {
            StartBlinking();
        }
    }
    private void Start()
    {
       
    }


    public void StartBlinking()
    {
        if (!isBlinking)
        {
            isBlinking = true;
            StartCoroutine(BlinkText());
        }
    }

    public void StopBlinking()
    {
        if (isBlinking)
        {
            isBlinking = false;
            StopCoroutine(BlinkText());
            // ??m b?o text hi?n th? l?i b�nh th??ng
            var color = textToBlink.color;
            color.a = 1f;
            textToBlink.color = color;
        }
    }

    private IEnumerator
        BlinkText()
    {
        while (isBlinking)
        {
            // Gi?m alpha xu?ng 0
            var color = textToBlink.color;
            color.a = 0f;
            textToBlink.color = color;

            yield return new WaitForSeconds(blinkSpeed / 2);

            // T?ng alpha l�n 1
            color.a = 1f;
            textToBlink.color = color;

            yield return new WaitForSeconds(blinkSpeed / 2);
        }
    }
}
