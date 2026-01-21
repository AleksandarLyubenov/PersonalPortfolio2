using UnityEngine;

public class ShellCasing : MonoBehaviour
{
    [SerializeField] private AudioSource shellAudio;
    private bool hasPlayed = false;

    public void SetAudio(AudioSource source)
    {
        shellAudio = source;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasPlayed && shellAudio != null)
        {
            shellAudio.Play();
            hasPlayed = true;

            Destroy(this, 5f);
        }
    }
}
