﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttackCombo2 : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        Player.MyInstance.comboEffectMark = true;

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Player.MyInstance.comboMark)
        {

            Player.MyInstance.clickCount = 3;


            //Player.MyInstance.comboMark = false;

        }
        else
        {

            Player.MyInstance.clickCount = 0;

        }
        Player.MyInstance.comboEffectMark = true;

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

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
        // animator.SetBool("Attack", false);
        // Obviously "noOfClicks" is not directly accessible here so you have to make it public and get it reference somehow to use it here  
        Player.MyInstance.comboEffectMark = false;

    }
}
