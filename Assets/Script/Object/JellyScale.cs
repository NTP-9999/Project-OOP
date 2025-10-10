using UnityEngine;
using System.Collections;

public class JellyScale : MonoBehaviour
{
    public Vector3 originalScale;
    public Vector3 targetScale;
    public float duration = 0.7f;
    public float startDelay = 0.1f;

    private bool canAnimate = false;

    void Start()
    {
        originalScale = transform.localScale;
        StartCoroutine(WaitAndStart());
    }

    IEnumerator WaitAndStart()
    {
        yield return new WaitForSeconds(startDelay);
        canAnimate = true;
    }

    void LateUpdate()
    {
        if (!canAnimate) return; // รอก่อน

        float t = Mathf.PingPong(Time.time / duration, 1f);
        t = Mathf.SmoothStep(0f, 1f, t);
        transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
    }
}
