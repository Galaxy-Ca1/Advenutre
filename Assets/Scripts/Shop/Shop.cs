using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Shop : MonoBehaviour
{
    [Header("Items")]
    [SerializeField] private ShopItemSO[] shopItems;

    [Header("Components")]
    [Tooltip("If empty, validators are found on this object and its children.")]
    [SerializeField] private ShopValidatorBehaviour[] validators;

    [Tooltip("If empty, reward handlers are found on this object and its children.")]
    [SerializeField] private ShopRewardHandlerBehaviour[] rewardHandlers;

    public event Action<ShopItemSO> OnPurchaseSuccess;
    public event Action<ShopItemSO, string> OnPurchaseFailed;

    private readonly Dictionary<ShopRewardType, ShopRewardHandlerBehaviour> handlersByType = new Dictionary<ShopRewardType, ShopRewardHandlerBehaviour>();

    private void Awake()
    {
        EnsureDefaultComponents();
        CacheValidatorsAndHandlers();
    }

    public IReadOnlyList<ShopItemSO> GetShopItems()
    {
        return shopItems;
    }

    public bool TryPurchase(ShopItemSO item)
    {
        if (item == null)
        {
            Debug.LogError("[Shop] TryPurchase failed: item is null.");
            return false;
        }

        foreach (ShopValidatorBehaviour validator in validators)
        {
            if (validator == null)
            {
                continue;
            }

            if (!validator.Validate(item, out string reason))
            {
                OnPurchaseFailed?.Invoke(item, reason);
                return false;
            }
        }

        if (!Player.Instance.TrySpendGoldCoins(item.price))
        {
            string reason = $"Need {item.price} coins, you have {Player.Instance.CurrentGoldCoins}.";
            OnPurchaseFailed?.Invoke(item, reason);
            return false;
        }

        if (handlersByType.TryGetValue(item.rewardType, out ShopRewardHandlerBehaviour handler))
        {
            handler.Grant(item);
        }
        else
        {
            Debug.LogWarning($"[Shop] No reward handler for {item.rewardType}.");
        }

        OnPurchaseSuccess?.Invoke(item);
        return true;
    }

    private void EnsureDefaultComponents()
    {
        if (GetComponentInChildren<PlayerFundsValidator>(true) == null)
        {
            gameObject.AddComponent<PlayerFundsValidator>();
        }

        if (GetComponentInChildren<HealthPurchaseValidator>(true) == null)
        {
            gameObject.AddComponent<HealthPurchaseValidator>();
        }

        if (GetComponentInChildren<HealthRewardHandler>(true) == null)
        {
            gameObject.AddComponent<HealthRewardHandler>();
        }

        if (GetComponentInChildren<PotionRewardHandler>(true) == null)
        {
            gameObject.AddComponent<PotionRewardHandler>();
        }

        if (GetComponentInChildren<CoinsRewardHandler>(true) == null)
        {
            gameObject.AddComponent<CoinsRewardHandler>();
        }

        if (GetComponentInChildren<WeaponRewardHandler>(true) == null)
        {
            gameObject.AddComponent<WeaponRewardHandler>();
        }
    }

    private void CacheValidatorsAndHandlers()
    {
        if (validators == null || validators.Length == 0)
        {
            validators = GetComponentsInChildren<ShopValidatorBehaviour>(true);
        }

        if (rewardHandlers == null || rewardHandlers.Length == 0)
        {
            rewardHandlers = GetComponentsInChildren<ShopRewardHandlerBehaviour>(true);
        }

        handlersByType.Clear();
        foreach (ShopRewardHandlerBehaviour handler in rewardHandlers)
        {
            if (handler == null)
            {
                continue;
            }

            handlersByType[handler.RewardType] = handler;
        }
    }
}
