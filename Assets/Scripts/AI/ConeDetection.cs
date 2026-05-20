using UnityEngine;

public class ConeDetection : MonoBehaviour
{
    [Header("Cone de vision")]
    public float distance = 5f;
    public float angle = 90f;
    public Transform joueur;

    private bool voyaitJoueur = false;

    void Update()
    {
        bool voit = JoueurDetecte();

        if (voit && !voyaitJoueur)
            JaugeDetection.Instance.SignalerDetection(true);
        else if (!voit && voyaitJoueur)
            JaugeDetection.Instance.SignalerDetection(false);

        voyaitJoueur = voit;
    }

    bool JoueurDetecte()
    {
        if (joueur == null) return false;

        Vector3 direction = joueur.position - transform.position;
        if (direction.magnitude > distance) return false;

        float angleDiff = Vector3.Angle(transform.forward, direction);
        if (angleDiff > angle / 2f) return false;

        return true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = voyaitJoueur ? Color.red : Color.yellow;
        Vector3 rayonGauche = Quaternion.Euler(0, -angle / 2f, 0) * transform.forward;
        Vector3 rayonDroit = Quaternion.Euler(0, angle / 2f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, rayonGauche * distance);
        Gizmos.DrawRay(transform.position, rayonDroit * distance);
        Gizmos.DrawWireSphere(transform.position, distance);
    }
}