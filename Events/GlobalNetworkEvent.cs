namespace Morpeh.Globals {
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
    public abstract class GlobalNetworkEvent : GlobalEvent, IOnEventCallback {
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

        public override void Publish(int data) {
            this.CheckIsInitialized();

            var serializedData = this.Serialize(data);

            var customData = new SerializableCustomData {
                eventCode = this.eventCode,
                data      = serializedData
            };

            PhotonNetwork.RaiseEvent(200, customData, this.raiseEventOptions,
                this.sendOptions == SerializableSendOptions.SendReliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
        }

        public override void NextFrame(int data) => this.Publish(data);

        public void OnEvent(EventData photonEvent) {
            if (photonEvent.Code != 200) {
                return;
            }

            var customData = (SerializableCustomData) photonEvent.CustomData;

            if (customData.eventCode != this.eventCode) {
                return;
            }

            base.NextFrame(this.Deserialize(customData.data));
        }
    }

}