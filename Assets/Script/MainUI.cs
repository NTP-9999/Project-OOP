using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using System.Collections;

public class MainUI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button saveButton;

    private void Awake()
    {
        saveButton.onClick.AddListener(() => {
            var data = Player.Instance.GetData();
            SaveData(data.name, data.health, data.stamina, data.hungry, data.fatigue); 
        });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !Player.Instance.IsDead)
        {
            SetActive(pausePanel, !pausePanel.activeSelf);
        }
    }
    private void SetActive(GameObject panel, bool active)
    {
        if (panel == null) return;

        panel.SetActive(active);
        if (active)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f; // หยุดเกม
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f; // เล่นเกมต่อ
        }
    }

    public void SaveData(string name, float health, float stamina, int hungry, int fatigue) {
        StartCoroutine(PostScore(name, health, stamina, hungry, fatigue));
    }

    private IEnumerator PostScore(string name, float health, float stamina, int hungry, int fatigue) {
        // สร้าง JSON
        string json = "{\"name\":\"" + name + "\", \"health\":" + health + ", \"stamina\":" + stamina + ", \"hungry\":" + hungry + ", \"fatigue\":" + fatigue + "}";
        
        // ยิงไปที่ Server ของคุณ (ถ้าทดสอบในเครื่องใช้ localhost)
        var request = new UnityWebRequest("http://localhost:3000/SaveScore", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) {
            Debug.Log("บันทึกสำเร็จ!");
            Destroy(gameObject);
        } else {
            Debug.Log("Error: " + request.error);
        }
    }
}
