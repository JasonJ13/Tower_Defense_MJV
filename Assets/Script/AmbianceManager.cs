using System.Collections;
using UnityEngine;

public class AmbianceManager : MonoBehaviour
{
    [SerializeField] private AudioSource wind1;
    [SerializeField] private AudioSource wind2;

    private IEnumerator PlayWind()
    {
        int secs = Random.Range(3, 10);
        yield return new WaitForSeconds(secs);
        int r = Random.Range(0, 1);
        if (r == 0)
        {
            wind1.Play();
        }else
        {
            wind2.Play();
        }

    }

    private void Update()
    {
        if (!wind1.isPlaying || !wind2.isPlaying)
        {
            StartCoroutine(PlayWind());
        }
    }
}
