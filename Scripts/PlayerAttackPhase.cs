using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackPhase : MonoBehaviour
{
    public int Checkint = 0;
    public float ComboCountDown;
    public int AttackAnimNum = 1;
    public bool IsPerformingAttack = false;
    public Animator PlayerAnim;

    private void Update()
    {
        if(ComboCountDown > 0)
        {
            ComboCountDown -= Time.deltaTime * 0.4f;
        }
        else { ComboCountDown = 0; }
    }
    public void AttackPhase1()
    {
        Checkint = 1;
    }
    public void AttackPhase0()
    {
        Checkint = 0;
    }

    public void PerformingAttack1()
    {
        IsPerformingAttack = true;
    }
    public void PerformingAttack0()
    {
        //Has performed Attack(AttackFinished)
        ComboCountDown += 1;
        IsPerformingAttack = false;
        if(PlayerAnim.GetBool("IsAttack5") == true)
        {
            ComboCountDown = 0;
        }
        PlayerAnim.SetBool("IsAttack1", false);
        PlayerAnim.SetBool("IsAttack2", false);
        PlayerAnim.SetBool("IsAttack3", false);
        PlayerAnim.SetBool("IsAttack4", false);
        PlayerAnim.SetBool("IsAttack5", false);
        PlayerAnim.SetBool("IsIdle", true);

    }
}
