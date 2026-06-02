using UnityEngine;

public abstract class ShopValidatorBehaviour : MonoBehaviour
{
    public abstract bool Validate(ShopItemSO item, out string failReason);
}
