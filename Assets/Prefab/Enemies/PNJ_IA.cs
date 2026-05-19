using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class PNJ_IA : MonoBehaviour
{
    [Header("Deplacement")]
    [SerializeField] private float vitesseMarche = 2f;
    [SerializeField] private float vitesseRotation = 180f;
    [SerializeField] private bool directionAleatoireAuDepart = true;

    [Header("Detection")]
    [SerializeField] private float distanceDetection = 1.2f;
    [SerializeField] private float rayonDetection = 0.35f;
    [SerializeField] private float hauteurDetection = 0.6f;
    [SerializeField] private LayerMask obstacleMask = ~0;
    [SerializeField] private float delaiChangementDirection = 0.25f;

    [Header("Virage")]
    [SerializeField] private float angleMinRotation = 90f;
    [SerializeField] private float angleMaxRotation = 180f;

    [Header("Optionnel")]
    [SerializeField] private float gravite = -9.81f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip animationMarche;
    [SerializeField] private bool jouerAnimationAuDemarrage = true;

    private CharacterController characterController;
    private Quaternion rotationCible;
    private float vitesseVerticale;
    private float prochainChangementDirection;
    private PlayableGraph animationGraph;
    private bool animationInitialisee;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = animator != null ? animator : GetComponentInChildren<Animator>();
        rotationCible = transform.rotation;

        if (directionAleatoireAuDepart)
        {
            ChoisirNouvelleDirection(Random.Range(0f, 360f));
            transform.rotation = rotationCible;
        }

        if (jouerAnimationAuDemarrage)
        {
            InitialiserAnimation();
        }
    }

    void OnEnable()
    {
        if (jouerAnimationAuDemarrage)
        {
            InitialiserAnimation();
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        TournerVersLaDirection();

        if (Time.time >= prochainChangementDirection && ObstacleDevant())
        {
            TournerAleatoirement();
        }

        Avancer();
    }

    void OnDisable()
    {
        DetruireAnimation();
    }

    void OnDestroy()
    {
        DetruireAnimation();
    }

    void Reset()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void TournerVersLaDirection()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            rotationCible,
            vitesseRotation * Time.deltaTime);
    }

    void Avancer()
    {
        Vector3 mouvementHorizontal = transform.forward * vitesseMarche;

        if (characterController != null)
        {
            if (characterController.isGrounded && vitesseVerticale < 0f)
            {
                vitesseVerticale = -2f;
            }
            else
            {
                vitesseVerticale += gravite * Time.deltaTime;
            }

            Vector3 mouvement = mouvementHorizontal;
            mouvement.y = vitesseVerticale;
            characterController.Move(mouvement * Time.deltaTime);
            return;
        }

        transform.position += mouvementHorizontal * Time.deltaTime;
    }

    bool ObstacleDevant()
    {
        Vector3 origine = transform.position + Vector3.up * hauteurDetection;
        RaycastHit[] hits = Physics.SphereCastAll(
            origine,
            rayonDetection,
            transform.forward,
            distanceDetection,
            obstacleMask,
            QueryTriggerInteraction.Ignore);

        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.transform.IsChildOf(transform))
            {
                return true;
            }
        }

        return false;
    }

    void TournerAleatoirement()
    {
        float angle = Random.Range(angleMinRotation, angleMaxRotation);
        float sens = Random.value < 0.5f ? -1f : 1f;
        ChoisirNouvelleDirection(sens * angle);
    }

    void ChoisirNouvelleDirection(float angle)
    {
        rotationCible = Quaternion.Euler(0f, transform.eulerAngles.y + angle, 0f);
        prochainChangementDirection = Time.time + delaiChangementDirection;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (((1 << hit.gameObject.layer) & obstacleMask) != 0)
        {
            TournerAleatoirement();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & obstacleMask) != 0)
        {
            TournerAleatoirement();
        }
    }

    void InitialiserAnimation()
    {
        if (animationInitialisee || animator == null || animationMarche == null)
        {
            return;
        }

        animator.applyRootMotion = false;

        animationGraph = PlayableGraph.Create($"{name}_PNJ_IA_Animation");
        AnimationPlayableOutput output = AnimationPlayableOutput.Create(animationGraph, "Animation", animator);

        animationMarche.wrapMode = WrapMode.Loop;
        AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(animationGraph, animationMarche);
        clipPlayable.SetApplyFootIK(true);
        clipPlayable.SetDuration(double.MaxValue);
        output.SetSourcePlayable(clipPlayable);

        animationGraph.Play();
        animationInitialisee = true;
    }

    void DetruireAnimation()
    {
        if (!animationInitialisee)
        {
            return;
        }

        if (animationGraph.IsValid())
        {
            animationGraph.Destroy();
        }

        animationInitialisee = false;
        
        }
}
