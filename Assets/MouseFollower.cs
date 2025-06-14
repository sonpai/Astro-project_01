////using System.Collections;
////using System.Collections.Generic;
////using UnityEngine;


////public class MouseFollower : MonoBehaviour
////{
////    [SerializeField]
////    private Canvas canvas;

////    [SerializeField]
////    private Camera mainCam;


////    [SerializeField]
////    private UIInventoryItem item;

////    public void Awake()
////    {
////        canvas = transform.root.GetComponent<Canvas>();
////        mainCam = Camera.main;
////        item = GetComponentInChildren<UIInventoryItem>();
////    }

////    public void SetData(Sprite sprite, int quantity)
////    {
////        item.SetData(sprite, quantity);
////    }

////    void Update()
////    {
////        Vector2 position;
////        RectTransformUtility.ScreenPointToLocalPointInRectangle(
////            (RectTransform)canvas.transform,
////            Input.mousePosition,
////            canvas.worldCamera,
////            out position
////                );
////        transform.position = canvas.transform.TransformPoint(position);
////    }

////    public void Toggle(bool val)
////    {
////        Debug.Log($"Item toggled {val}");
////        gameObject.SetActive(val);
////    }

////}

//// MouseFollower.cs (Revised for simplicity)
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class MouseFollower : MonoBehaviour
//{
//    [SerializeField] private Canvas canvas;
//    // [SerializeField] private Camera mainCam; // Optional, canvas.worldCamera is often sufficient

//    [Header("UI Elements for Follower")]
//    [SerializeField] private Image itemImage;
//    [SerializeField] private TMP_Text quantityText;

//    void Awake()
//    {
//        if (canvas == null)
//        {
//            // Try to find the root canvas
//            Transform rootCanvasTransform = transform.root;
//            canvas = rootCanvasTransform.GetComponent<Canvas>();
//            if (canvas == null) Debug.LogError("MouseFollower: Canvas not found on root or not assigned!");
//        }

//        // Ensure itemImage and quantityText are assigned in the inspector for the MouseFollower GameObject
//        if (itemImage == null) Debug.LogError("MouseFollower: itemImage UI component not assigned in Inspector!");
//        if (quantityText == null) Debug.LogError("MouseFollower: quantityText UI component not assigned in Inspector!");

//        Toggle(false); // Start hidden
//    }

//    public void SetData(Sprite sprite, int quantity)
//    {
//        if (itemImage != null)
//        {
//            bool hasItem = sprite != null;
//            itemImage.gameObject.SetActive(hasItem);
//            itemImage.sprite = sprite;
//        }

//        if (quantityText != null)
//        {
//            bool showQuantity = sprite != null && quantity > 1; // Show quantity if item exists and is > 1
//            quantityText.gameObject.SetActive(showQuantity);
//            if (showQuantity)
//            {
//                quantityText.text = quantity.ToString();
//            }
//            else
//            {
//                quantityText.text = "";
//            }
//        }
//    }

//    void Update()
//    {
//        if (!gameObject.activeSelf || canvas == null) return;

//        Vector2 position;
//        RectTransformUtility.ScreenPointToLocalPointInRectangle(
//            (RectTransform)canvas.transform,
//            Input.mousePosition,
//            canvas.worldCamera, // Use the camera associated with the canvas
//            out position
//        );
//        // For precise positioning of UI elements, set anchoredPosition
//        ((RectTransform)transform).anchoredPosition = position;
//    }

//    public void Toggle(bool val)
//    {
//        // Debug.Log($"MouseFollower toggled {val}");
//        gameObject.SetActive(val);
//        if (!val) // If hiding, clear its visual data
//        {
//            if (itemImage != null) itemImage.sprite = null;
//            if (quantityText != null) quantityText.text = "";
//        }
//    }
//}

// MouseFollower.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MouseFollower : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private TMP_Text quantityText;

    private RectTransform rectTransform;

    private void Awake()
    {
        // Get components on this object
        rectTransform = GetComponent<RectTransform>();

        // Find the root canvas automatically
        if (canvas == null)
        {
            canvas = transform.root.GetComponent<Canvas>();
        }

        // Start hidden
        Toggle(false);
    }

    public void SetData(Sprite sprite, int quantity)
    {
        // Set the visual data for the follower
        itemImage.sprite = sprite;
        if (quantity > 1)
        {
            quantityText.text = quantity.ToString();
            quantityText.gameObject.SetActive(true);
        }
        else
        {
            quantityText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Follow the mouse position
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            Input.mousePosition,
            canvas.worldCamera,
            out position
        );
        rectTransform.position = canvas.transform.TransformPoint(position);
    }

    public void Toggle(bool val)
    {
        // Show or hide the entire mouse follower object
        gameObject.SetActive(val);
    }
}
