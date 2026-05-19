using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float tempsRestant = 720f;
    public TextMeshProUGUI timerText;

    void Update()
    {
        if (tempsRestant > 0)
        {
            tempsRestant -= Time.deltaTime;

            if (tempsRestant < 0)
            {
                tempsRestant = 0;
            }

            int minutes = Mathf.FloorToInt(tempsRestant / 60);
            int secondes = Mathf.FloorToInt(tempsRestant % 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, secondes);
        }
    }
}