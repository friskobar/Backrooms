using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distractions : MonoBehaviour
{
    [SerializeField]AudioClip[] distractions;
    [SerializeField] AudioSource source;
    [SerializeField] float T = 180;
    int i = 0;

    void Start()
    {
        StartCoroutine(Loop());
    }
    
    IEnumerator Loop()
    {
        yield return new WaitForSecondsRealtime(T);
        i = Random.Range(0, distractions.Length);
        source.clip = distractions[i];
        source.Play();
        StartCoroutine(Loop());
    }
}
