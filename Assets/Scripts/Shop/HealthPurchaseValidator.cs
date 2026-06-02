public class HealthPurchaseValidator : ShopValidatorBehaviour
{
    public override bool Validate(ShopItemSO item, out string failReason)
    {
        if (!item.IsHealingItem || item.allowPurchaseWhenHealthFull || !Player.Instance.IsHealthFull())
        {
            failReason = null;
            return true;
        }

        failReason = "Health is already full.";
        return false;
    }
}
