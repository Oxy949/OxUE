using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    public class DestroyFSM : FsmStateAction
    {
        [RequiredField]
        public FsmOwnerDefault gameObject;

        public FsmString name;

        public override void Reset()
        {
            gameObject = null;
            name = new FsmString { UseVariable = true };
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            
            PlayMakerFSM[] fsms = go.GetComponents<PlayMakerFSM>();
                
            foreach (PlayMakerFSM fsm in fsms)
            {
                if (fsm.FsmName == name.Value)
                {
                    Object.Destroy(fsm);
                }
            }

            Finish();
        }
    }
}