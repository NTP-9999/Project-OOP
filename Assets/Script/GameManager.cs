using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    private Base house;
    [SerializeField] private float distanceFromHouse = 20f;
    public int currentDay = 0;
    public int difficulty = 1;
    public bool isNight = true;

    private Enemy enemyPrefab;
    private Animal animalPrefab;

    [Header("Entity Tracking")]
    [SerializeField] private List<Enemy> enemies = new();
    [SerializeField] private List<Animal> animals = new();

    [SerializeField] private TMP_Text dayText;

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
            cycle.OnNewDay.AddListener(NewDay);
        }

        house = FindObjectOfType<Base>();
        enemyPrefab = Resources.Load<Enemy>("Prefabs/Enemy");
        animalPrefab = Resources.Load<Animal>("Prefabs/Deer");

        StartDay();
    }

    private void Update()
    {
        dayText.text = $"Day : {currentDay}";
    }

    public void StartNight()
    {
        isNight = true;
        Debug.Log($"🌙 Night {currentDay} begins!");
        StartCoroutine(SpawnEnemyWave());
    }

    public void StartDay()
    {
        isNight = false;
        currentDay++;
        difficulty++;
        StartCoroutine(SpawnAnimals());
        Debug.Log("☀️ Daytime is here, enemies stop spawning.");
    }

    private void NewDay()
    {
        IncreaseDifficulty();
    }

    private void IncreaseDifficulty()
    {
        Debug.Log($"📈 Difficulty increased → {difficulty}");
    }

    public void ClearEnemy()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }
        enemies.Clear();
    }

    public void ClearAnimals()
    {
        foreach (Animal animal in animals)
        {
            if (animal != null)
            {
                Destroy(animal.gameObject);
            }
        }
        animals.Clear();
    }

    private IEnumerator SpawnEnemyWave()
    {
        int enemiesToSpawn = currentDay * 2; // formula, tweak as needed

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Vector3 spawnPoint = GetRandomPointOnCircle(house.transform.position, distanceFromHouse); // assuming center is (0,0,0) and radius 20
            if (NavMesh.SamplePosition(spawnPoint, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                // ใช้ hit.position แทน spawnPoint
                Enemy newEnemy = Instantiate(enemyPrefab, hit.position, Quaternion.identity);
                enemies.Add(newEnemy);
            }
            else
            {
                Debug.LogWarning($"⚠️ Failed to find NavMesh near {spawnPoint}, skipping spawn.");
            }

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

            if (NavMesh.SamplePosition(spawnPoint, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                Animal newAnimal = Instantiate(animalPrefab, spawnPoint, Quaternion.identity);
                animals.Add(newAnimal);
            }
            else
            {
                Debug.LogWarning($"⚠️ Failed to find NavMesh near {spawnPoint}, skipping spawn.");
            }

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

        return new Vector3(center.x + x, center.y , center.z + z);
    }

}
