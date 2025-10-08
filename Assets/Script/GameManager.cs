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
    public Transform[] enemySpawnPoints;
    [SerializeField] private float enemySpawnDelay = 1f;

    [Header("Animal Settings")]
    public Animal animalPrefab;
    public Transform[] animalSpawnPoints;
    [SerializeField] private float animalSpawnDelay = 2f;

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

    private void Start()
    {
        DayNightCycle cycle = FindFirstObjectByType<DayNightCycle>();
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
        Debug.Log($"☀️ Day {currentDay} begins! Animals will spawn...");
        StartCoroutine(SpawnAnimals());
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

    // -------------------------
    // Enemy Spawning
    // -------------------------
    private IEnumerator SpawnEnemyWave()
    {
        int enemiesToSpawn = currentDay * 2; // scale enemy count

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Transform spawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length)];
            Enemy newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            // newEnemy.InitStats(difficulty);

            yield return new WaitForSeconds(enemySpawnDelay);
        }

        Debug.Log($"✅ Finished spawning {enemiesToSpawn} enemies for Night {currentDay}.");
    }

    // -------------------------
    // Animal Spawning
    // -------------------------
    private IEnumerator SpawnAnimals()
    {
        // Example: fewer animals than enemies
        int animalsToSpawn = Mathf.Max(1, currentDay); // at least 1, then scale slowly

        for (int i = 0; i < animalsToSpawn; i++)
        {
            Transform spawnPoint = animalSpawnPoints[Random.Range(0, animalSpawnPoints.Length)];
            Animal newAnimal = Instantiate(animalPrefab, spawnPoint.position, Quaternion.identity);

            Debug.Log($"Spawned animal {i + 1}/{animalsToSpawn} at {spawnPoint.name}");

            yield return new WaitForSeconds(animalSpawnDelay);
        }

        Debug.Log($"✅ Finished spawning {animalsToSpawn} animals for Day {currentDay}.");
    }
}
