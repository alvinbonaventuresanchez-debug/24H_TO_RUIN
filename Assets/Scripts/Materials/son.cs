using UnityEngine;

public class SalleSecretAudio : MonoBehaviour
{
    public AudioSource musiqueprincipale;
    public AudioSource musiqueSecrete;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (musiqueprincipale != null && musiqueprincipale.isPlaying)
                musiqueprincipale.Stop();

            if (musiqueSecrete != null && !musiqueSecrete.isPlaying)
                musiqueSecrete.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (musiqueSecrete != null && musiqueSecrete.isPlaying)
                musiqueSecrete.Stop();

            if (musiqueprincipale != null && !musiqueprincipale.isPlaying)
                musiqueprincipale.Play();
        }
    }
}