using UnityEngine;
using UnityEngine.UI;

public class JaugeDetection : MonoBehaviour
{
    public static JaugeDetection Instance;

    [Header("Jauge")]
    public float valeurMax = 100f;
    public float vitesseMontee = 10f;
    public float vitesseDescente = 5f;

    [Header("UI")]
    public Slider sliderJauge;

    private float valeurCourante = 0f;
    private int nbPNJVoyant = 0;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (nbPNJVoyant > 0)
            valeurCourante += vitesseMontee * Time.deltaTime;
        else
            valeurCourante -= vitesseDescente * Time.deltaTime;

        valeurCourante = Mathf.Clamp(valeurCourante, 0f, valeurMax);

        if (sliderJauge != null)
            sliderJauge.value = valeurCourante / valeurMax;

        if (valeurCourante >= valeurMax)
            OnDefaite();
    }

    public void SignalerDetection(bool voit)
    {
        if (voit) nbPNJVoyant++;
        else nbPNJVoyant = Mathf.Max(0, nbPNJVoyant - 1);
    }

    void OnDefaite()
    {
        Debug.Log("DEFAITE");
        // Ici tu appelleras ton systeme de game over
    }
}