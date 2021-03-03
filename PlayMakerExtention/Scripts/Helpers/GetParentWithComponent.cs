using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets the Parent of a Game Object.")]
	public class GetParentWithComponent : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        [RequiredField]
        [Tooltip("Store the component in an Object variable.\nNOTE: Set theObject variable's Object Type to get a component of that type. E.g., set Object Type to UnityEngine.AudioListener to get the AudioListener component on the camera.")]
        public FsmObject componentType;

        [UIHint(UIHint.Variable)]
		public FsmGameObject storeResult;

		public override void Reset()
		{
			gameObject = null;
			storeResult = null;
            componentType = null;
        }

		public override void OnEnter()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go != null)
				storeResult.Value = go.GetComponentInParent(componentType.ObjectType).gameObject == null ? null : go.GetComponentInParent(componentType.ObjectType).gameObject;
			else
				storeResult.Value = null;
			
			Finish();
		}
	}
}