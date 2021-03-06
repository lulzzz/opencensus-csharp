﻿// <copyright file="SpanDataTest.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Trace.Export.Test
{
    using System.Collections.Generic;
    using OpenCensus.Common;
    using OpenCensus.Trace.Internal;
    using Xunit;

    public class SpanDataTest
    {
        private static readonly ITimestamp startTimestamp = Timestamp.Create(123, 456);
        private static readonly ITimestamp eventTimestamp1 = Timestamp.Create(123, 457);
        private static readonly ITimestamp eventTimestamp2 = Timestamp.Create(123, 458);
        private static readonly ITimestamp eventTimestamp3 = Timestamp.Create(123, 459);
        private static readonly ITimestamp endTimestamp = Timestamp.Create(123, 460);
        private static readonly string SPAN_NAME = "MySpanName";
        private static readonly string ANNOTATION_TEXT = "MyAnnotationText";
        private static readonly IAnnotation annotation = Annotation.FromDescription(ANNOTATION_TEXT);
        // private static readonly NetworkEvent recvNetworkEvent =
        //    NetworkEvent.Builder(NetworkEvent.Type.RECV, 1).build();
        //      private static readonly NetworkEvent sentNetworkEvent =
        //    NetworkEvent.Builder(NetworkEvent.Type.SENT, 1).build();
        private static readonly IMessageEvent recvMessageEvent = MessageEvent.Builder(MessageEventType.RECEIVED, 1).Build();
        private static readonly IMessageEvent sentMessageEvent = MessageEvent.Builder(MessageEventType.SENT, 1).Build();
        private static readonly Status status = Status.DEADLINE_EXCEEDED.WithDescription("TooSlow");
        private static readonly int CHILD_SPAN_COUNT = 13;
        private readonly IRandomGenerator random = new RandomGenerator(1234);
        private readonly ISpanContext spanContext;
        private readonly ISpanId parentSpanId; 
        private readonly IDictionary<string, IAttributeValue> attributesMap = new Dictionary<string, IAttributeValue>();
        private readonly IList<ITimedEvent<IAnnotation>> annotationsList = new List<ITimedEvent<IAnnotation>>();
        // private readonly List<TimedEvent<NetworkEvent>> networkEventsList =
        //    new List<SpanData.TimedEvent<NetworkEvent>>();
        private readonly IList<ITimedEvent<IMessageEvent>> messageEventsList = new List<ITimedEvent<IMessageEvent>>();
        private readonly IList<ILink> linksList = new List<ILink>();

        private IAttributes attributes;
        private ITimedEvents<IAnnotation> annotations;
        // private TimedEvents<NetworkEvent> networkEvents;
        private ITimedEvents<IMessageEvent> messageEvents;
        private LinkList links;

        public SpanDataTest()
        {
            spanContext = SpanContext.Create(TraceId.GenerateRandomId(random), SpanId.GenerateRandomId(random), TraceOptions.DEFAULT);
            parentSpanId = SpanId.GenerateRandomId(random);

            attributesMap.Add("MyAttributeKey1", AttributeValue.LongAttributeValue(10));
            attributesMap.Add("MyAttributeKey2", AttributeValue.BooleanAttributeValue(true));
            attributes = Attributes.Create(attributesMap, 1);

            annotationsList.Add(TimedEvent<IAnnotation>.Create(eventTimestamp1, annotation));
            annotationsList.Add(TimedEvent<IAnnotation>.Create(eventTimestamp3, annotation));
            annotations = TimedEvents<IAnnotation>.Create(annotationsList, 2);

            // networkEventsList.add(SpanData.TimedEvent.Create(eventTimestamp1, recvNetworkEvent));
            // networkEventsList.add(SpanData.TimedEvent.Create(eventTimestamp2, sentNetworkEvent));
            // networkEvents = TimedEvents.Create(networkEventsList, 3);

            messageEventsList.Add(TimedEvent<IMessageEvent>.Create(eventTimestamp1, recvMessageEvent));
            messageEventsList.Add(TimedEvent<IMessageEvent>.Create(eventTimestamp2, sentMessageEvent));
            messageEvents = TimedEvents<IMessageEvent>.Create(messageEventsList, 3);

            linksList.Add(Link.FromSpanContext(spanContext, LinkType.CHILD_LINKED_SPAN));
            links = LinkList.Create(linksList, 0);
        }

        [Fact]
        public void SpanData_AllValues()
        {
            ISpanData spanData =
                SpanData.Create(
                    spanContext,
                    parentSpanId,
                    true,
                    SPAN_NAME,
                    startTimestamp,
                    attributes,
                    annotations,
                    messageEvents,
                    links,
                    CHILD_SPAN_COUNT,
                    status,
                    endTimestamp);
            Assert.Equal(spanContext, spanData.Context);
            Assert.Equal(parentSpanId, spanData.ParentSpanId);
            Assert.True(spanData.HasRemoteParent);
            Assert.Equal(SPAN_NAME, spanData.Name);
            Assert.Equal(startTimestamp, spanData.StartTimestamp);
            Assert.Equal(attributes, spanData.Attributes);
            Assert.Equal(annotations, spanData.Annotations);
            Assert.Equal(messageEvents, spanData.MessageEvents);
            Assert.Equal(links, spanData.Links);
            Assert.Equal(CHILD_SPAN_COUNT, spanData.ChildSpanCount);
            Assert.Equal(status, spanData.Status);
            Assert.Equal(endTimestamp, spanData.EndTimestamp);
        }

        // [Fact]
        // public void SpanData_Create_Compatibility()
        // {
        //    SpanData spanData =
        //        SpanData.Create(
        //            spanContext,
        //            parentSpanId,
        //            true,
        //            SPAN_NAME,
        //            startTimestamp,
        //            attributes,
        //            annotations,
        //            networkEvents,
        //            links,
        //            CHILD_SPAN_COUNT,
        //            status,
        //            endTimestamp);
        //    Assert.Equal(spanData.getContext()).isEqualTo(spanContext);
        //    Assert.Equal(spanData.getParentSpanId()).isEqualTo(parentSpanId);
        //    Assert.Equal(spanData.getHasRemoteParent()).isTrue();
        //    Assert.Equal(spanData.getName()).isEqualTo(SPAN_NAME);
        //    Assert.Equal(spanData.getStartTimestamp()).isEqualTo(startTimestamp);
        //    Assert.Equal(spanData.getAttributes()).isEqualTo(attributes);
        //    Assert.Equal(spanData.getAnnotations()).isEqualTo(annotations);
        //    Assert.Equal(spanData.getNetworkEvents()).isEqualTo(networkEvents);
        //    Assert.Equal(spanData.getMessageEvents()).isEqualTo(messageEvents);
        //    Assert.Equal(spanData.getLinks()).isEqualTo(links);
        //    Assert.Equal(spanData.getChildSpanCount()).isEqualTo(CHILD_SPAN_COUNT);
        //    Assert.Equal(spanData.getStatus()).isEqualTo(status);
        //    Assert.Equal(spanData.getEndTimestamp()).isEqualTo(endTimestamp);
        // }

        [Fact]
        public void SpanData_RootActiveSpan()
        {
            ISpanData spanData =
                SpanData.Create(
                    spanContext,
                    null,
                    null,
                    SPAN_NAME,
                    startTimestamp,
                    attributes,
                    annotations,
                    messageEvents,
                    links,
                    null,
                    null,
                    null);
            Assert.Equal(spanContext, spanData.Context);
            Assert.Null(spanData.ParentSpanId);
            Assert.Null(spanData.HasRemoteParent);
            Assert.Equal(SPAN_NAME, spanData.Name);
            Assert.Equal(startTimestamp, spanData.StartTimestamp);
            Assert.Equal(attributes, spanData.Attributes);
            Assert.Equal(annotations, spanData.Annotations);
            Assert.Equal(messageEvents, spanData.MessageEvents);
            Assert.Equal(links, spanData.Links);
            Assert.Null(spanData.ChildSpanCount);
            Assert.Null(spanData.Status);
            Assert.Null(spanData.EndTimestamp);
        }

        [Fact]
        public void SpanData_AllDataEmpty()
        {
            ISpanData spanData =
                SpanData.Create(
                    spanContext,
                    parentSpanId,
                    false,
                    SPAN_NAME,
                    startTimestamp,
                    Attributes.Create(new Dictionary<string, IAttributeValue>(), 0),
                    TimedEvents<IAnnotation>.Create(new List<ITimedEvent<IAnnotation>>(), 0),
                    TimedEvents<IMessageEvent>.Create(new List<ITimedEvent<IMessageEvent>>(), 0),
                    LinkList.Create(new List<ILink>(), 0),
                    0,
                    status,
                    endTimestamp);

            Assert.Equal(spanContext, spanData.Context);
            Assert.Equal(parentSpanId, spanData.ParentSpanId);
            Assert.False(spanData.HasRemoteParent);
            Assert.Equal(SPAN_NAME, spanData.Name);
            Assert.Equal(startTimestamp, spanData.StartTimestamp);
            Assert.Empty(spanData.Attributes.AttributeMap);
            Assert.Empty(spanData.Annotations.Events);
            Assert.Empty(spanData.MessageEvents.Events);
            Assert.Empty(spanData.Links.Links);
            Assert.Equal(0, spanData.ChildSpanCount);
            Assert.Equal(status, spanData.Status);
            Assert.Equal(endTimestamp, spanData.EndTimestamp);
        }

        [Fact]
        public void SpanDataEquals()
        {
            ISpanData allSpanData1 =
                SpanData.Create(
                    spanContext,
                    parentSpanId,
                    false,
                    SPAN_NAME,
                    startTimestamp,
                    attributes,
                    annotations,
                    messageEvents,
                    links,
                    CHILD_SPAN_COUNT,
                    status,
                    endTimestamp);
            ISpanData allSpanData2 =
                SpanData.Create(
                    spanContext,
                    parentSpanId,
                    false,
                    SPAN_NAME,
                    startTimestamp,
                    attributes,
                    annotations,
                    messageEvents,
                    links,
                    CHILD_SPAN_COUNT,
                    status,
                    endTimestamp);
            ISpanData emptySpanData =
                SpanData.Create(
                    spanContext,
                    parentSpanId,
                    false,
                    SPAN_NAME,
                    startTimestamp,
                    Attributes.Create(new Dictionary<string, IAttributeValue>(), 0),
                    TimedEvents<IAnnotation>.Create(new List<ITimedEvent<IAnnotation>>(), 0),
                    TimedEvents<IMessageEvent>.Create(new List<ITimedEvent<IMessageEvent>>(), 0),
                    LinkList.Create(new List<ILink>(), 0),
                    0,
                    status,
                    endTimestamp);

            Assert.Equal(allSpanData1, allSpanData2);
            Assert.NotEqual(emptySpanData, allSpanData1);
            Assert.NotEqual(emptySpanData, allSpanData2);

        }

        [Fact]
        public void SpanData_ToString()
        {
            string spanDataString =
                SpanData.Create(
                        spanContext,
                        parentSpanId,
                        false,
                        SPAN_NAME,
                        startTimestamp,
                        attributes,
                        annotations,
                        messageEvents,
                        links,
                        CHILD_SPAN_COUNT,
                        status,
                        endTimestamp)
                    .ToString();
            Assert.Contains(spanContext.ToString(), spanDataString);
            Assert.Contains(parentSpanId.ToString(), spanDataString);
            Assert.Contains(SPAN_NAME, spanDataString);
            Assert.Contains(startTimestamp.ToString(), spanDataString);
            Assert.Contains(attributes.ToString(), spanDataString);
            Assert.Contains(annotations.ToString(), spanDataString);
            Assert.Contains(messageEvents.ToString(), spanDataString);
            Assert.Contains(links.ToString(), spanDataString);
            Assert.Contains(status.ToString(), spanDataString);
            Assert.Contains(endTimestamp.ToString(), spanDataString);
        }
    }
}
