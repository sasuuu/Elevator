using UnityEngine.Audio;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    AudioSource audioSource;
    public SoundObject[] sounds;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play(string name, bool loop)
    {
        foreach(SoundObject obj in sounds)
        {
            if (obj.name == name)
            {
                audioSource.clip = obj.clip;
                audioSource.loop = loop;
                audioSource.Play();
            }
        }
    }
}
