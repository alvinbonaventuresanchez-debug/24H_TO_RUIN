using UnityEngine;

public class AttractionInput : MonoBehaviour
{
    public PatrouilleBoucle[] pnjs;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach (PatrouilleBoucle pnj in pnjs)
            {
                pnj.AttirerVersJoueur(true);
                ConeDetection cone = pnj.GetComponentInChildren<ConeDetection>();
                if (cone != null)
                {
                    cone.SetModeAlerte(true);
                    cone.distance = 4f;
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.H))
        {
            foreach (PatrouilleBoucle pnj in pnjs)
            {
                pnj.AttirerVersJoueur(false);
                ConeDetection cone = pnj.GetComponentInChildren<ConeDetection>();
                if (cone != null)
                {
                    cone.SetModeAlerte(false);
                    cone.distance = 2f;
                }
            }
        }
    }
}
