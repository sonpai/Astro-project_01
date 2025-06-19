using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("--------- audio source --------------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("--------- audio clip --------------")]

    public AudioClip background;
    public AudioClip death;
    public AudioClip checkpoint;
    public AudioClip wallTouch;
    public AudioClip portalIn;
    public AudioClip portalOut;


    public void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }
}
