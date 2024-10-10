using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnteringFaseState : StateMachineBehaviour
{
    public enum TriggerOn { EXIT, ENTER }

    public TriggerOn triggerOn = TriggerOn.EXIT;
    public int fase = 1;

    SniperController sniperController;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        sniperController = animator.GetComponent<SniperController>();

        if (triggerOn == TriggerOn.ENTER)
            sniperController.SetarFase(fase);
    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (triggerOn == TriggerOn.EXIT)
            sniperController.SetarFase(fase);
    }
}
