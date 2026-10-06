using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class PuzzleLogic : MonoBehaviour
{
    private const string HighestLevelKey = "HighestLevelIndex";

    [Header("Levels")]
    [SerializeField] private List<GameObject> levelPrefabs = new();
    [SerializeField] private int menuLevelsCount = 2;

    [Header("Completion Effects")]
    [SerializeField] private GameObject winConfetti;
    [SerializeField] private AudioSource winSound;

    [Header("Puzzle Validation")]
    [SerializeField] private LayerMask puzzleLayer;
    [SerializeField] private LayerMask socketLayer;

    [SerializeField]
    [Min(0.001f)]
    private float detectionRadius = 0.01f;

    [SerializeField]
    [Min(0.01f)]
    private float relevantPartDistance = 0.15f;

    [SerializeField]
    [Min(0.1f)]
    private float blockSearchRadius = 2f;

    private readonly List<SnapInteractable> currentSockets = new();

    private GameObject currentLevelInstance;
    private int currentLevelIndex;
    private bool isLevelCompleted;

    private void Start()
    {
        SpawnLevel(0);
    }

    private void Update()
    {
        if (!ShouldValidatePuzzle())
            return;

        if (IsPuzzleSolved())
        {
            StartCoroutine(WinSequence());
        }
    }

    private bool ShouldValidatePuzzle()
    {
        return !isLevelCompleted
            && currentLevelIndex >= menuLevelsCount
            && currentLevelInstance != null
            && currentSockets.Count > 0;
    }

    private bool IsPuzzleSolved()
    {
        return AreAllSocketsOccupied()
            && AreAllRelevantBlockPartsInsideSockets();
    }

    private bool AreAllSocketsOccupied()
    {
        foreach (SnapInteractable socket in currentSockets)
        {
            bool occupied = Physics.CheckSphere(
                socket.transform.position,
                detectionRadius,
                puzzleLayer
            );

            if (!occupied)
                return false;
        }

        return true;
    }

    private bool AreAllRelevantBlockPartsInsideSockets()
    {
        Collider[] blockParts = Physics.OverlapSphere(
            currentLevelInstance.transform.position,
            blockSearchRadius,
            puzzleLayer
        );

        foreach (Collider blockPart in blockParts)
        {
            bool touchesSocket = Physics.CheckSphere(
                blockPart.transform.position,
                detectionRadius,
                socketLayer
            );

            if (!touchesSocket && IsBlockPartRelevant(blockPart.transform.position))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsBlockPartRelevant(Vector3 partPosition)
    {
        foreach (SnapInteractable socket in currentSockets)
        {
            if (Vector3.Distance(partPosition, socket.transform.position)
                < relevantPartDistance)
            {
                return true;
            }
        }

        return false;
    }

    public void SpawnLevel(int index)
    {
        if (levelPrefabs == null || index < 0 || index >= levelPrefabs.Count)
        {
            Debug.LogWarning($"Cannot load level index {index}.");
            return;
        }

        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
        }

        currentLevelIndex = index;

        currentLevelInstance = Instantiate(
            levelPrefabs[index],
            transform.position,
            Quaternion.identity
        );

        currentSockets.Clear();

        currentSockets.AddRange(
            currentLevelInstance.GetComponentsInChildren<SnapInteractable>(true)
        );

        isLevelCompleted = false;

        if (winConfetti != null)
        {
            winConfetti.SetActive(false);
        }
    }

    public void LoadSavedLevel()
    {
        if (levelPrefabs == null || levelPrefabs.Count == 0)
            return;

        int targetIndex = PlayerPrefs.GetInt(
            HighestLevelKey,
            menuLevelsCount
        );

        targetIndex = Mathf.Clamp(
            targetIndex,
            0,
            levelPrefabs.Count - 1
        );

        if (targetIndex < menuLevelsCount
            && levelPrefabs.Count > menuLevelsCount)
        {
            targetIndex = menuLevelsCount;
        }

        SpawnLevel(targetIndex);
    }

    private IEnumerator WinSequence()
    {
        isLevelCompleted = true;

        Debug.Log($"Puzzle level {currentLevelIndex} completed.");

        if (winConfetti != null)
        {
            winConfetti.SetActive(true);
        }

        if (winSound != null)
        {
            winSound.Play();

            winSound.SetScheduledEndTime(
                AudioSettings.dspTime + 5.0
            );
        }

        SaveProgress();

        yield return new WaitForSeconds(4f);

        SpawnLevel(0);
    }

    private void SaveProgress()
    {
        int nextLevelIndex = currentLevelIndex + 1;

        int highestLevel = PlayerPrefs.GetInt(
            HighestLevelKey,
            menuLevelsCount
        );

        if (nextLevelIndex <= highestLevel)
            return;

        PlayerPrefs.SetInt(
            HighestLevelKey,
            nextLevelIndex
        );

        PlayerPrefs.Save();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (currentSockets == null)
            return;

        foreach (SnapInteractable socket in currentSockets)
        {
            if (socket != null)
            {
                Gizmos.DrawWireSphere(
                    socket.transform.position,
                    detectionRadius
                );
            }
        }
    }
}