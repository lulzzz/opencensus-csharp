﻿// <copyright file="TraceId.cs" company="OpenCensus Authors">
// Copyright 2018, OpenCensus Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of theLicense at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace OpenCensus.Trace
{
    using System;
    using OpenCensus.Trace.Internal;
    using OpenCensus.Utils;

    public sealed class TraceId : ITraceId
    {
        public const int SIZE = 16;
        public static readonly TraceId INVALID = new TraceId(new byte[SIZE]);

        private readonly byte[] bytes;

        public byte[] Bytes
        {
            get
            {
                byte[] copyOf = new byte[SIZE];
                Buffer.BlockCopy(this.bytes, 0, copyOf, 0, SIZE);
                return copyOf;
            }
        }

        private TraceId(byte[] bytes)
        {
            this.bytes = bytes;
        }

        public static ITraceId FromBytes(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (buffer.Length != SIZE)
            {
                throw new ArgumentOutOfRangeException(string.Format("Invalid size: expected {0}, got {1}", SIZE, buffer.Length));
            }

            byte[] bytesCopied = new byte[SIZE];
            Buffer.BlockCopy(buffer, 0, bytesCopied, 0, SIZE);
            return new TraceId(bytesCopied);
        }

        public static ITraceId FromBytes(byte[] src, int srcOffset)
        {
            byte[] bytes = new byte[SIZE];
            Buffer.BlockCopy(src, srcOffset, bytes, 0, SIZE);
            return new TraceId(bytes);
        }

        public static ITraceId FromLowerBase16(string src)
        {
            if (src.Length != 2 * SIZE)
            {
                throw new ArgumentOutOfRangeException(string.Format("Invalid size: expected {0}, got {1}", 2 * SIZE, src.Length));
            }

            byte[] bytes = Arrays.StringToByteArray(src);
            return new TraceId(bytes);
        }

        public static ITraceId GenerateRandomId(IRandomGenerator random)
        {
            byte[] bytes = new byte[SIZE];
            do
            {
                random.NextBytes(bytes);
            }
            while (Arrays.Equals(bytes, INVALID.bytes));
            return new TraceId(bytes);
        }

        public void CopyBytesTo(byte[] dest, int destOffset)
        {
            Buffer.BlockCopy(this.bytes, 0, dest, destOffset, SIZE);
        }

        public bool IsValid
        {
            get
            {
                return !Arrays.Equals(this.bytes, INVALID.bytes);
            }
        }

        public string ToLowerBase16()
        {
            var bytes = this.Bytes;
            var result = Arrays.ByteArrayToString(bytes);
            return result;
        }

        public long LowerLong
        {
            get
            {
                long result = 0;
                for (int i = 0; i < 8; i++)
                {
                    result <<= 8;
#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
                    result |= this.bytes[i] & 0xff;
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand
                }

                if (result < 0)
                {
                    return -result;
                }

                return result;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            if (!(obj is TraceId))
            {
                return false;
            }

            TraceId that = (TraceId)obj;
            return Arrays.Equals(this.bytes, that.bytes);
        }

        public override int GetHashCode()
        {
            return Arrays.GetHashCode(this.bytes);
        }

        public override string ToString()
        {
            return "TraceId{"
               + "bytes=" + this.ToLowerBase16()
               + "}";
        }

        public int CompareTo(ITraceId other)
        {
            TraceId that = other as TraceId;
            for (int i = 0; i < SIZE; i++)
            {
                if (this.bytes[i] != that.bytes[i])
                {
                    sbyte b1 = (sbyte)this.bytes[i];
                    sbyte b2 = (sbyte)that.bytes[i];

                    return b1 < b2 ? -1 : 1;
                }
            }

            return 0;
        }
    }
}
