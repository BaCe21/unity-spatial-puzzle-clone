using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class PuzzleLogic : MonoBehaviour
{
    [Header("Konfiguracja")]
    public List<GameObject> levelPrefabs;
    public int menuLevelsCount = 2; 

    [Header("Efekty")]
    public GameObject winConfetti;
    public AudioSource winSound;

    [Header("Detekcja Fizyczna")]
    public LayerMask puzzleLayer;
    public LayerMask socketLayer;
    public float detectionRadius = 0.01f;

    private GameObject currentLevelInstance;
    private List<SnapInteractable> currentSockets = new List<SnapInteractable>();
    private int currentLevelIndex = 0;
    private bool isLevelCompleted = false;
    private const string SAVE_KEY = "HighestLevelIndex";

    void Start() 
    { 
        SpawnLevel(0); 
    }

    void Update()
    {
        if (isLevelCompleted) return;
        if (currentLevelIndex < menuLevelsCount) return;
        if (currentLevelInstance == null || currentSockets.Count == 0) return;

        // WARUNEK 1: Czy wszystkie GNIAZDA są pełne?
        bool socketsHappy = true;
        foreach (var socket in currentSockets)
        {
            if (!Physics.CheckSphere(socket.transform.position, detectionRadius, puzzleLayer))
            {
                socketsHappy = false;
                break;
            }
        }

        // WARUNEK 2: Czy żaden KLOCEK nie wystaje?
        bool blocksHappy = true;
        
        Collider[] activeBlockParts = Physics.OverlapSphere(currentLevelInstance.transform.position, 2.0f, puzzleLayer);

        foreach (var blockPart in activeBlockParts)
        {
            bool touchesSocket = Physics.CheckSphere(blockPart.transform.position, detectionRadius, socketLayer);
            
            if (!touchesSocket)
            {
                if (IsBlockPartRelevant(blockPart.transform.position))
                {
                     blocksHappy = false;
                     break;
                }
            }
        }

        if (socketsHappy && blocksHappy)
        {
            Debug.Log(">> POZIOM UKOŃCZONY PRECYZYJNIE!");
            StartCoroutine(WinSequence());
        }
    }

    bool IsBlockPartRelevant(Vector3 partPosition)
    {
        foreach(var socket in currentSockets)
        {
            if(Vector3.Distance(partPosition, socket.transform.position) < 0.15f)
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
            Debug.LogWarning($"Próba załadowania poziomu {index}, który nie istnieje! Zostaję tu gdzie jestem.");
            return; 
        }

        if (currentLevelInstance != null) Destroy(currentLevelInstance);

        currentLevelIndex = index;
        currentLevelInstance = Instantiate(levelPrefabs[index], transform.position, Quaternion.identity);

        currentSockets.Clear();
        currentSockets.AddRange(currentLevelInstance.GetComponentsInChildren<SnapInteractable>(true));
        
        isLevelCompleted = false;
        if (winConfetti != null) winConfetti.SetActive(false);
    }

    // ŁADOWANIE ZAPISU 
    public void LoadSavedLevel()
    {
        int targetIndex = PlayerPrefs.GetInt(SAVE_KEY, menuLevelsCount);
        targetIndex = Mathf.Clamp(targetIndex, 0, levelPrefabs.Count - 1);
        if (targetIndex < menuLevelsCount && levelPrefabs.Count > menuLevelsCount)
        {
            targetIndex = menuLevelsCount;
        }

        Debug.Log($">> Bezpieczne ładowanie poziomu: {targetIndex}");
        SpawnLevel(targetIndex);
    }

    IEnumerator WinSequence()
    {
        isLevelCompleted = true;
        if (winConfetti != null) winConfetti.SetActive(true);
        if (winSound != null) 
        {
            winSound.Play();
            winSound.SetScheduledEndTime(AudioSettings.dspTime + 5.0f);
        }
        
        int nextLevelIndex = currentLevelIndex + 1;
        
        // Zapisujemy postęp tylko jeśli idziemy do przodu
        if (nextLevelIndex > PlayerPrefs.GetInt(SAVE_KEY, 0))
        {
            PlayerPrefs.SetInt(SAVE_KEY, nextLevelIndex);
            PlayerPrefs.Save();
        }
        
        yield return new WaitForSeconds(4.0f);
        SpawnLevel(0);
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (currentSockets != null)
            foreach (var s in currentSockets) if(s) Gizmos.DrawWireSphere(s.transform.position, detectionRadius);
    }
}