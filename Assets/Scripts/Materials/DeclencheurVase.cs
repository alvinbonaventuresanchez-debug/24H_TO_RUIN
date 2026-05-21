using UnityEngine;

public class DeclencheurVase : MonoBehaviour
{
    [Header("References")]
    public MurSecret murSecret;
    public float distanceInteraction = 2f;

    private ObjetPortable objetPortable;
    private Transform joueur;

    void Start()
    {
        objetPortable = GetComponent<ObjetPortable>();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            joueur = playerObj.transform;
    }

    void Update()
    {
        if (joueur == null || murSecret == null) return;

        // Le vase doit être porté ET le joueur appuie sur E
        bool vaseEnMain = objetPortable != null && ObjetPortable.EstPorteActuellement(this.gameObject);
        if (!vaseEnMain) return;

        float distanceAuMur = Vector3.Distance(transform.position, murSecret.transform.position);
        if (distanceAuMur > distanceInteraction) return;

        if (Input.GetKeyDown(KeyCode.R))
            murSecret.Ouvrir();
    }
}