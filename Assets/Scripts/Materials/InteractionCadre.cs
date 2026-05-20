using UnityEngine;

public class InteractionCadre : MonoBehaviour
{
    [Header("Interaction")]
    public float distanceInteraction = 2f;
    public Transform joueur;

    private Rigidbody rb;
    private bool estTombe = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (estTombe) return;

        // Verifie que le joueur est assez proche
        float distance = Vector3.Distance(transform.position, joueur.position);
        if (distance > distanceInteraction) return;

        // Verifie que le joueur appuie sur E
        if (Input.GetKeyDown(KeyCode.E))
        {
            FaireTomber();
        }
    }

    void FaireTomber()
    {
        // Desactive Is Kinematic pour que la gravite prenne effet
        rb.isKinematic = false;

        // Donne une petite impulsion pour simuler le decrochage
        rb.AddForce(Vector3.forward * 2f, ForceMode.Impulse);

        estTombe = true;
    }
}