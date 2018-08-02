﻿namespace OpenCensus.Trace
{
    using OpenCensus.Common;

    public interface ITracer
    {
        ISpan CurrentSpan { get; }

        IScope WithSpan(ISpan span);

        //<C> Callable<C> withSpan(Span span, final Callable<C> callable)
        //Runnable withSpan(Span span, Runnable runnable)
        ISpanBuilder SpanBuilder(string spanName);

        ISpanBuilder SpanBuilderWithExplicitParent(string spanName, ISpan parent = null);

        ISpanBuilder SpanBuilderWithRemoteParent(string spanName, ISpanContext remoteParentSpanContext = null);
    }
}
