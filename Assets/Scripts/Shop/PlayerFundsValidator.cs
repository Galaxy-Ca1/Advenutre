public class PlayerFundsValidator : ShopValidatorBehaviour
{
    public override bool Validate(ShopItemSO item, out string failReason)
    {
        if (Player.Instance.CanSpendGoldCoins(item.price))
        {
            failReason = null;
            return true;
        }

        failReason = $"Need {item.price} coins, you have {Player.Instance.CurrentGoldCoins}.";
        return false;
    }
}
