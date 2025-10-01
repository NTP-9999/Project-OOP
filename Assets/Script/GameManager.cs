using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public int currentDay = 1;
    public int difficulty = 1;
    public bool isNight = false;

    [Header("Enemy Settings")]
    public Enemy enemyPrefab;
    public Transform[] spawnPoints;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [System.Obsolete]
    private void Start()
    {
        // Find the DayNightCycle in scene
        DayNightCycle cycle = FindObjectOfType<DayNightCycle>();
        if (cycle != null)
        {
            // Hook into its events
            cycle.OnSunset.AddListener(StartNight);
            cycle.OnSunrise.AddListener(StartDay);
            cycle.OnNewDay.AddListener(NewDay);
        }
    }

    private void StartNight()
    {
        isNight = true;
        Debug.Log($"🌙 Night {currentDay} begins!");
        SpawnEnemyWave();
    }

    private void StartDay()
    {
        isNight = false;
        Debug.Log("☀️ Daytime is here, enemies stop spawning.");
    }

    private void NewDay()
    {
        currentDay++;
        IncreaseDifficulty();
    }

    private void IncreaseDifficulty()
    {
        difficulty++;
        Debug.Log($"📈 Difficulty increased → {difficulty}");
    }

    private void SpawnEnemyWave()
    {
        int enemiesToSpawn = currentDay * 2; // formula, tweak as needed

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Enemy newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            //newEnemy.InitStats(difficulty);
        }

        Debug.Log($"Spawned {enemiesToSpawn} enemies for Night {currentDay}.");
    }
}
