using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InteractableUI : MonoBehaviour
{
    [SerializeField] private Transform uiPosition;
    [SerializeField] private string interactableText = "E";
    [SerializeField] private Vector3 canvasScale = new Vector3(0.007f, 0.007f, 0.007f);
    [SerializeField] private Vector3 maxJellyScale = new Vector3(0.01f, 0.01f, 0.01f);
    [SerializeField] private bool isPlaceableStructure;
    private Canvas canvasUI;
    private Canvas currentCanvas;
    private GameObject requireItemUI;
    private GameObject repairRequireItemCanvas;
    private GameObject RepairRequireItemCanvas;
    private Weapon weapon;
    private void Start()
    {
        if (canvasUI != null)
            canvasUI.gameObject.SetActive(false);

        canvasUI = Resources.Load<Canvas>("UI/InteractableUI");
        requireItemUI = Resources.Load<GameObject>("UI/RequireItemUI");
        repairRequireItemCanvas = Resources.Load<GameObject>("UI/RepairRequireItemCanvas");
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

            if (isPlaceableStructure)
            {
                weapon = GetComponent<Weapon>();

                RepairRequireItemCanvas = Instantiate(repairRequireItemCanvas, uiPosition.position, Quaternion.identity);
                RepairRequireItemCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
                RepairRequireItemCanvas.transform.position -= new Vector3(0f, 5f, 0f);
                Transform requireItemList = RepairRequireItemCanvas.transform.Find("List");
                
                foreach (var requireItem in weapon.RepairData.RequireItems)
                {
                    GameObject RequireItemUI = Instantiate(requireItemUI, requireItemList);
                    RequireItemUI.GetComponent<Image>().sprite = requireItem.Item.Icon;
                    RequireItemUI.GetComponentInChildren<TMP_Text>().text = $"{requireItem.Amount}";
                }
            }
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
            Destroy(RepairRequireItemCanvas);
        }
    }
}
