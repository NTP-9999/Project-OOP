using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    private Base house;
    [SerializeField] private float distanceFromHouse = 20f;
    public int currentDay = 1;
    public int difficulty = 1;
    public bool isNight = true;

    [Header("Enemy Settings")]
    public Enemy enemyPrefab;
    [Header("Animal Settings")]
    public Animal animalPrefab;

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

        house = FindObjectOfType<Base>();
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
        StartCoroutine(SpawnAnimals());
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
            Vector3 spawnPoint = GetRandomPointOnCircle(house.transform.position, distanceFromHouse); // assuming center is (0,0,0) and radius 20
            Enemy newEnemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
            // newEnemy.InitStats(difficulty);

            yield return new WaitForSeconds(Random.Range(1f, 3f)); // wait before spawning next
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
            Vector3 spawnPoint = GetRandomPointInCircle(house.transform.position, distanceFromHouse - 5f); // within a smaller radius
            Animal newAnimal = Instantiate(animalPrefab, spawnPoint, Quaternion.identity);

            yield return new WaitForSeconds(Random.Range(1f, 3f));
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
