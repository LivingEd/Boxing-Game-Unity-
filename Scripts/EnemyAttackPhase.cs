using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackPhase : MonoBehaviour
{
    public int Checkint = 0;

    public void AttackPhase1()
    {
        Checkint = 1;
    }
    public void AttackPhase0()
    {
        Checkint = 0;
    }
}
