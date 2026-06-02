using UnityEngine;
using DP.Utils;

public class ActiveWeapon : Singleton<ActiveWeapon>
{
    [SerializeField] private WeaponSO[] weaponSOList;

    private WeaponSO currentWeaponSO;
    private bool isFlipped;

    private void Start()
    {
        isFlipped = false;

        if (weaponSOList == null || weaponSOList.Length == 0)
        {
            Debug.LogError("[ActiveWeapon] Weapon list is empty.");
            return;
        }

        SetCurrentWeaponSO(weaponSOList[0].weaponKeyboardKey);
    }

    protected override void Update()
    {
        base.Update();
        SetActiveWeaponRotation();
    }

    public WeaponSO GetCurrentWeaponSO()
    {
        return currentWeaponSO;
    }

    public void SetCurrentWeaponSO(int keyboardKey)
    {
        if (weaponSOList == null)
        {
            return;
        }

        for (int i = 0; i < weaponSOList.Length; i++)
        {
            if (weaponSOList[i] != null && weaponSOList[i].weaponKeyboardKey == keyboardKey)
            {
                ActivateWeapon(weaponSOList[i]);
                break;
            }
        }
    }

    public void SetCurrentWeaponSODirect(WeaponSO weaponSO)
    {
        if (weaponSO == null)
        {
            Debug.LogError("[ActiveWeapon] Cannot equip a null WeaponSO.");
            return;
        }

        ActivateWeapon(weaponSO);
    }

    private void MouseFollowRotation()
    {
        if (Camera.main == null || GameInput.Instance == null)
        {
            return;
        }

        Vector3 mousePos = GameInput.Instance.GetMousePosition();
        Vector2 direction = Camera.main.WorldToScreenPoint(transform.position) - mousePos;
        transform.right = -direction;
    }

    private void MouseFollowingDirection()
    {
        if (Camera.main == null || GameInput.Instance == null || Player.Instance == null)
        {
            return;
        }

        Vector3 mousePos = GameInput.Instance.GetMousePosition();
        Vector3 playerScreenPoint = Utils.GetGameObjectScreenPoint(Player.Instance.transform);

        if (mousePos.x < playerScreenPoint.x)
        {
            isFlipped = true;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            isFlipped = false;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void SetActiveWeaponRotation()
    {
        if (Player.Instance == null || !Player.Instance.IsAlive())
        {
            return;
        }

        if (currentWeaponSO != null && currentWeaponSO.weaponSpinsAround)
        {
            MouseFollowRotation();
        }
        else
        {
            MouseFollowingDirection();
        }
    }

    private void ActivateWeapon(WeaponSO weaponSO)
    {
        if (weaponSO.weaponPrefab == null)
        {
            Debug.LogError($"[ActiveWeapon] Weapon prefab is missing on {weaponSO.name}.");
            return;
        }

        if (currentWeaponSO != null && currentWeaponSO.weaponGameObject != null)
        {
            Destroy(currentWeaponSO.weaponGameObject.gameObject);
            currentWeaponSO.weaponGameObject = null;
        }

        currentWeaponSO = weaponSO;
        SetActiveWeaponRotation();

        BaseWeapon currentWeaponPrefab = currentWeaponSO.weaponPrefab;
        Vector3 prefabPosition = currentWeaponPrefab.transform.position;

        if (isFlipped)
        {
            prefabPosition = new Vector3(-prefabPosition.x, prefabPosition.y, prefabPosition.z);
        }

        BaseWeapon spawnedWeapon = Instantiate(
            currentWeaponPrefab,
            transform.position + prefabPosition,
            transform.rotation * currentWeaponPrefab.transform.rotation
        );

        spawnedWeapon.transform.SetParent(transform);
        currentWeaponSO.weaponGameObject = spawnedWeapon;
    }
}
