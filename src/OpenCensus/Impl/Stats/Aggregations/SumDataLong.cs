﻿// <copyright file="SumDataLong.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Stats.Aggregations
{
    using System;

    public sealed class SumDataLong : AggregationData, ISumDataLong
    {
        public long Sum { get; }

        internal SumDataLong(long sum)
        {
            this.Sum = sum;
        }

        public static ISumDataLong Create(long sum)
        {
            return new SumDataLong(sum);
        }

        public override M Match<M>(
                Func<ISumDataDouble, M> p0,
                Func<ISumDataLong, M> p1,
                Func<ICountData, M> p2,
                Func<IMeanData, M> p3,
                Func<IDistributionData, M> p4,
                Func<ILastValueDataDouble, M> p5,
                Func<ILastValueDataLong, M> p6,
                Func<IAggregationData, M> defaultFunction)
        {
            return p1.Invoke(this);
        }

        public override string ToString()
        {
            return "SumDataLong{"
                + "sum=" + this.Sum
                + "}";
        }

        public override bool Equals(object o)
        {
            if (o == this)
            {
                return true;
            }

            if (o is SumDataLong that)
            {
                return this.Sum == that.Sum;
            }

            return false;
        }

        public override int GetHashCode()
        {
            long h = 1;
            h *= 1000003;
            h ^= (this.Sum >> 32) ^ this.Sum;
            return (int)h;
        }
    }
}
