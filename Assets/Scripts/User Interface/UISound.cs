using UnityEngine;

public class UISound : MonoBehaviour
{
    public AudioClip clickSound;
    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        source.PlayOneShot(clickSound);
    }
}