using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoingAfk : StateMachineBehaviour
{
    [SerializeField] private float timeToAfk;

    private float timer;
    
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetFloat("PosX") == 0 && animator.GetFloat("PosY") == 0)
        {
            timer += Time.deltaTime;

            if (timer >= timeToAfk)
                animator.SetBool("Idle", true);
        }
        else
            timer = 0;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
    }
    
}
