using UnityEngine;

public abstract class ShopRewardHandlerBehaviour : MonoBehaviour
{
    public abstract ShopRewardType RewardType { get; }

    public abstract void Grant(ShopItemSO item);
}
