using Oculus.Interaction;
using UnityEngine;

public class MagicSocket : MonoBehaviour
{
    public enum ActionType
    {
        LoadSpecificLevel,
        LoadLatestLevel,
        QuitApplication
    }

    [Header("Action")]
    [SerializeField] private ActionType actionType;

    [SerializeField]
    [Min(0)]
    private int levelIndexToLoad;

    [Header("References")]
    [SerializeField] private SnapInteractable mySocket;
    [SerializeField] private PuzzleLogic puzzleLogic;

    private void Awake()
    {
        if (puzzleLogic == null)
        {
            puzzleLogic = FindAnyObjectByType<PuzzleLogic>();
        }
    }

    private void OnEnable()
    {
        if (mySocket != null)
        {
            mySocket.WhenStateChanged += HandleStateChange;
        }
    }

    private void OnDisable()
    {
        if (mySocket != null)
        {
            mySocket.WhenStateChanged -= HandleStateChange;
        }
    }

    private void HandleStateChange(InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Select)
        {
            PerformAction();
        }
    }

    private void PerformAction()
    {
        if (actionType == ActionType.QuitApplication)
        {
            QuitApplication();
            return;
        }

        if (puzzleLogic == null)
        {
            Debug.LogWarning(
                $"{nameof(MagicSocket)} on {name} cannot find {nameof(PuzzleLogic)}."
            );
            return;
        }

        switch (actionType)
        {
            case ActionType.LoadSpecificLevel:
                puzzleLogic.SpawnLevel(levelIndexToLoad);
                break;

            case ActionType.LoadLatestLevel:
                puzzleLogic.LoadSavedLevel();
                break;
        }
    }

    private static void QuitApplication()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}