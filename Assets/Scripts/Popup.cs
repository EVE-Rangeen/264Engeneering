/// <summary>
/// 谈恩萁创建
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    public AnimationCurve showCurve;
    public float animationSpeed = 1f;
    public GameObject panel;
    private Vector3 originalScale;
    
    void OnEnable()
    {
        if (panel != null)
        {
            originalScale = panel.transform.localScale;
            StartCoroutine(showPanel(panel));
        }
    }

    IEnumerator showPanel(GameObject gameObject)
    {
        gameObject.transform.localScale = Vector3.zero;
        
        float timer = 0;
        while(timer<=1)
        {
            gameObject.transform.localScale = originalScale * showCurve.Evaluate(timer);
            timer += Time.unscaledDeltaTime * animationSpeed;
            yield return null;
        }
        
        gameObject.transform.localScale = originalScale;
    }
}
