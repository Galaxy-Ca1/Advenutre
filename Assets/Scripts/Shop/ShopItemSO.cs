using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Shop Item", fileName = "NewShopItem")]
public class ShopItemSO : ScriptableObject
{
    [Header("Display")]
    public string itemName;
    [TextArea] public string itemDescription;
    public Sprite itemIcon;

    [Header("Price and Amount")]
    [Min(0)] public int price;
    [Min(1)] public int amountPerPurchase = 1;

    [Header("Reward")]
    public ShopRewardType rewardType;
    public WeaponSO weaponSO;

    [Header("Purchase Rules")]
    public bool allowPurchaseWhenHealthFull;

    public bool IsHealingItem => rewardType == ShopRewardType.Health || rewardType == ShopRewardType.Potion;
}
