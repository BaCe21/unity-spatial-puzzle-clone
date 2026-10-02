using UnityEngine;
using Oculus.Interaction;

public class MagicSocket : MonoBehaviour
{
    public enum ActionType
    {
        LoadSpecificLevel,  
        LoadLatestLevel,
        QuitApplication
    }

    [Header("Co ma robić ten Socket?")]
    public ActionType actionType;

    [Header("Jeśli 'LoadSpecificLevel', który to index?")]
    public int levelIndexToLoad = 0;

    [Header("Podłącz tutaj SnapInteractable z tego obiektu")]
    public SnapInteractable mySocket;

    void Start()
    {
        if (mySocket != null)
        {
            mySocket.WhenStateChanged += HandleStateChange;
        }
    }

    void OnDestroy()
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

   void PerformAction()
    {
        var manager = FindAnyObjectByType<PuzzleLogic>();
        
        if (manager == null) return;

        switch (actionType)
        {
            case ActionType.LoadSpecificLevel:
                manager.SpawnLevel(levelIndexToLoad);
                break;

            case ActionType.LoadLatestLevel:
                manager.LoadSavedLevel();
                break;

            case ActionType.QuitApplication:
                Application.Quit();
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif
                break;
        }
    }
}