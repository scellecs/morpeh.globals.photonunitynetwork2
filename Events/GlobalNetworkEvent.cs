namespace Morpeh.Globals {
    using System.Collections.Generic;
    using ExitGames.Client.Photon;
    using Photon.Pun;
    using Photon.Realtime;
    using PhotonUnityNetwork2;
    using Unity.IL2CPP.CompilerServices;
    using UnityEngine;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Globals/Events/Network/Event")]
    public class GlobalNetworkEvent : GlobalEvent, IOnEventCallback {
        public int eventCode;

        public SerializableRaiseEventOptions raiseEventOptions;
        public SerializableSendOptions       sendOptions;

        protected override bool CheckIsInitialized() {
            var check = base.CheckIsInitialized();
            if (check) {
                PhotonNetwork.AddCallbackTarget(this);
            }
            return check;
        }

        public override void Dispose() {
            base.Dispose();
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public override void Publish() => this.Publish(-1);
        public override void NextFrame() => this.NextFrame(-1);

        public override void Publish(int data) {
            this.CheckIsInitialized();

            var serializedData = this.Serialize(data);

            var customData = new object[] {
                this.eventCode,
                serializedData
            };

            PhotonNetwork.RaiseEvent(199, customData, this.raiseEventOptions,
                this.sendOptions == SerializableSendOptions.SendReliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);

            //Photon don't send event on client itself, some workaround
            if (this.raiseEventOptions.Receivers == ReceiverGroup.All) {
                base.NextFrame(data);
            }
        }

        public override void NextFrame(int data) => this.Publish(data);

        public void OnEvent(EventData photonEvent) {
            if (photonEvent.Code != 199) {
                return;
            }
            var customData = (object[]) photonEvent.CustomData;
            var code       = (int) customData[0];

            if (code != this.eventCode) {
                return;
            }

            base.NextFrame(this.Deserialize((string) customData[1]));
        }
    }

}