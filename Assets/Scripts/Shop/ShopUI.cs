using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Shop shop;
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private GameObject shopItemButtonPrefab;
    [SerializeField] private TMP_Text feedbackText;

    private void Start()
    {
        if (shop == null)
        {
            shop = GetComponentInParent<Shop>();
        }

        if (shop == null)
        {
            Debug.LogError("[ShopUI] Shop reference is missing.");
            enabled = false;
            return;
        }

        shop.OnPurchaseSuccess += HandleSuccess;
        shop.OnPurchaseFailed += HandleFailed;

        BuildUI();
    }

    private void OnDestroy()
    {
        if (shop == null)
        {
            return;
        }

        shop.OnPurchaseSuccess -= HandleSuccess;
        shop.OnPurchaseFailed -= HandleFailed;
    }

    public void BuildUI()
    {
        if (itemsContainer == null || shopItemButtonPrefab == null)
        {
            Debug.LogError("[ShopUI] Items container or item button prefab is missing.");
            return;
        }

        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }

        IReadOnlyList<ShopItemSO> items = shop.GetShopItems();
        if (items == null)
        {
            return;
        }

        foreach (ShopItemSO item in items)
        {
            if (item != null)
            {
                CreateItemButton(item);
            }
        }
    }

    private void CreateItemButton(ShopItemSO item)
    {
        GameObject buttonObject = Instantiate(shopItemButtonPrefab, itemsContainer);

        SetChildText(buttonObject, "ItemName", item.itemName);
        SetChildText(buttonObject, "Description", item.itemDescription);
        SetChildText(buttonObject, "Price", $"{item.price}");
        SetChildText(buttonObject, "Amount", $"x{item.amountPerPurchase}");

        Image icon = buttonObject.transform.Find("Icon")?.GetComponent<Image>();
        if (icon != null)
        {
            icon.sprite = item.itemIcon;
            icon.enabled = item.itemIcon != null;
        }

        Button button = buttonObject.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => shop.TryPurchase(item));
        }
    }

    private void HandleSuccess(ShopItemSO item)
    {
        ShowFeedback($"Bought: {item.itemName}", Color.green);
    }

    private void HandleFailed(ShopItemSO item, string reason)
    {
        ShowFeedback(reason, Color.red);
    }

    private void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null)
        {
            return;
        }

        feedbackText.text = message;
        feedbackText.color = color;
    }

    private static void SetChildText(GameObject parent, string childName, string text)
    {
        Transform child = parent.transform.Find(childName);
        if (child == null)
        {
            return;
        }

        TMP_Text label = child.GetComponent<TMP_Text>();
        if (label != null)
        {
            label.text = text;
        }
    }
}
