using UnityEngine;
using TMPro;
using UnityEngine.TextCore.LowLevel;

public class GameTimer : MonoBehaviour
{
    public float tempsRestant = 720f;
    public TextMeshProUGUI timerText;
    [SerializeField] private Font timerFontSource;

    private TMP_FontAsset timerFontAsset;
    private Font appliedFontSource;

    void Awake()
    {
        ApplyTimerFont();
    }

    void OnValidate()
    {
        ApplyTimerFont();
    }

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

            if (timerText != null)
            {
                timerText.text = string.Format("{0:00}:{1:00}", minutes, secondes);
            }
        }
    }

    void ApplyTimerFont()
    {
        if (timerText == null || timerFontSource == null)
        {
            return;
        }

        if (timerFontAsset == null || appliedFontSource != timerFontSource)
        {
            timerFontAsset = TMP_FontAsset.CreateFontAsset(
                timerFontSource,
                90,
                9,
                GlyphRenderMode.SDFAA,
                1024,
                1024,
                AtlasPopulationMode.Dynamic);

            appliedFontSource = timerFontSource;
        }

        timerText.font = timerFontAsset;
        timerText.fontSharedMaterial = timerFontAsset.material;
        timerText.SetAllDirty();
    }
}
