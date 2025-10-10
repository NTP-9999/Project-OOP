using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class InteractableUI : MonoBehaviour
{
    [SerializeField] private Transform uiPosition;
    [SerializeField] private string interactableText = "E";
    [SerializeField] private Vector3 canvasScale = new Vector3(0.007f, 0.007f, 0.007f);
    [SerializeField] private Vector3 maxJellyScale = new Vector3(0.01f, 0.01f, 0.01f);
    private Canvas canvasUI;
    private Canvas currentCanvas;
    private void Start()
    {
        if (canvasUI != null)
            canvasUI.gameObject.SetActive(false);

        canvasUI = Resources.Load<Canvas>("UI/InteractableUI");
    }

    private void CreateUI()
    {
        if (canvasUI != null && uiPosition != null)
        {
            currentCanvas = Instantiate(canvasUI, uiPosition.position, Quaternion.identity);
            currentCanvas.worldCamera = Camera.main;
            TMP_Text InteractableText = currentCanvas.transform.Find("InteractableText").GetComponent<TMP_Text>();
            InteractableText.text = interactableText;
            currentCanvas.transform.SetParent(uiPosition);
            currentCanvas.transform.position = uiPosition.transform.position;
            currentCanvas.transform.localScale = canvasScale;

            currentCanvas.gameObject.AddComponent<LookAtCamera>();
            JellyScale jellyScale = currentCanvas.gameObject.AddComponent<JellyScale>();
            jellyScale.targetScale = maxJellyScale;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            if (canvasUI != null && uiPosition != null) CreateUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            Destroy(currentCanvas.gameObject);
        }
    }
}
