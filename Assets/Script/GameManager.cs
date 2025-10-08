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
    private Base house;
    [SerializeField] private float distanceFromHouse = 20f;
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

        house = FindFirstObjectByType<Base>();
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
            Vector3 spawnPoint = GetRandomPointOnCircle(house.transform.position, distanceFromHouse);
            Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
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
            Vector3 spawnPoint = GetRandomPointInCircle(house.transform.position, distanceFromHouse / 2f);
            Instantiate(animalPrefab, spawnPoint, Quaternion.identity);

            yield return new WaitForSeconds(animalSpawnDelay);
        }

        Debug.Log($"✅ Finished spawning {animalsToSpawn} animals for Day {currentDay}.");
    }

    Vector3 GetRandomPointOnCircle(Vector3 center, float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        return new Vector3(center.x + x, center.y, center.z + z);
    }

    Vector3 GetRandomPointInCircle(Vector3 center, float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);

        float r = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;

        float x = Mathf.Cos(angle) * r;
        float z = Mathf.Sin(angle) * r;

        return new Vector3(center.x + x, center.y, center.z + z);
    }

}
