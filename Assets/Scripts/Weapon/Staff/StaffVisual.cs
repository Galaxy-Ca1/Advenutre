using UnityEngine;

public class StaffVisual : MonoBehaviour
{
    [SerializeField] private Staff staff;

    private const string ATTACK = "Attack";

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (staff == null)
        {
            staff = GetComponentInParent<Staff>();
        }
    }

    private void OnEnable()
    {
        if (staff != null)
        {
            staff.OnStaffAttack += Staff_OnStaffAttack;
        }
    }

    private void OnDisable()
    {
        if (staff != null)
        {
            staff.OnStaffAttack -= Staff_OnStaffAttack;
        }
    }

    private void Staff_OnStaffAttack(object sender, System.EventArgs e)
    {
        if (animator != null)
        {
            animator.SetTrigger(ATTACK);
        }
    }
}
