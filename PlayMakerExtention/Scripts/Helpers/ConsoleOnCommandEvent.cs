// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Console")]
    [Tooltip("Console OnCommand Event")]
    public class ConsoleOnCommandEvent : FsmStateAction
    {
        [RequiredField]
        public FsmString command;

        [UIHint(UIHint.Variable)]
        public FsmString[] args;
        public FsmBool isCheat;

        [Tooltip("On Command")]
        public FsmEvent onCommand;

        public override void Reset()
        {
            command = new FsmString();
            args = new FsmString[0];
            isCheat = true;
            onCommand = null;
        }

        public override void OnEnter()
        {
            DeviceConsole.Instance.CommandEvent -= Instance_CommandEvent;
            DeviceConsole.Instance.CommandEvent += Instance_CommandEvent;
        }

        private void Instance_CommandEvent(object sender, DeviceConsole.OnCommandEventArgs e)
        {
            if (!isCheat.Value || (isCheat.Value))
            {
                if (command.Value.ToLower() == e.args[0].ToLower())
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        args[i].Value = e.args[i + 1];
                    }

                    Fsm.Event(onCommand);
                }
            }
            else
            {
                //GameManager.Instance.PlayerWarnCheats();
            }
        }
    }
}
