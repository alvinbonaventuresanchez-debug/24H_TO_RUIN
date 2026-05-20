using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PNJ_IA : MonoBehaviour
{
    [Header("Deplacement")]
    [SerializeField] private float vitesse = 2f;
    [SerializeField] private float vitesseRotation = 8f;

    // Chaque etape : direction + distance a parcourir
    // Le PNJ avance de X unites dans la direction indiquee, puis passe a l'etape suivante
    [System.Serializable]
    public struct Etape
    {
        public Vector3 direction;
        public float distance;
    }

    [SerializeField] private Etape[] etapes = new Etape[]
    {
        new Etape { direction = Vector3.forward,  distance = 3f },
        new Etape { direction = -Vector3.right,   distance = 3f },
        new Etape { direction = -Vector3.forward, distance = 3f },
        new Etape { direction = Vector3.right,    distance = 3f },
    };

    [Header("Animation")]
    [SerializeField] private AnimationClip walkClip;

    private Rigidbody rb;
    private PlayableGraph walkGraph;

    private int indexEtape = 0;
    private float distanceParcourue = 0f;
    private Quaternion rotationCible;

    // -------------------------------------------------------------------------
    // Cycle de vie
    // -------------------------------------------------------------------------

    private void Awake()
    {
        ConfigurerPhysique();
        TryCreateWalkGraph();

        rotationCible = Quaternion.LookRotation(etapes[indexEtape].direction, Vector3.up);
        rb.MoveRotation(rotationCible);
    }

    private void FixedUpdate()
    {
        Etape etapeActuelle = etapes[indexEtape];

        // Distance parcourue ce FixedUpdate
        float deplacement = vitesse * Time.fixedDeltaTime;
        distanceParcourue += deplacement;

        // Si on a atteint ou depasse la distance cible, on passe a l'etape suivante
        if (distanceParcourue >= etapeActuelle.distance)
        {
            // On se place exactement a la fin de l'etape pour eviter la derive
            float surplus = distanceParcourue - etapeActuelle.distance;
            rb.MovePosition(rb.position + etapeActuelle.direction.normalized * (deplacement - surplus));

            indexEtape = (indexEtape + 1) % etapes.Length;
            distanceParcourue = surplus; // on repart avec le surplus dans la nouvelle direction

            rotationCible = Quaternion.LookRotation(etapes[indexEtape].direction, Vector3.up);
        }

        // Rotation progressive
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, rotationCible, vitesseRotation * Time.fixedDeltaTime));

        // Deplacement dans la direction de l'etape courante (pas transform.forward pour eviter la derive pendant la rotation)
        rb.MovePosition(rb.position + etapes[indexEtape].direction.normalized * vitesse * Time.fixedDeltaTime);
    }

    private void OnDestroy()
    {
        if (walkGraph.IsValid())
        {
            walkGraph.Destroy();
        }
    }

    // -------------------------------------------------------------------------
    // Physique
    // -------------------------------------------------------------------------

    private void ConfigurerPhysique()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.constraints = RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationZ;
    }

    // -------------------------------------------------------------------------
    // Animation
    // -------------------------------------------------------------------------

    private void TryCreateWalkGraph()
    {
        Animator animator = GetComponent<Animator>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator == null || walkClip == null)
        {
            return;
        }

        walkGraph = PlayableGraph.Create($"{name}_WalkGraph");
        AnimationPlayableOutput output = AnimationPlayableOutput.Create(walkGraph, "Walk", animator);
        AnimationClipPlayable walkPlayable = AnimationClipPlayable.Create(walkGraph, walkClip);
        output.SetSourcePlayable(walkPlayable);
        walkGraph.Play();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (walkClip != null)
        {
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { "Assets/models" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);

            foreach (Object asset in assets)
            {
                AnimationClip clip = asset as AnimationClip;

                if (clip == null || clip.name.StartsWith("__preview__"))
                {
                    continue;
                }

                walkClip = clip;
                EditorUtility.SetDirty(this);
                return;
            }
        }
    }
#endif
}