using UnityEngine;

public class PNJ_HitboxSuivi : MonoBehaviour
{
    [SerializeField] private Transform cibleRoot;
    [SerializeField] private Vector3 offsetLocal;
    [SerializeField] private bool suivreRotation;

    public void DefinirCible(Transform nouvelleCible)
    {
        cibleRoot = nouvelleCible;
    }

    void LateUpdate()
    {
        if (cibleRoot == null)
        {
            return;
        }

        transform.position = cibleRoot.TransformPoint(offsetLocal);

        if (suivreRotation)
        {
            transform.rotation = Quaternion.Euler(0f, cibleRoot.eulerAngles.y, 0f);
        }
    }
}
