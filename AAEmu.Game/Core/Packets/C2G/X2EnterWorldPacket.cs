﻿using AAEmu.Commons.Network;
using AAEmu.Game.Core.Managers.World;
using AAEmu.Game.Core.Network.Game;
using AAEmu.Game.Core.Packets.G2C;

namespace AAEmu.Game.Core.Packets.C2G
{
    public class X2EnterWorldPacket : GamePacket
    {
        public X2EnterWorldPacket() : base(CSOffsets.X2EnterWorldPacket, 1)
        {

        }

        public override void Read(PacketStream stream)
        {
            var pFrom = stream.ReadUInt32();
            var pTo = stream.ReadUInt32();
            var accountId = stream.ReadUInt64();
            var cookie = stream.ReadUInt32();
            var zoneId = stream.ReadInt32();
            var tb = stream.ReadInt16();
            var revision = stream.ReadUInt32();
            var index = stream.ReadUInt32();

            EnterWorldManager.Instance.Login(Connection, accountId, cookie);
        }
    }
}