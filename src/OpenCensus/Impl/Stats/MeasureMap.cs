﻿namespace OpenCensus.Stats
{
    using OpenCensus.Stats.Measures;
    using OpenCensus.Tags;

    internal sealed class MeasureMap : MeasureMapBase
    {
        private readonly StatsManager statsManager;
        private readonly MeasureMapBuilder builder = MeasureMapBuilder.Builder();

        internal static IMeasureMap Create(StatsManager statsManager)
        {
            return new MeasureMap(statsManager);
        }

        internal MeasureMap(StatsManager statsManager)
        {
            this.statsManager = statsManager;
        }

        public override IMeasureMap Put(IMeasureDouble measure, double value)
        {
            builder.Put(measure, value);
            return this;
        }

        public override IMeasureMap Put(IMeasureLong measure, long value)
        {
            builder.Put(measure, value);
            return this;
        }

        public override void Record()
        {
            // Use the context key directly, to avoid depending on the tags implementation.
            Record(CurrentTagContextUtils.CurrentTagContext);
        }

        public override void Record(ITagContext tags)
        {
            statsManager.Record(tags, builder.Build());
        }
    }
}
