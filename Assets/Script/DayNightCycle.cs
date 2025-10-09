using UnityEngine;
using System;
using UnityEngine.Events;

[Serializable]
public class DayNightSettings
{
    [Header("Time Settings")]
    [Range(0.1f, 10f)]
    public float timeSpeed = 1f;

    [Range(0f, 24f)]
    public float startTime = 6f; // Start at 6 AM

    [Header("Sun Settings")]
    public Light sunLight;
    public Gradient sunColor = new();
    public AnimationCurve sunIntensity = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Moon Settings")]
    public Light moonLight;
    public Gradient moonColor = new();
    public AnimationCurve moonIntensityCurve;

    [Header("Sky Settings")]
    public Material skyboxMaterial;
    public Gradient skyTint = new();
    public AnimationCurve skyExposure = AnimationCurve.EaseInOut(0, 0.5f, 1, 1.3f);

    [Header("Fog Settings")]
    public bool useFog = true;
    public Gradient fogColor = new();
    public AnimationCurve fogDensity = AnimationCurve.EaseInOut(0, 0.01f, 1, 0.05f);
}

public class DayNightCycle : MonoBehaviour
{
    [Header("Day/Night Cycle Controller")]
    public DayNightSettings settings = new();

    [Header("Debug Info")]
    [SerializeField, Range(0f, 24f)]
    private float currentTime;

    [SerializeField]
    private string timeDisplay;

    [Header("Events")]
    public UnityEvent OnSunrise;
    public UnityEvent OnNoon;
    public UnityEvent OnSunset;
    public UnityEvent OnMidnight;
    public UnityEvent OnNewDay;

    // Private variables
    private bool hasSunriseTriggered, hasNoonTriggered, hasSunsetTriggered, hasMidnightTriggered;
    [SerializeField]
    private int dayCount = 1;

    // Skybox runtime handling
    private Material originalSkyboxMaterial;
    private Material runtimeSkyboxMaterial;

    // Public properties for easy access
    public float CurrentTime => currentTime;
    public bool IsDay => currentTime >= 6f && currentTime < 18f;
    public bool IsNight => !IsDay;
    public string TimeString => FormatTime(currentTime);
    public int DayCount => dayCount;

    void Start()
    {
        InitializeDayNightCycle();
    }

    void Update()
    {
        UpdateTime();
        UpdateLighting();
        UpdateSkybox();
        UpdateFog();
        CheckTimeEvents();
        UpdateDebugDisplay();
    }

    void InitializeDayNightCycle()
    {
        currentTime = settings.startTime;

        SetupDefaultSunGradient();
        
        SetupDefaultMoonGradient();

        SetupDefaultSkyGradient();

        SetupDefaultFogGradient();

        settings.moonIntensityCurve = new AnimationCurve(
            new Keyframe(0f, 0.2f),   // 18:00 เริ่มมีแสงนิด ๆ
            new Keyframe(0.25f, 0.05f), // 21:00 เริ่มมืด
            new Keyframe(0.5f, 0.02f),  // 00:00 มืดสุด
            new Keyframe(0.75f, 0.05f), // 03:00 เริ่มสว่างขึ้นเล็กน้อย
            new Keyframe(1f, 0.2f)    // 06:00 จางหายไป
        );

        // Enable fog if using it
        if (settings.useFog)
        {
            RenderSettings.fog = true;
        }

        // Prepare runtime skybox instance to avoid editing the asset during Play Mode
        originalSkyboxMaterial = RenderSettings.skybox;

        Material sourceSkybox = settings.skyboxMaterial != null ? settings.skyboxMaterial : originalSkyboxMaterial;
        if (sourceSkybox != null)
        {
            runtimeSkyboxMaterial = new Material(sourceSkybox)
            {
                name = sourceSkybox.name + " (Runtime)"
            };
            RenderSettings.skybox = runtimeSkyboxMaterial;
        }
    }

    void UpdateTime()
    {
        currentTime += Time.deltaTime * settings.timeSpeed;

        if (currentTime >= 24f)
        {
            currentTime = 0f;
            dayCount++;
            ResetDailyEvents();
            OnNewDay?.Invoke();
        }
    }

    void UpdateLighting()
    {
        float normalizedTime = currentTime / 24f;

        // Update Sun
        if (settings.sunLight != null)
        {
            // Sun rotation (rises in east, sets in west)
            float sunAngle = currentTime * 15f; // 15 degrees per hour, starting at sunrise
            settings.sunLight.transform.rotation = Quaternion.Euler(sunAngle - 90f, 30f, 0f);

            // Sun color and intensity
            settings.sunLight.color = settings.sunColor.Evaluate(normalizedTime);
            settings.sunLight.intensity = settings.sunIntensity.Evaluate(normalizedTime);

            // Disable sun at night
            settings.sunLight.enabled = IsDay;
        }

        // Update Moon
        if (settings.moonLight != null)
        {
            // Moon rotation (opposite to sun)
            float moonAngle = currentTime * 15f + 180f;
            settings.moonLight.transform.rotation = Quaternion.Euler(moonAngle - 90f, -30f, 0f);

            float nightT = 0f;
            if (currentTime >= 18f)
                nightT = (currentTime - 18f) / 12f; // 18–24 => 0–0.5
            else
                nightT = (currentTime + 6f) / 12f;  // 0–6 => 0.5–1

            settings.moonLight.color = settings.moonColor.Evaluate(nightT);
            settings.moonLight.intensity = settings.moonIntensityCurve.Evaluate(nightT);
            settings.moonLight.enabled = IsNight;
        }
    }

    void UpdateSkybox()
    {
        Material targetSkybox = runtimeSkyboxMaterial != null ? runtimeSkyboxMaterial : settings.skyboxMaterial;
        if (targetSkybox != null)
        {
            float normalizedTime = currentTime / 24f;

            // Update sky tint
            if (targetSkybox.HasProperty("_Tint"))
            {
                targetSkybox.SetColor("_Tint", settings.skyTint.Evaluate(normalizedTime));
            }

            // Update sky exposure
            if (targetSkybox.HasProperty("_Exposure"))
            {
                targetSkybox.SetFloat("_Exposure", settings.skyExposure.Evaluate(normalizedTime));
            }

            if (RenderSettings.skybox != targetSkybox)
            {
                RenderSettings.skybox = targetSkybox;
            }
        }
    }

    void UpdateFog()
    {
        if (settings.useFog)
        {
            float normalizedTime = currentTime / 24f;
            RenderSettings.fogColor = settings.fogColor.Evaluate(normalizedTime);
            RenderSettings.fogDensity = settings.fogDensity.Evaluate(normalizedTime);
        }
    }

    void CheckTimeEvents()
    {
        // Sunrise (6 AM)
        if (currentTime >= 6f && currentTime < 6.1f && !hasSunriseTriggered)
        {
            OnSunrise?.Invoke();
            hasSunriseTriggered = true;
        }

        // Noon (12 PM)
        if (currentTime >= 12f && currentTime < 12.1f && !hasNoonTriggered)
        {
            OnNoon?.Invoke();
            hasNoonTriggered = true;
        }

        // Sunset (6 PM)
        if (currentTime >= 18f && currentTime < 18.1f && !hasSunsetTriggered)
        {
            OnSunset?.Invoke();
            hasSunsetTriggered = true;
        }

        // Midnight (12 AM)
        if (currentTime >= 0f && currentTime < 0.1f && !hasMidnightTriggered)
        {
            OnMidnight?.Invoke();
            hasMidnightTriggered = true;
        }
    }

    void ResetDailyEvents()
    {
        hasSunriseTriggered = hasNoonTriggered = hasSunsetTriggered = hasMidnightTriggered = false;
    }

    void OnDisable()
    {
        if (runtimeSkyboxMaterial != null)
        {
            if (Application.isPlaying)
            {
                Destroy(runtimeSkyboxMaterial);
            }
            else
            {
                DestroyImmediate(runtimeSkyboxMaterial);
            }
            runtimeSkyboxMaterial = null;
        }

        if (originalSkyboxMaterial != null)
        {
            RenderSettings.skybox = originalSkyboxMaterial;
        }
    }

    void UpdateDebugDisplay()
    {
        timeDisplay = FormatTime(currentTime);
    }

    string FormatTime(float time)
    {
        int hours = Mathf.FloorToInt(time);
        int minutes = Mathf.FloorToInt((time - hours) * 60);
        string period = hours >= 12 ? "PM" : "AM";
        int displayHour = hours == 0 ? 12 : (hours > 12 ? hours - 12 : hours);
        return $"{displayHour:00}:{minutes:00} {period}";
    }

    void SetupDefaultSunGradient()
    {
        GradientColorKey[] colorKeys = new GradientColorKey[5];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[5];

        // Night -> Dawn -> Day -> Dusk -> Night (loop)
        Color nightColor = new(0.2f, 0.2f, 0.4f);
        colorKeys[0] = new GradientColorKey(nightColor, 0f);     // Night
        colorKeys[1] = new GradientColorKey(new Color(1f, 0.6f, 0.3f), 0.25f);    // Dawn
        colorKeys[2] = new GradientColorKey(new Color(1f, 0.95f, 0.8f), 0.5f);    // Day
        colorKeys[3] = new GradientColorKey(new Color(1f, 0.4f, 0.2f), 0.75f);    // Dusk
        colorKeys[4] = new GradientColorKey(nightColor, 1f);     // Night (same as 0f)

        for (int i = 0; i < 4; i++)
            alphaKeys[i] = new GradientAlphaKey(1f, i * 0.25f);
        alphaKeys[4] = new GradientAlphaKey(1f, 1f); // Ensure last alpha matches first

        settings.sunColor.SetKeys(colorKeys, alphaKeys);

        settings.sunIntensity = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 0f),    // Midnight, low, flat
            new Keyframe(0.5f, 1f, 0f, 0f),    // Noon, high, flat
            new Keyframe(1f, 0f, 0f, 0f)     // Midnight, low, flat (same as 0f)
        );
    }

    void SetupDefaultMoonGradient()
    {
        GradientColorKey[] colorKeys = new GradientColorKey[4];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[4];

        // สีแสงพระจันทร์ (จริง ๆ แล้วคือสีของแสง Directional Light ตอนกลางคืน)
        // เน้นให้แสงเย็น ๆ จาง ๆ ไม่สว่างจนเกินไป
        colorKeys[0] = new GradientColorKey(new Color(0.5f, 0.55f, 0.7f), 0f);   // 18:00 - เริ่มขึ้น
        colorKeys[1] = new GradientColorKey(new Color(0.35f, 0.4f, 0.55f), 0.3f); // 21:00 - ฟ้ามืด
        colorKeys[2] = new GradientColorKey(new Color(0.25f, 0.3f, 0.45f), 0.6f); // 00:00 - มืดสุด
        colorKeys[3] = new GradientColorKey(new Color(0.4f, 0.45f, 0.6f), 1f);    // 06:00 - เริ่มจางก่อนเช้า

        for (int i = 0; i < colorKeys.Length; i++)
            alphaKeys[i] = new GradientAlphaKey(1f, colorKeys[i].time);

        settings.moonColor = new Gradient();
        settings.moonColor.SetKeys(colorKeys, alphaKeys);
    }



    void SetupDefaultSkyGradient()
    {
        GradientColorKey[] colorKeys = new GradientColorKey[5];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[5];

        colorKeys[0] = new GradientColorKey(new Color(0.02f, 0.03f, 0.06f), 0f);   // เที่ยงคืน – มืดมาก
        colorKeys[1] = new GradientColorKey(new Color(0.05f, 0.07f, 0.12f), 0.25f); // 06:00 – ฟ้ารุ่งสาง
        colorKeys[2] = new GradientColorKey(new Color(0.55f, 0.75f, 1f), 0.5f);     // กลางวัน
        colorKeys[3] = new GradientColorKey(new Color(0.2f, 0.1f, 0.2f), 0.75f);    // 18:00 – ฟ้าเย็น
        colorKeys[4] = new GradientColorKey(new Color(0.02f, 0.03f, 0.06f), 1f);    // เที่ยงคืน – มืดอีกครั้ง

        for (int i = 0; i < colorKeys.Length; i++)
            alphaKeys[i] = new GradientAlphaKey(1f, colorKeys[i].time);

        settings.skyTint.SetKeys(colorKeys, alphaKeys);

        settings.skyExposure = new AnimationCurve(
            new Keyframe(0f, 0.15f),   // เที่ยงคืน – มืดมาก
            new Keyframe(0.25f, 0.4f), // ฟ้ารุ่งสาง
            new Keyframe(0.5f, 1.2f),  // กลางวัน
            new Keyframe(0.75f, 0.4f), // เย็น
            new Keyframe(1f, 0.15f)    // เที่ยงคืน – มืดอีกครั้ง
        );
    }

    void SetupDefaultFogGradient()
    {
        GradientColorKey[] colorKeys = new GradientColorKey[4];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[4];

        Color nightColor = new(0.1f, 0.1f, 0.2f);
        colorKeys[0] = new GradientColorKey(nightColor, 0f);     // Night
        colorKeys[1] = new GradientColorKey(new Color(0.6f, 0.4f, 0.3f), 0.3f);   // Dawn
        colorKeys[2] = new GradientColorKey(new Color(0.7f, 0.8f, 1f), 0.7f);     // Day
        colorKeys[3] = new GradientColorKey(nightColor, 1f);     // Night (same as 0f)

        for (int i = 0; i < 3; i++)
            alphaKeys[i] = new GradientAlphaKey(1f, i * 0.33f);
        alphaKeys[3] = new GradientAlphaKey(1f, 1f); // Ensure last alpha matches first

        settings.fogColor.SetKeys(colorKeys, alphaKeys);

        settings.fogDensity = new AnimationCurve(
            new Keyframe(0f, 0.01f, 0f, 0f),    // Midnight, low, flat
            new Keyframe(0.5f, 0.05f, 0f, 0f),    // Noon, high, flat
            new Keyframe(1f, 0.01f, 0f, 0f)     // Midnight, low, flat (same as 0f)
        );
    }

    // Public methods for external control
    public void SetTime(float time)
    {
        currentTime = Mathf.Clamp(time, 0f, 24f);
    }

    public void SetTimeSpeed(float speed)
    {
        settings.timeSpeed = Mathf.Max(0.1f, speed);
    }

    public void PauseTime()
    {
        settings.timeSpeed = 0f;
    }

    public void ResumeTime()
    {
        settings.timeSpeed = 1f;
    }

    public void SetDay(int day)
    {
        dayCount = Mathf.Max(1, day);
    }
}