using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjetPortable : MonoBehaviour
{
    [Header("Position dans les mains")]
    public Vector3 offsetPosition = new Vector3(0f, -0.16f, 0.31f);
    public float distanceInteraction = 2f;

    [Header("Indicateur interaction")]
    [SerializeField] private GameObject boutonE; // Glisser le BoutonE ici dans l'Inspector

    private static readonly System.Collections.Generic.List<ObjetPortable> objetsPortables = new System.Collections.Generic.List<ObjetPortable>();
    private static ObjetPortable objetPorteActuel;
    private static int frameDerniereInteraction = -1;

    private Transform joueur;
    private Camera cam;
    private Rigidbody rb;
    private Collider[] collidersObjet;
    private Collider[] collidersJoueur;
    private Collider[] collidersObstacle;
    private Bounds boundsLocaux;
    private Vector3 pointAccrocheLocal;
    private bool estPorte = false;

    void OnEnable()
    {
        if (!objetsPortables.Contains(this))
            objetsPortables.Add(this);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collidersObjet = GetComponentsInChildren<Collider>(true);
        CalculerBoundsLocaux();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError("Aucun objet avec le tag 'Player' n'a ete trouve.", this);
            return;
        }

        joueur = playerObj.transform;
        collidersJoueur = playerObj.GetComponentsInChildren<Collider>(true);
        IgnorerCollisionAvecJoueur(true);
        collidersObstacle = TrouverCollidersObstacle();

        cam = Camera.main;

        // S'assurer que le bouton est caché au départ
        if (boutonE != null)
            boutonE.SetActive(false);
    }

    void Update()
    {
        if (joueur == null || cam == null)
            return;

        if (Input.GetKeyDown(KeyCode.E))
            GererInteraction();

        if (estPorte)
            SuivreLaCamera();

        MettreAJourBoutonE();
    }

    void MettreAJourBoutonE()
    {
        if (boutonE == null || estPorte) return;

        // Affiche le bouton uniquement si cet objet est le plus proche ET dans le range
        ObjetPortable objetLePlusProche = TrouverObjetLePlusProche(joueur.position);
        bool doitAfficher = (objetLePlusProche == this);

        if (boutonE.activeSelf != doitAfficher)
            boutonE.SetActive(doitAfficher);
    }

    void Ramasser()
    {
        if (objetPorteActuel != null && objetPorteActuel != this)
            return;

        if (boutonE != null)
            boutonE.SetActive(false);

        IgnorerCollisionAvecObstacle(true);
        rb.isKinematic = true;
        rb.useGravity = false;
        estPorte = true;
        objetPorteActuel = this;
    }

    void Poser()
    {
        IgnorerCollisionAvecObstacle(false);
        rb.isKinematic = false;
        rb.useGravity = true;
        estPorte = false;

        if (objetPorteActuel == this)
            objetPorteActuel = null;
    }

    void SuivreLaCamera()
    {
        Quaternion rotationCible = cam.transform.rotation;
        Vector3 pointVise = cam.transform.position
                          + cam.transform.right * offsetPosition.x
                          + cam.transform.up * offsetPosition.y
                          + cam.transform.forward * offsetPosition.z;

        Vector3 pointAccrocheMonde = rotationCible * Vector3.Scale(pointAccrocheLocal, transform.lossyScale);
        Vector3 cible = pointVise - pointAccrocheMonde;

        transform.position = Vector3.Lerp(transform.position, cible, Time.deltaTime * 15f);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationCible, Time.deltaTime * 15f);
    }

    void IgnorerCollisionAvecJoueur(bool ignorer)
    {
        if (collidersObjet == null || collidersJoueur == null) return;

        foreach (Collider colliderObjet in collidersObjet)
        {
            if (colliderObjet == null) continue;

            foreach (Collider colliderJoueur in collidersJoueur)
            {
                if (colliderJoueur == null) continue;
                Physics.IgnoreCollision(colliderObjet, colliderJoueur, ignorer);
            }
        }
    }

    Collider[] TrouverCollidersObstacle()
    {
        Collider[] tousLesColliders = FindObjectsByType<Collider>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        System.Collections.Generic.List<Collider> obstacles = new System.Collections.Generic.List<Collider>();
        int obstacleLayer = LayerMask.NameToLayer("Obstacle");

        foreach (Collider colliderTrouve in tousLesColliders)
        {
            if (colliderTrouve == null) continue;
            if (colliderTrouve.transform.IsChildOf(transform)) continue;
            string nom = colliderTrouve.gameObject.name.ToLowerInvariant();
            bool estObstacle = colliderTrouve.gameObject.layer == obstacleLayer
                            || nom.Contains("mur")
                            || nom.Contains("wall");
            if (!estObstacle) continue;

            obstacles.Add(colliderTrouve);
        }

        return obstacles.ToArray();
    }

    void IgnorerCollisionAvecObstacle(bool ignorer)
    {
        if (collidersObjet == null || collidersObstacle == null) return;

        foreach (Collider colliderObjet in collidersObjet)
        {
            if (colliderObjet == null) continue;

            foreach (Collider colliderObstacle in collidersObstacle)
            {
                if (colliderObstacle == null) continue;
                Physics.IgnoreCollision(colliderObjet, colliderObstacle, ignorer);
            }
        }
    }

    void CalculerBoundsLocaux()
    {
        if (collidersObjet == null || collidersObjet.Length == 0)
        {
            boundsLocaux = new Bounds(Vector3.zero, Vector3.one * 0.1f);
            pointAccrocheLocal = Vector3.zero;
            return;
        }

        bool premierPoint = true;
        Bounds resultat = new Bounds();

        foreach (Collider colliderObjet in collidersObjet)
        {
            if (colliderObjet == null) continue;

            Bounds boundsMonde = colliderObjet.bounds;
            Vector3 centre = boundsMonde.center;
            Vector3 extents = boundsMonde.extents;

            Vector3[] coins =
            {
                centre + new Vector3(-extents.x, -extents.y, -extents.z),
                centre + new Vector3(-extents.x, -extents.y,  extents.z),
                centre + new Vector3(-extents.x,  extents.y, -extents.z),
                centre + new Vector3(-extents.x,  extents.y,  extents.z),
                centre + new Vector3( extents.x, -extents.y, -extents.z),
                centre + new Vector3( extents.x, -extents.y,  extents.z),
                centre + new Vector3( extents.x,  extents.y, -extents.z),
                centre + new Vector3( extents.x,  extents.y,  extents.z)
            };

            foreach (Vector3 coinMonde in coins)
            {
                Vector3 coinLocal = transform.InverseTransformPoint(coinMonde);
                if (premierPoint)
                {
                    resultat = new Bounds(coinLocal, Vector3.zero);
                    premierPoint = false;
                }
                else
                {
                    resultat.Encapsulate(coinLocal);
                }
            }
        }

        if (premierPoint)
        {
            boundsLocaux = new Bounds(Vector3.zero, Vector3.one * 0.1f);
            pointAccrocheLocal = Vector3.zero;
            return;
        }

        boundsLocaux = resultat;
        pointAccrocheLocal = new Vector3(boundsLocaux.center.x, boundsLocaux.center.y, boundsLocaux.min.z);
    }

    void GererInteraction()
    {
        if (frameDerniereInteraction == Time.frameCount)
            return;

        frameDerniereInteraction = Time.frameCount;

        if (objetPorteActuel != null)
        {
            objetPorteActuel.Poser();
            return;
        }

        ObjetPortable objetLePlusProche = TrouverObjetLePlusProche(joueur.position);
        if (objetLePlusProche != null)
            objetLePlusProche.Ramasser();
    }

    static ObjetPortable TrouverObjetLePlusProche(Vector3 positionJoueur)
    {
        ObjetPortable objetLePlusProche = null;
        float meilleureDistance = float.MaxValue;

        foreach (ObjetPortable objet in objetsPortables)
        {
            if (objet == null || !objet.isActiveAndEnabled || objet.estPorte)
                continue;

            float distance = Vector3.Distance(objet.PointInteractionMonde(), positionJoueur);
            if (distance > objet.distanceInteraction || distance >= meilleureDistance)
                continue;

            meilleureDistance = distance;
            objetLePlusProche = objet;
        }

        return objetLePlusProche;
    }

    Vector3 PointInteractionMonde()
    {
        return transform.TransformPoint(boundsLocaux.center);
    }

    void OnDisable()
    {
        objetsPortables.Remove(this);

        if (boutonE != null)
            boutonE.SetActive(false);

        if (objetPorteActuel == this)
            objetPorteActuel = null;
    }
}