using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationModelSwap : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Anim exit");
        animator.gameObject.GetComponent<Reference>().m_reference.GetComponent<Problem>().ModelSwap(); // What the fuck is this shit seriously.
        //animator.gameObject.GetComponent<Reference>().m_reference.GetComponent<Problem>().SetActive(false);
    }
}
