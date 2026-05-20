using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InteractionCadre : MonoBehaviour
{
    [Header("Interaction")]
    public float distanceInteraction = 2f;
    public Transform joueur;

    [Header("UI")]
    public GameObject indicateurE;

    private Rigidbody rb;
    private bool estTombe = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        TrouverJoueurSiNecessaire();
    }

    void Start()
    {
        if (indicateurE != null)
            indicateurE.SetActive(false);
    }

    void Update()
    {
        if (estTombe) return;
        if (!TrouverJoueurSiNecessaire())
        {
            if (indicateurE != null)
                indicateurE.SetActive(false);
            return;
        }

        float distance = Vector3.Distance(transform.position, joueur.position);
        bool assezProche = distance <= distanceInteraction;

        // Affiche ou cache l'indicateur
        if (indicateurE != null)
            indicateurE.SetActive(assezProche);

        if (!assezProche) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            FaireTomber();
        }
    }

    void FaireTomber()
    {
        if (rb == null) return;

        rb.isKinematic = false;
        rb.AddForce(Vector3.forward * 2f, ForceMode.Impulse);
        estTombe = true;

        if (indicateurE != null)
            indicateurE.SetActive(false);
    }

    bool TrouverJoueurSiNecessaire()
    {
        if (joueur != null)
            return true;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null)
            return false;

        joueur = playerObj.transform;
        return true;
    }
}
