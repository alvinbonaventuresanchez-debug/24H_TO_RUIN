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

    [Header("Character Controller")]
    [SerializeField] private bool autoConfigurerCharacterController = true;
    [SerializeField] private float controllerHeight = 1.8f;
    [SerializeField] private float controllerRadius = 0.35f;
    [SerializeField] private float skinWidth = 0.02f;
    [SerializeField] private float stepOffset = 0f;
    [SerializeField] private bool corrigerPositionAuDemarrage = true;
    [SerializeField] private float margeDepenetration = 0.02f;

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

    void Start()
    {
        if (corrigerPositionAuDemarrage)
        {
            CorrigerPositionInitiale();
        }
    }

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = animator != null ? animator : GetComponentInChildren<Animator>();
        ConfigurerCharacterController();
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
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        ConfigurerCharacterController();
    }

    void OnValidate()
    {
        characterController = GetComponent<CharacterController>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        ConfigurerCharacterController();
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

    void ConfigurerCharacterController()
    {
        if (characterController == null)
        {
            return;
        }

        if (autoConfigurerCharacterController)
        {
            float hauteur = Mathf.Max(controllerHeight, 0.2f);
            float rayonMax = Mathf.Max((hauteur * 0.5f) - 0.01f, 0.05f);
            float rayon = Mathf.Clamp(controllerRadius, 0.05f, rayonMax);

            characterController.height = hauteur;
            characterController.radius = rayon;
            characterController.center = new Vector3(0f, hauteur * 0.5f, 0f);
            characterController.skinWidth = Mathf.Clamp(skinWidth, 0.005f, rayon);
            characterController.minMoveDistance = 0f;
        }

        characterController.stepOffset = Mathf.Clamp(stepOffset, 0f, CalculerStepOffsetMaximum());
    }

    float CalculerStepOffsetMaximum()
    {
        if (characterController == null)
        {
            return 0f;
        }

        Vector3 scale = transform.lossyScale;
        float scaledHeight = Mathf.Abs(characterController.height * scale.y);
        float radiusScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));
        float scaledRadius = Mathf.Abs(characterController.radius) * radiusScale;
        float maxScaledStepOffset = Mathf.Max(0f, scaledHeight + (scaledRadius * 2f) - 0.001f);
        float stepScale = Mathf.Max(Mathf.Abs(scale.y), 0.0001f);

        return maxScaledStepOffset / stepScale;
    }

    void CorrigerPositionInitiale()
    {
        if (characterController == null)
        {
            return;
        }

        Vector3 capsuleTop;
        Vector3 capsuleBottom;
        float capsuleRadius;
        ConstruireCapsuleMonde(out capsuleTop, out capsuleBottom, out capsuleRadius);

        Collider[] overlaps = Physics.OverlapCapsule(
            capsuleTop,
            capsuleBottom,
            capsuleRadius,
            obstacleMask,
            QueryTriggerInteraction.Ignore);

        Vector3 correction = Vector3.zero;

        foreach (Collider overlap in overlaps)
        {
            if (overlap == characterController || overlap.transform.IsChildOf(transform))
            {
                continue;
            }

            if (Physics.ComputePenetration(
                characterController,
                transform.position,
                transform.rotation,
                overlap,
                overlap.transform.position,
                overlap.transform.rotation,
                out Vector3 direction,
                out float distance))
            {
                correction += direction * (distance + margeDepenetration);
            }
        }

        if (correction.sqrMagnitude <= 0.000001f)
        {
            return;
        }

        bool etatInitial = characterController.enabled;
        characterController.enabled = false;
        transform.position += correction;
        characterController.enabled = etatInitial;
    }

    void ConstruireCapsuleMonde(out Vector3 capsuleTop, out Vector3 capsuleBottom, out float capsuleRadius)
    {
        Vector3 scale = transform.lossyScale;
        float scaledHeight = Mathf.Abs(characterController.height * scale.y);
        float radiusScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));
        capsuleRadius = Mathf.Abs(characterController.radius) * radiusScale;

        float demiHauteur = Mathf.Max((scaledHeight * 0.5f) - capsuleRadius, 0f);
        Vector3 centreMonde = transform.TransformPoint(characterController.center);
        Vector3 axe = transform.up * demiHauteur;

        capsuleTop = centreMonde + axe;
        capsuleBottom = centreMonde - axe;
    }
}
