using UnityEngine;
using TMPro;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private float tempsRestant = 720f;
    public TextMeshProUGUI timerText;
    [SerializeField] private Font timerFontSource;
    [SerializeField] private string timeoutSceneName = "Defeat";

    private TMP_FontAsset timerFontAsset;
    private Font appliedFontSource;
    private bool hasTimerEnded;

    public float TempsRestant
    {
        get => tempsRestant;
        set
        {
            tempsRestant = Mathf.Max(0f, value);
            UpdateTimerDisplay();
        }
    }

    public bool HasTimerEnded => hasTimerEnded;

    void Awake()
    {
        ApplyTimerFont();
        UpdateTimerDisplay();
    }

    void OnValidate()
    {
        ApplyTimerFont();
    }

    void Update()
    {
        if (hasTimerEnded)
        {
            return;
        }

        if (tempsRestant > 0)
        {
            tempsRestant -= Time.deltaTime;

            if (tempsRestant < 0)
            {
                tempsRestant = 0;
            }

            UpdateTimerDisplay();

            if (tempsRestant <= 0)
            {
                EndGameOnTimeout();
            }
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(tempsRestant / 60);
        int secondes = Mathf.FloorToInt(tempsRestant % 60);

        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, secondes);
        }
    }

    void EndGameOnTimeout()
    {
        hasTimerEnded = true;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        string sceneToLoad = string.IsNullOrWhiteSpace(timeoutSceneName) ? "Defeat" : timeoutSceneName;
        SceneManager.LoadScene(sceneToLoad);
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
