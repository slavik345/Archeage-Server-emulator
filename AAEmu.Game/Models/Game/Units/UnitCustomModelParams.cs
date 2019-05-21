﻿using System.ComponentModel;
using AAEmu.Commons.Network;

namespace AAEmu.Game.Models.Game.Units
{
    public enum UnitCustomModelType
    {
        None = 0,
        Hair = 1,
        Skin = 2,
        Face = 3
    }

    public class FixedDecalAsset : PacketMarshaler
    {
        public uint AssetId { get; set; }
        public float AssetWeight { get; set; }

        public FixedDecalAsset(uint assetId = 0, float assetWeight = 0)
        {
            AssetId = assetId;
            AssetWeight = assetWeight;
        }

        public override void Read(PacketStream stream)
        {
            AssetId = stream.ReadUInt32();
            AssetWeight = stream.ReadSingle();
        }

        public override PacketStream Write(PacketStream stream)
        {
            stream.Write(AssetId);
            stream.Write(AssetWeight);
            return stream;
        }
    }

    public class FaceModel : PacketMarshaler
    {
        public uint MovableDecalAssetId { get; set; }
        public float MovableDecalWeight { get; set; }
        public float MovableDecalScale { get; set; }
        public float MovableDecalRotate { get; set; }
        public short MovableDecalMoveX { get; set; }
        public short MovableDecalMoveY { get; set; }

        public FixedDecalAsset[] FixedDecalAsset { get; }

        public uint DiffuseMapId { get; set; }
        public uint NormalMapId { get; set; }
        public uint EyelashMapId { get; set; }
        public float NormalMapWeight { get; set; }
        public uint LipColor { get; set; }
        public uint LeftPupilColor { get; set; }
        public uint RightPupilColor { get; set; }
        public uint EyebrowColor { get; set; }
        public uint DecoColor { get; set; }

        public byte[] Modifier { get; set; }

        public FaceModel()
        {
            //FixedDecalAsset = new FixedDecalAsset[4]; // 1.2
            FixedDecalAsset = new FixedDecalAsset[6];   // 3.0.3.0
            for (var i = 0; i < FixedDecalAsset.Length; i++)
            {
                FixedDecalAsset[i] = new FixedDecalAsset();
            }

            Modifier = new byte[128];
        }

        public bool SetFixedDecalAsset(byte index, uint id, float weight)
        {
            if (FixedDecalAsset.Length <= index)
            {
                return false;
            }

            FixedDecalAsset[index].AssetId = id;
            FixedDecalAsset[index].AssetWeight = weight;

            return true;
        }

        public override void Read(PacketStream stream)
        {
            MovableDecalAssetId = stream.ReadUInt32(); // type
            MovableDecalWeight = stream.ReadSingle();  // weight
            MovableDecalScale = stream.ReadSingle();   // scale
            MovableDecalRotate = stream.ReadSingle();  // rotate
            MovableDecalMoveX = stream.ReadInt16();    // moveX
            MovableDecalMoveY = stream.ReadInt16();    // moveY


            // почему-то нет такого в 3+
            //цикл 4 раза
            //foreach (var asset in FixedDecalAsset)
            //    asset.Read(stream);
            //---------------------------------------
            // for 3.0.3.0
            // --- begin pish
            //var hcount = 6;
            //do
            //{
            //    var pcount = 4;
            //    if (hcount < 4) { pcount = hcount; }
            //    m_assets = stream.ReadPisc(pcount);
            //    hcount -= pcount;
            //} while (hcount > 0);
            // --- end pish
            
            // замена вышенаписанному коду
            // --- begin pish
            var mAssets = stream.ReadPisc(4);
            // --- end pish
            FixedDecalAsset[0].AssetId = (uint)mAssets[0];
            FixedDecalAsset[1].AssetId = (uint)mAssets[1];
            FixedDecalAsset[2].AssetId = (uint)mAssets[2];
            FixedDecalAsset[3].AssetId = (uint)mAssets[3];

            // --- begin pish
            mAssets = stream.ReadPisc(2);
            // --- end pish
            FixedDecalAsset[4].AssetId = (uint)mAssets[0];
            FixedDecalAsset[5].AssetId = (uint)mAssets[1];

            // --- begin pish
            var mMap = stream.ReadPisc(3);
            // --- end pish

            // for 3.0.3.0
            for (var i = 0; i < 6; i++)
            {
                FixedDecalAsset[i].AssetWeight = stream.ReadSingle(); // weight
            }

            // почему-то нет такого в 3+
            //DiffuseMapId = stream.ReadUInt32();
            //NormalMapId = stream.ReadUInt32();
            //EyelashMapId = stream.ReadUInt32();
            DiffuseMapId = (uint)mMap[0];
            NormalMapId = (uint)mMap[1];
            EyelashMapId = (uint)mMap[2];

            NormalMapWeight = stream.ReadSingle();
            LipColor = stream.ReadUInt32();           // lip
            LeftPupilColor = stream.ReadUInt32();     // leftPupil
            RightPupilColor = stream.ReadUInt32();    // rightPupil
            EyebrowColor = stream.ReadUInt32();       // eyebrow
            DecoColor = stream.ReadUInt32();          // deco

            Modifier = stream.ReadBytes();
        }

        public override PacketStream Write(PacketStream stream)
        {
            stream.Write(MovableDecalAssetId);
            stream.Write(MovableDecalWeight);
            stream.Write(MovableDecalScale);
            stream.Write(MovableDecalRotate);
            stream.Write(MovableDecalMoveX);
            stream.Write(MovableDecalMoveY);

            //for 1.2
            //foreach (var asset in FixedDecalAsset)
            //    stream.Write(asset);

            //for 3.0.3.0
            stream.WritePisc(FixedDecalAsset[0].AssetId, FixedDecalAsset[1].AssetId, FixedDecalAsset[2].AssetId, FixedDecalAsset[3].AssetId);
            stream.WritePisc(FixedDecalAsset[4].AssetId, FixedDecalAsset[5].AssetId);
            stream.WritePisc(DiffuseMapId, NormalMapId, EyelashMapId);
            for (var i = 0; i < 6; i++)
            {
                stream.Write(FixedDecalAsset[i].AssetWeight); // weight
            }
            //stream.Write(DiffuseMapId);
            //stream.Write(NormalMapId);
            //stream.Write(EyelashMapId);
            stream.Write(NormalMapWeight);
            stream.Write(LipColor);
            stream.Write(LeftPupilColor);
            stream.Write(RightPupilColor);
            stream.Write(EyebrowColor);
            stream.Write(DecoColor);

            stream.Write(Modifier, true);
            return stream;
        }
    }

    public class UnitCustomModelParams : PacketMarshaler
    {
        private UnitCustomModelType _type;

        public uint HairColorId { get; private set; }
        public uint SkinColorId { get; private set; }
        //public uint UnkId { get; private set; } // TODO ...
        public float UnkId { get; private set; } // TODO ...

        public FaceModel Face { get; private set; }

        public UnitCustomModelParams(UnitCustomModelType type = UnitCustomModelType.None)
        {
            SetType(type);
        }

        public UnitCustomModelParams SetType(UnitCustomModelType type)
        {
            _type = type;
            if (_type == UnitCustomModelType.Face)
                Face = new FaceModel();
            return this;
        }

        public UnitCustomModelParams SetHairColorId(uint hairColorId)
        {
            HairColorId = hairColorId;
            return this;
        }

        public UnitCustomModelParams SetSkinColorId(uint skinColorId)
        {
            SkinColorId = skinColorId;
            return this;
        }

        public UnitCustomModelParams SetFace(FaceModel face)
        {
            Face = face;
            return this;
        }

        public override void Read(PacketStream stream)
        {
            SetType((UnitCustomModelType) stream.ReadByte()); // ext

            if (_type == UnitCustomModelType.None)
                return;

            // Hair
            HairColorId = stream.ReadUInt32(); // HairColorId

            stream.ReadUInt32(); // type for 3.0.3.0
            stream.ReadUInt32();  // defaultHairColor for 3.0.3.0
            stream.ReadUInt32(); // twoToneHair for 3.0.3.0
            stream.ReadSingle(); // twoToneFirstWidth for 3.0.3.0
            stream.ReadSingle(); // twoToneSecondWidth for 3.0.3.0

            if (_type == UnitCustomModelType.Hair)
                return;

            SkinColorId = stream.ReadUInt32();
            stream.ReadUInt32(); // type for 3.0.3.0
            stream.ReadUInt32(); // type for 3.0.3.0
            // Skin
            //UnkId = stream.ReadUInt32();
            UnkId = stream.ReadSingle(); // weight

            if (_type == UnitCustomModelType.Skin)
                return;

            // Face
            Face.Read(stream);
        }

        public override PacketStream Write(PacketStream stream)
        {
            stream.Write((byte) _type); // ext

            if (_type == UnitCustomModelType.None)
                return stream;

            stream.Write(HairColorId);

            // for 3.0.3.0
            stream.Write(0);  // type for 3.0.3.0
            stream.Write(0);  // defaultHairColor for 3.0.3.0
            stream.Write(0);  // twoToneHair for 3.0.3.0
            stream.Write(0f); // twoToneFirstWidth for 3.0.3.0
            stream.Write(0f); // twoToneSecondWidth for 3.0.3.0

            if (_type == UnitCustomModelType.Hair)
                return stream;

            stream.Write(SkinColorId);
            stream.Write(0);  // type for 3.0.3.0
            stream.Write(0);  // type for 3.0.3.0
            stream.Write(UnkId); // weight

            if (_type == UnitCustomModelType.Skin)
                return stream;

            stream.Write(Face);

            return stream;
        }
    }
}
