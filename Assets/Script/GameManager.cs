using UnityEngine;
using System.Collections;

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
    [SerializeField] private float spawnDelay = 1f; // delay between spawns

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
            cycle.OnSunset.AddListener(StartNight);
            cycle.OnSunrise.AddListener(StartDay);
            cycle.OnNewDay.AddListener(NewDay);
        }
    }

    private void StartNight()
    {
        isNight = true;
        Debug.Log($"🌙 Night {currentDay} begins!");
        StartCoroutine(SpawnEnemyWave());
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

    private IEnumerator SpawnEnemyWave()
    {
        int enemiesToSpawn = currentDay * 2; // formula, tweak as needed

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Enemy newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

            // If you want to scale stats:
            // newEnemy.InitStats(difficulty);

            Debug.Log($"Spawned enemy {i + 1}/{enemiesToSpawn} at {spawnPoint.name}");

            yield return new WaitForSeconds(spawnDelay); // wait before spawning next
        }

        Debug.Log($"✅ Finished spawning {enemiesToSpawn} enemies for Night {currentDay}.");
    }
}
