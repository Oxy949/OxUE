using System;
using System.Collections;
using OxUE;
// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.
using UnityEngine;
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("LoadingUI")]
    [Tooltip("Hide loading screen")]
    public class LoadingUIHide : FsmStateAction
    {
        public FsmFloat time;

        [Tooltip("On finished")]
        public FsmEvent onFinished;

        public override void Reset()
        {
            onFinished = null;
            time = 1f;
        }

        public override void OnEnter()
        {
            target = null;
            if (onFinished != null)
            {
                LoadingUI.Instance.StateChanged -= OnStateChanged;
                LoadingUI.Instance.StateChanged += OnStateChanged;
            }
            LoadingUI.Instance.Hide(time.Value);
            if (onFinished == null)
                Finish();
        }

        private FsmEvent target = null;
        public override void OnUpdate()
        {
            if (target != null)
            {
                Finish();
                Fsm.Event(target);
            }
        }

        void OnStateChanged(bool visible)
        {
            if (visible == false)
            {
                LoadingUI.Instance.StateChanged -= OnStateChanged;
                target = onFinished;
            }
        }
    }
}
