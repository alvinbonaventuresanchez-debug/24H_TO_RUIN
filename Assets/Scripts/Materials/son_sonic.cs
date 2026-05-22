using UnityEngine;

public class ProximitySound : MonoBehaviour
{
    public float distance = 1f;
    public AudioSource musiqueprincipale;
    private AudioSource audioSource;
    private Transform player;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= distance)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
            if (musiqueprincipale != null && musiqueprincipale.isPlaying)
                musiqueprincipale.Stop();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
            if (musiqueprincipale != null && !musiqueprincipale.isPlaying)
                musiqueprincipale.Play();
        }
    }
}