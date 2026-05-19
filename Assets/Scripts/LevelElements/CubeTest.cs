using UnityEngine;

public class CubeTest : MonoBehaviour
{
    public int compteur = 1;
    public bool compteurActif = true;
    public float timer = 0;
    public string nomCube = "Cube 1";
    private bool isSphereDestroyed = false;
    public GameObject sphere;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AfficherNomCube();
        //sphere.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < 3)
        {
            sphere.GetComponent<SphereTest>().MoveSphere();
            Debug.Log(timer);
            IncrementTimer();
        } else if (!isSphereDestroyed){
            Destroy(sphere);
            isSphereDestroyed = true;
        }
    }

    private void AfficherNomCube(){
        Debug.Log(nomCube);
    }

    private void IncrementTimer(){
        timer += Time.deltaTime;
    }
}
