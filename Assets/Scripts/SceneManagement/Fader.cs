using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    [SerializeField] Image fader;
    public static Fader i { get; private set; }
    private void Awake()
    {
        i = this;
    }
    public IEnumerator FadeIN(float time)
    {
        yield return fader.DOFade(1f, time).WaitForCompletion();
    }
    public IEnumerator FadeOUT(float time)
    {
        yield return fader.DOFade(0f, time).WaitForCompletion();
    }
}
