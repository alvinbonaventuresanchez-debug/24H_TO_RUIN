using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PNJ_IA : MonoBehaviour
{
    [SerializeField] private float vitesse = 2f;
    [SerializeField] private float dureeAvance = 3f;
    [SerializeField] private float gravite = -20f;
    [SerializeField] private float forceAuSol = -2f;
    [SerializeField] private PNJ_HitboxSuivi pnjHitbox;

    private CharacterController characterController;
    private float tempsRestant;
    private float vitesseVerticale;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        tempsRestant = dureeAvance;
        InitialiserHitbox();
    }

    void OnEnable()
    {
        tempsRestant = dureeAvance;
        vitesseVerticale = 0f;
        InitialiserHitbox();
    }

    void OnValidate()
    {
        InitialiserHitbox();
    }

    void Update()
    {
        bool estAuSol = characterController.isGrounded;

        if (estAuSol && vitesseVerticale < 0f)
        {
            vitesseVerticale = forceAuSol;
        }
        else
        {
            vitesseVerticale += gravite * Time.deltaTime;
        }

        Vector3 mouvement = Vector3.zero;

        if (tempsRestant > 0f)
        {
            mouvement += transform.forward * vitesse;
            tempsRestant -= Time.deltaTime;
        }

        mouvement.y = vitesseVerticale;
        characterController.Move(mouvement * Time.deltaTime);
    }

    void InitialiserHitbox()
    {
        if (pnjHitbox != null)
        {
            pnjHitbox.DefinirCible(transform);
        }
    }
}
