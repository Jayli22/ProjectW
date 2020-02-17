using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttackCombo3 : StateMachineBehaviour
{
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.MyInstance.StatusSwitch(Player.CurrentState.BaseAttack);

        if (stateInfo.normalizedTime > Player.MyInstance.baseAttackPreCastTime[2] && Player.MyInstance.comboEffectMark == false)
        {
            Player.MyInstance.InstantiateEffectRotate(Player.MyInstance.attackEffect[2], Player.MyInstance.mouseAngle);
            Player.MyInstance.comboEffectMark = true;
        }
        //if (Player.MyInstance.comboMark)
        //{
        //    animator.SetBool("Attack", false);

        //    Player.MyInstance.clickCount = 2;


        //    //Player.MyInstance.comboMark = false;

        //}
        //else
        //{
        //    animator.SetBool("Attack", false);

        //    Player.MyInstance.clickCount = 0;

        //}
        //Player.MyInstance.comboEffectMark = true;

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if(Player.MyInstance.comboMark)
        //  {
        //      Player.MyInstance.noOfClicks++;
        //      Player.MyInstance.comboMark = false;

        //  }
        Player.MyInstance.comboEffectMark = false;
        if (Player.MyInstance.clickCount >= 3)
        {
            animator.SetInteger("AttackCMD", 1);
        }
        Player.MyInstance.StatusSwitch(Player.CurrentState.Normal);

    }
}
