using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    private Base house;
    public int currentDay = 1;
    public int difficulty = 1;
    public bool isNight = false;

    [Header("Enemy Settings")]
    private Enemy enemyPrefab;
    [SerializeField] private float distance = 10f;
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
        enemyPrefab = Resources.Load<Enemy>("Prefabs/Enemy");

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
            Vector3 spawnPos = GetRandomPointOnCircle(house.transform.position, distance);
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(spawnDelay); // wait before spawning next
        }

        Debug.Log($"✅ Finished spawning {enemiesToSpawn} enemies for Night {currentDay}.");
    }

    private Vector3 GetRandomPointOnCircle(Vector3 center, float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        
        return new Vector3(center.x + x, center.y, center.z + z);
    }
}
