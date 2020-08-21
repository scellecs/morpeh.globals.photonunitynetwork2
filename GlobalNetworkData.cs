namespace Morpeh.Globals.PhotonUnityNetwork2 {
    using System;
    using Photon.Realtime;

    [Serializable]
    public class SerializableRaiseEventOptions : RaiseEventOptions {
    }

    [Serializable]
    public enum SerializableSendOptions {
        SendReliable   = 0,
        SendUnreliable = 1
    }
}