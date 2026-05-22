using UnityEngine;

public class AllumerTV : MonoBehaviour
{
    public GameObject ecranAllume;
    public AudioSource audioTV;
    public AudioSource musiqueprincipale;
    private bool allumee = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            float dist = Vector3.Distance(transform.position, 
                Camera.main.transform.position);
            
            if (dist <= 2f)
            {
                allumee = !allumee;

                if (allumee)
                {
                    if (ecranAllume != null) ecranAllume.SetActive(true);
                    if (audioTV != null) audioTV.Play();
                    if (musiqueprincipale != null) musiqueprincipale.Stop();
                }
                else
                {
                    if (ecranAllume != null) ecranAllume.SetActive(false);
                    if (audioTV != null) audioTV.Stop();
                    if (musiqueprincipale != null) musiqueprincipale.Play();
                }
            }
        }
    }
}