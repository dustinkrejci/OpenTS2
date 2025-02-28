﻿using System;
using OpenTS2.Files.Utils;
using UnityEngine;

namespace OpenTS2.Files.Formats.DBPF.Types
{
    public struct FloatArray2D
    {
        private const uint TypeId = 0x6b943b43;
        private const string TypeName = "c2DArray";
        
        public readonly float[,] values;

        public FloatArray2D(float[,] values)
        {
            this.values = values;
        }

        public override string ToString()
        {
            return $"FloatArray2D({nameof(values)}: {values})";
        }

        public static FloatArray2D Deserialize(IoBuffer reader)
        {
            var id = reader.ReadUInt32();
            if (id != TypeId)
            {
                throw new ArgumentException($"FloatArray2D has wrong id {id:x}");
            }

            var version = reader.ReadUInt32();
            Debug.Assert(version == 1, "Wrong version for FloatArray2D");

            var name = reader.ReadUint32PrefixedString();
            Debug.Assert(name == TypeName, "Wrong type name for FloatArray2D");

            var width = reader.ReadInt32();
            var height = reader.ReadInt32();

            var values = new float[width, height];
            for (var i = 0; i < width * height; i++)
            {
                values[i % width, i / width] = reader.ReadFloat();
            }

            return new FloatArray2D(values);
        }
    }
}