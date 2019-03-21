namespace Networking
{
    public enum NetMessageType
    {
        ConnectionInfo,
        UpdatePlayerPostionAndState,
        UpdatePlayerData,
        UpdatePropPostionAndState,
        BulletHit,
        WeaponUsed,
        WeaponPickUp,
        WeaponDrop,
        ChatMessage,
        LevelInfo,
        LevelChange,
    }

    [System.Serializable]
    public abstract class Message
    {
        public byte MessageType { set; get; }

        protected Message(byte messageType)
        {
            MessageType = messageType;
        }
    }

    [System.Serializable]
    public class ConnectionInfo : Message
    {
        public ConnectionInfo(byte messageType, string msg) : base(messageType)
        {
            MessageType = (byte)NetMessageType.ConnectionInfo;
            Msg = msg;
        }

        public string Msg { set; get; }
        public string PlayerName { set; get; }
    }

    [System.Serializable]
    public class UpdatePlayerPositionAndState : Message
    {
        public UpdatePlayerPositionAndState(byte messageType) : base(messageType)
        {
            MessageType = (byte)NetMessageType.UpdatePlayerPostionAndState;
            Pos = new float[3];
            Vel = new float[3];
            Rot = new float[3];
        }

        public byte PlayerID { set; get; }
        public float[] Pos { set; get; }
        public float[] Vel { set; get; }
        public float[] Rot { set; get; }
        public bool Grounded { set; get; }
        public bool Sliding { set; get; }
    }

    [System.Serializable]
    public class UpdatePlayerData : Message
    {
        public UpdatePlayerData(byte messageType) : base(messageType)
        {
            MessageType = (byte)NetMessageType.UpdatePlayerData;
        }

        public int PlayerHealth { set; get; }
    }

    [System.Serializable]
    public class UpdatePropPostionAndState : Message
    {
        public UpdatePropPostionAndState(byte messageType) : base(messageType)
        {
            MessageType = (byte)NetMessageType.UpdatePropPostionAndState;
        }


    }

    [System.Serializable]
    public class BulletHit : Message
    {
        public BulletHit(byte messageType) : base(messageType)
        {
            MessageType = (byte)NetMessageType.BulletHit;
        }


    }

    [System.Serializable]
    public class WeaponUsed : Message
    {
        public WeaponUsed(byte messageType) : base(messageType)
        {
            MessageType = (byte)NetMessageType.WeaponUsed;
        }


    }

    [System.Serializable]
    public class WeaponPickUp : Message
    {
        public WeaponPickUp(byte messageType) : base(messageType)
        {
            MessageType = (byte)NetMessageType.WeaponPickUp;
        }


    }

    [System.Serializable]
    public class WeaponDrop : Message
    {
        public WeaponDrop(byte messageType) : base(messageType)
        {
            MessageType = (byte)NetMessageType.WeaponDrop;
        }


    }

    [System.Serializable]
    public class ChatMessage : Message
    {
        public ChatMessage(byte messageType) : base(messageType)
        {
            MessageType = (byte)NetMessageType.ChatMessage;
        }

        public string Text { set; get; }
    }

    [System.Serializable]
    public class LevelInfo : Message
    {
        public LevelInfo(byte messageType) : base(messageType)
        {
            MessageType = (byte)NetMessageType.LevelInfo;
        }


    }

    [System.Serializable]
    public class LevelChange : Message
    {
        public LevelChange(byte messageType) : base(messageType)
        {
            MessageType = (byte)NetMessageType.LevelChange;
        }


    }
}