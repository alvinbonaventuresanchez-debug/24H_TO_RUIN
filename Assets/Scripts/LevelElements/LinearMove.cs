using UnityEngine;

public class LinearMove : MonoBehaviour
{
    public Transform[] points;
    public float speed = 2f;

    private int currentIndex = 0;
    private Vector3 targetPos;

    void Start()
    {
        if(points.Length > 0)
        {
            targetPos = points[0].position;
        }
    }

    void Update()
    {
        //S'il y a moins de deux points, l'objet ne bouge pas
        if(points.Length < 2)
        {
            return;
        }

        //On déplace l'objet vers le point ciblé
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        //Si on est arrivé
        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos;

            //On passe au point suivant
            currentIndex = (currentIndex+1)% points.Length;
            targetPos = points[currentIndex].position;
        }
    }
}
