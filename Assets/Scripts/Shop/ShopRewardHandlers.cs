using UnityEngine;

public class HealthRewardHandler : ShopRewardHandlerBehaviour
{
    public override ShopRewardType RewardType => ShopRewardType.Health;

    public override void Grant(ShopItemSO item)
    {
        Player.Instance.HealPlayer(item.amountPerPurchase);
    }
}

public class PotionRewardHandler : ShopRewardHandlerBehaviour
{
    public override ShopRewardType RewardType => ShopRewardType.Potion;

    public override void Grant(ShopItemSO item)
    {
        Player.Instance.HealPlayer(item.amountPerPurchase);
    }
}

public class CoinsRewardHandler : ShopRewardHandlerBehaviour
{
    public override ShopRewardType RewardType => ShopRewardType.Coins;

    public override void Grant(ShopItemSO item)
    {
        Player.Instance.AddGoldCoin(item.amountPerPurchase);
    }
}

public class WeaponRewardHandler : ShopRewardHandlerBehaviour
{
    public override ShopRewardType RewardType => ShopRewardType.Weapon;

    public override void Grant(ShopItemSO item)
    {
        if (item.weaponSO == null)
        {
            Debug.LogWarning($"[Shop] Item '{item.itemName}' has no WeaponSO assigned.");
            return;
        }

        ActiveWeapon.Instance.SetCurrentWeaponSODirect(item.weaponSO);
    }
}
