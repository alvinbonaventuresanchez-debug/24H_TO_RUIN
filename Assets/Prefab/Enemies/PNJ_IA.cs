using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PNJ_IA : MonoBehaviour
{
    [SerializeField] private float vitesse = 1f;
    [SerializeField] private float dureeAvance = 1.5f;
    [SerializeField] private AnimationClip walkClip;
    [SerializeField] private string walkClipName = "mixamo.com";

    private float tempsEcoule;
    private Animator animator;
    private PlayableGraph walkGraph;
    private AnimationClipPlayable walkPlayable;
    private bool walkGraphReady;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (walkClip == null)
        {
            walkClip = FindWalkClipAtRuntime();
        }

        TryCreateWalkGraph();
        tempsEcoule = 0f;
        RestartWalkingAnimation();
    }

    private void OnEnable()
    {
        tempsEcoule = 0f;
        RestartWalkingAnimation();
    }

    private void Update()
    {
        bool isWalking = tempsEcoule < dureeAvance;

        if (isWalking)
        {
            transform.Translate(Vector3.forward * vitesse * Time.deltaTime, Space.Self);
            tempsEcoule += Time.deltaTime;
        }

        SetWalkingAnimationEnabled(isWalking);
    }

    private void OnDestroy()
    {
        if (walkGraph.IsValid())
        {
            walkGraph.Destroy();
        }
    }

    private void TryCreateWalkGraph()
    {
        if (walkGraphReady || animator == null || walkClip == null)
        {
            return;
        }

        walkGraph = PlayableGraph.Create($"{name}_WalkGraph");
        AnimationPlayableOutput output = AnimationPlayableOutput.Create(walkGraph, "Walk", animator);
        walkPlayable = AnimationClipPlayable.Create(walkGraph, walkClip);
        output.SetSourcePlayable(walkPlayable);
        walkGraph.Play();
        walkGraphReady = true;
    }

    private void SetWalkingAnimationEnabled(bool isWalking)
    {
        if (!walkGraphReady)
        {
            return;
        }

        walkPlayable.SetSpeed(isWalking ? 1f : 0f);
    }

    private void RestartWalkingAnimation()
    {
        if (!walkGraphReady)
        {
            return;
        }

        walkPlayable.SetTime(0d);
        SetWalkingAnimationEnabled(dureeAvance > 0f);
    }

    private AnimationClip FindWalkClipAtRuntime()
    {
        AnimationClip[] clips = Resources.FindObjectsOfTypeAll<AnimationClip>();

        foreach (AnimationClip clip in clips)
        {
            if (clip == null || clip.name != walkClipName)
            {
                continue;
            }

            if (clip.name.StartsWith("__preview__"))
            {
                continue;
            }

            return clip;
        }

        return null;
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (walkClip != null)
        {
            return;
        }

        Object[] assets = AssetDatabase.LoadAllAssetsAtPath("Assets/models/Walk With Briefcase.fbx");

        foreach (Object asset in assets)
        {
            AnimationClip clip = asset as AnimationClip;

            if (clip == null)
            {
                continue;
            }

            if (clip.name != walkClipName)
            {
                continue;
            }

            walkClip = clip;
            EditorUtility.SetDirty(this);
            return;
        }
    }
#endif
}
