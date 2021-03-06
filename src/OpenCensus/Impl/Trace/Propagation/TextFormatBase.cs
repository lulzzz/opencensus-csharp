﻿// <copyright file="TextFormatBase.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Trace.Propagation
{
    using System.Collections.Generic;

    public abstract class TextFormatBase : ITextFormat
    {
        private static readonly NoopTextFormat NOOP_TEXT_FORMAT = new NoopTextFormat();

        internal static ITextFormat NoopTextFormat
        {
            get
            {
                return NOOP_TEXT_FORMAT;
            }
        }

        public abstract IList<string> Fields { get; }

        public abstract ISpanContext Extract<C>(C carrier, IGetter<C> getter);

        public abstract void Inject<C>(ISpanContext spanContext, C carrier, ISetter<C> setter);
    }
}
