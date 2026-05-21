using UnityEngine;

public class MurSecret : MonoBehaviour
{
    [Header("Slide")]
    public float vitesseSlide = 3f;

    private bool estOuvert = false;
    private bool enMouvement = false;
    private Vector3 positionFermee;
    private Vector3 positionOuverte;

    void Start()
    {
        positionFermee = transform.position;

        // Décale vers la droite de la largeur exacte du mur
        float largeur = GetComponent<Renderer>().bounds.size.x;
        positionOuverte = positionFermee + transform.right * largeur;
    }

    void Update()
    {
        if (!enMouvement) return;

        Vector3 cible = estOuvert ? positionOuverte : positionFermee;
        transform.position = Vector3.MoveTowards(transform.position, cible, vitesseSlide * Time.deltaTime);

        if (transform.position == cible)
            enMouvement = false;
    }

    public void Ouvrir()
    {
        if (estOuvert || enMouvement) return;
        estOuvert = true;
        enMouvement = true;
    }
}