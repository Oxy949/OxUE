using System;
using System.Collections;
using OxUE;
// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.
using UnityEngine;
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("LoadingUI")]
	[Tooltip("Show loading screen")]
	public class LoadingUIShow : FsmStateAction
    {
        public FsmFloat time;

        [Tooltip("BG Color")]
        public FsmColor bgColor;

        public FsmString title;

        [Tooltip("On finished")]
        public FsmEvent onFinished;

        public override void Reset()
		{
            title = null;
            bgColor = Color.black;
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
            LoadingUI.Instance.Show(bgColor.Value, null, title.Value, time.Value);

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
            if (visible == true)
            {
                LoadingUI.Instance.StateChanged -= OnStateChanged;
                target = onFinished;
            }
        }
    }
}
