using UnityEngine;
using System.Collections;

public class AudioMaster : MonoBehaviour {

    public AudioClip[] Track;
    public int currentTrackIndex = 0;

    AudioSource ThisAudioSource;

    void Start()
    {
        ThisAudioSource = GetComponent<AudioSource>();
    }

    public IEnumerator FadeNextTrack(float delay)
    {
        yield return new WaitForSeconds(delay);
        float speed = 0.3F;
        float audioVolume = ThisAudioSource.volume;
        while(ThisAudioSource.volume > 0)
        {
            ThisAudioSource.volume -= speed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        currentTrackIndex++;
        ThisAudioSource.volume = 0F;
        ThisAudioSource.clip = Track[currentTrackIndex];
        ThisAudioSource.Play();
        while (ThisAudioSource.volume < audioVolume)
        {
            ThisAudioSource.volume += speed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        ThisAudioSource.volume = audioVolume;
    }
}
