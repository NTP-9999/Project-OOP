using UnityEngine;
using TMPro;

public class RainbowText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;   // Assign your TextMeshProUGUI component
    [SerializeField] private float speed = 1f; // Speed of color change

    private void Start()
    {
        if (text == null)
            text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (text == null) return;

        // Mathf.PingPong or just cycling hue
        float hue = Mathf.PingPong(Time.time * speed, 1f); 
        Color rainbowColor = Color.HSVToRGB(hue, 1f, 1f);
        text.color = rainbowColor;
    }
}
