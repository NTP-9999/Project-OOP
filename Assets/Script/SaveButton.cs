using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class SaveButton : MonoBehaviour {

    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField scoreInput;
    [SerializeField] private Button saveButton;

    private void Awake()
    {
        saveButton.onClick.AddListener(() => {
            if (!string.IsNullOrEmpty(nameInput.text) && !string.IsNullOrEmpty(scoreInput.text))
                SaveScore(nameInput.text, int.Parse(scoreInput.text)); // ตัวอย่างการบันทึกคะแนน
            else
                Debug.Log("กรุณากรอกชื่อและคะแนนให้ถูกต้อง");
        });
    }
    public void SaveScore(string name, int score) {
        StartCoroutine(PostScore(name, score));
    }

    private IEnumerator PostScore(string name, int score) {
        // สร้าง JSON
        string json = "{\"name\":\"" + name + "\", \"score\":" + score + "}";
        
        // ยิงไปที่ Server ของคุณ (ถ้าทดสอบในเครื่องใช้ localhost)
        var request = new UnityWebRequest("http://localhost:3000/saveScore", "POST");
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