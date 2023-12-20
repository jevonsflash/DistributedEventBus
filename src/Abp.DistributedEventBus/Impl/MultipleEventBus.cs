using Abp.DistributedEventBus.Exceptions;
using Castle.Core.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Events.Bus;
using Abp.Events.Bus.Handlers.Internals;
using Abp.DistributedEventBus.Events;
using Abp.Events.Bus.Factories;
using Abp.Events.Bus.Handlers;
using Abp.Threading.Extensions;
using Abp.Threading;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Abp.Events.Bus.Factories.Internals;
using Abp.Extensions;
using Castle.MicroKernel;
using Abp.Reflection;
using System.Text.RegularExpressions;
using Castle.MicroKernel.Registration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Abp.DistributedEventBus
{
    public class MultipleEventBus : IMultipleEventBus
    {
        public ILogger Logger { get; set; }

        private const string PayloadKey = "payload";
        private readonly ITypeFinder typeFinder;
        private readonly IDistributedEventProducer _publisher;
        private readonly IDistributedEventSubscriber _subscriber;
        private readonly IDistributedEventTopicSelector _topicSelector;
        private readonly IDistributedEventSerializer _remoteEventSerializer;

        private bool _disposed;

        public MultipleEventBus(
            ITypeFinder typeFinder,
            IDistributedEventProducer publisher,
            IDistributedEventSubscriber subscriber,
            IDistributedEventTopicSelector topicSelector,
            IDistributedEventSerializer remoteEventSerializer
        )
        {
            this.typeFinder = typeFinder;
            _publisher = publisher;
            _subscriber = subscriber;
            _topicSelector = topicSelector;
            _remoteEventSerializer = remoteEventSerializer;

            Logger = NullLogger.Instance;
            _handlerFactories = new ConcurrentDictionary<Type, List<IEventHandlerFactory>>();

        }


        /// <summary>
        /// Reference to the Logger.
        /// </summary>

        /// <summary>
        /// All registered handler factories.
        /// Key: Type of the event
        /// Value: List of handler factories
        /// </summary>
        private readonly ConcurrentDictionary<Type, List<IEventHandlerFactory>> _handlerFactories;

        /// <summary>
        /// Creates a new <see cref="EventBus"/> instance.
        /// Instead of creating a new instace, you can use <see cref="Default"/> to use Global <see cref="EventBus"/>.
        /// </summary>


        /// <inheritdoc/>
        public IDisposable Register<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            return Register(typeof(TEventData), new ActionEventHandler<TEventData>(action));
        }

        /// <inheritdoc/>
        public IDisposable AsyncRegister<TEventData>(Func<TEventData, Task> action) where TEventData : IEventData
        {
            return Register(typeof(TEventData), new AsyncActionEventHandler<TEventData>(action));
        }

        /// <inheritdoc/>
        public IDisposable Register<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData
        {
            return Register(typeof(TEventData), handler);
        }

        /// <inheritdoc/>
        public IDisposable AsyncRegister<TEventData>(IAsyncEventHandler<TEventData> handler) where TEventData : IEventData
        {
            return Register(typeof(TEventData), handler);
        }

        /// <inheritdoc/>
        public IDisposable Register<TEventData, THandler>()
            where TEventData : IEventData
            where THandler : IEventHandler, new()
        {
            return Register(typeof(TEventData), new TransientEventHandlerFactory<THandler>());
        }

        /// <inheritdoc/>
        public IDisposable Register(Type eventType, IEventHandler handler)
        {
            return Register(eventType, new SingleInstanceHandlerFactory(handler));
        }

        /// <inheritdoc/>
        public IDisposable Register<TEventData>(IEventHandlerFactory factory) where TEventData : IEventData
        {
            return Register(typeof(TEventData), factory);
        }

        /// <inheritdoc/>
        public IDisposable Register(Type eventType, IEventHandlerFactory factory)
        {
            GetOrCreateHandlerFactories(eventType);
            List<IEventHandlerFactory> currentLists;
            if (_handlerFactories.TryGetValue(eventType, out currentLists))
            {
                lock (currentLists)
                {
                    if (currentLists.Count == 0)
                    {
                        //Register to distributed event
                        this.Subscribe(eventType.ToString());
                    }
                    currentLists.Add(factory);
                }
            }
            return new FactoryUnregistrar(this, eventType, factory);
        }

        /// <inheritdoc/>
        public void Unregister<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            Check.NotNull(action, nameof(action));

            GetOrCreateHandlerFactories(typeof(TEventData))
                .Locking(factories =>
                {
                    factories.RemoveAll(
                        factory =>
                        {
                            var singleInstanceFactory = factory as SingleInstanceHandlerFactory;
                            if (singleInstanceFactory == null)
                            {
                                return false;
                            }

                            var actionHandler = singleInstanceFactory.HandlerInstance as ActionEventHandler<TEventData>;
                            if (actionHandler == null)
                            {
                                return false;
                            }

                            return actionHandler.Action == action;
                        });
                });
        }

        /// <inheritdoc/>
        public void AsyncUnregister<TEventData>(Func<TEventData, Task> action) where TEventData : IEventData
        {
            Check.NotNull(action, nameof(action));

            GetOrCreateHandlerFactories(typeof(TEventData))
                .Locking(factories =>
                {
                    factories.RemoveAll(
                        factory =>
                        {
                            var singleInstanceFactory = factory as SingleInstanceHandlerFactory;
                            if (singleInstanceFactory == null)
                            {
                                return false;
                            }

                            var actionHandler = singleInstanceFactory.HandlerInstance as AsyncActionEventHandler<TEventData>;
                            if (actionHandler == null)
                            {
                                return false;
                            }

                            return actionHandler.Action == action;
                        });
                });
        }

        /// <inheritdoc/>
        public void Unregister<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData
        {
            Unregister(typeof(TEventData), handler);
        }

        /// <inheritdoc/>
        public void AsyncUnregister<TEventData>(IAsyncEventHandler<TEventData> handler) where TEventData : IEventData
        {
            Unregister(typeof(TEventData), handler);
        }

        /// <inheritdoc/>
        public void Unregister(Type eventType, IEventHandler handler)
        {
            GetOrCreateHandlerFactories(eventType)
                .Locking(factories =>
                {
                    factories.RemoveAll(
                        factory =>
                            factory is SingleInstanceHandlerFactory &&
                            (factory as SingleInstanceHandlerFactory).HandlerInstance == handler
                        );
                });
        }

        /// <inheritdoc/>
        public void Unregister<TEventData>(IEventHandlerFactory factory) where TEventData : IEventData
        {
            Unregister(typeof(TEventData), factory);
        }

        /// <inheritdoc/>
        public void Unregister(Type eventType, IEventHandlerFactory factory)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Remove(factory));
        }

        /// <inheritdoc/>
        public void UnregisterAll<TEventData>() where TEventData : IEventData
        {
            UnregisterAll(typeof(TEventData));

        }

        /// <inheritdoc/>
        public void UnregisterAll(Type eventType)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories =>
            {

                factories.Clear();
                this.UnsubscribeAll();
            });
        }

        /// <inheritdoc/>
        public void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            Trigger((object)null, eventData);
        }

        /// <inheritdoc/>
        public void Trigger<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
        {
            Trigger(typeof(TEventData), eventSource, eventData);
        }

        /// <inheritdoc/>
        public void Trigger(Type eventType, IEventData eventData)
        {
            Trigger(eventType, null, eventData);
        }

        /// <inheritdoc/>
        public void Trigger(Type eventType, object eventSource, IEventData eventData)
        {
            var exceptions = new List<Exception>();

            eventData.EventSource = eventSource;

            foreach (var handlerFactories in GetHandlerFactories(eventType))
            {
                foreach (var handlerFactory in handlerFactories.EventHandlerFactories)
                {
                    var handlerType = handlerFactory.GetHandlerType();

                    if (IsAsyncEventHandler(handlerType))
                    {
                        AsyncHelper.RunSync(() =>
                        {

                            var task = TriggerAsyncHandlingException(handlerFactory, handlerFactories.EventType, eventData, exceptions);
                            return task;
                        });
                    }
                    else if (IsEventHandler(handlerType))
                    {
                        TriggerHandlingException(handlerFactory, handlerFactories.EventType, eventData, exceptions);
                    }
                    else
                    {
                        var message = $"Event handler to register for event type {eventType.Name} does not implement IEventHandler<{eventType.Name}> or IAsyncEventHandler<{eventType.Name}> interface!";
                        exceptions.Add(new AbpException(message));
                    }
                }
            }

            if (!(typeof(DistributedEventBusEvent) == eventType
               || typeof(DistributedEventBusEvent).IsAssignableFrom(eventType)
               || typeof(DistributedEventMessageHandleExceptionData) == eventType
               || typeof(DistributedEventHandleExceptionData) == eventType
                ))
            {
                if (typeof(DistributedEventArgs) != eventType)
                {
                    TriggerRemote(eventType, eventSource, eventData);

                }
            }



            //Implements generic argument inheritance. See IEventDataWithInheritableGenericArgument
            if (eventType.GetTypeInfo().IsGenericType &&
                eventType.GetGenericArguments().Length == 1 &&
                typeof(IEventDataWithInheritableGenericArgument).IsAssignableFrom(eventType))
            {
                var genericArg = eventType.GetGenericArguments()[0];
                var baseArg = genericArg.GetTypeInfo().BaseType;
                if (baseArg != null)
                {
                    var baseEventType = eventType.GetGenericTypeDefinition().MakeGenericType(baseArg);
                    var constructorArgs = ((IEventDataWithInheritableGenericArgument)eventData).GetConstructorArgs();
                    var baseEventData = (IEventData)Activator.CreateInstance(baseEventType, constructorArgs);
                    baseEventData.EventTime = eventData.EventTime;
                    Trigger(baseEventType, eventData.EventSource, baseEventData);
                }
            }

            if (exceptions.Any())
            {
                if (exceptions.Count == 1)
                {
                    exceptions[0].ReThrow();
                }

                throw new AggregateException("More than one error has occurred while triggering the event: " + eventType, exceptions);
            }

        }


        public void TriggerRemote(Type eventType, object eventSource, IEventData eventData)
        {
            var exceptions = new List<Exception>();
            eventData.EventSource = eventSource;
            TriggerDistributedEventHandlingException(eventType, eventData, exceptions);
            if (exceptions.Any())
            {
                if (exceptions.Count == 1)
                {
                    exceptions[0].ReThrow();
                }

                throw new AggregateException("More than one error has occurred while triggering the event: " + eventType, exceptions);
            }
        }


        /// <inheritdoc/>
        public Task TriggerAsync<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            return TriggerAsync((object)null, eventData);
        }

        /// <inheritdoc/>
        public Task TriggerAsync<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
        {
            return TriggerAsync(typeof(TEventData), eventSource, eventData);
        }

        /// <inheritdoc/>
        public Task TriggerAsync(Type eventType, IEventData eventData)
        {
            return TriggerAsync(eventType, null, eventData);
        }

        /// <inheritdoc/>
        public async Task TriggerAsync(Type eventType, object eventSource, IEventData eventData)
        {
            var exceptions = new List<Exception>();

            eventData.EventSource = eventSource;

            await new SynchronizationContextRemover();

            foreach (var handlerFactories in GetHandlerFactories(eventType))
            {
                foreach (var handlerFactory in handlerFactories.EventHandlerFactories)
                {
                    var handlerType = handlerFactory.GetHandlerType();

                    if (IsAsyncEventHandler(handlerType))
                    {
                        await TriggerAsyncHandlingException(handlerFactory, handlerFactories.EventType, eventData, exceptions);
                    }
                    else if (IsEventHandler(handlerType))
                    {
                        TriggerHandlingException(handlerFactory, handlerFactories.EventType, eventData, exceptions);

                    }
                    else
                    {
                        var message = $"Event handler to register for event type {eventType.Name} does not implement IEventHandler<{eventType.Name}> or IAsyncEventHandler<{eventType.Name}> interface!";
                        exceptions.Add(new AbpException(message));
                    }
                }
            }

            //Implements generic argument inheritance. See IEventDataWithInheritableGenericArgument
            if (eventType.GetTypeInfo().IsGenericType &&
                eventType.GetGenericArguments().Length == 1 &&
                typeof(IEventDataWithInheritableGenericArgument).IsAssignableFrom(eventType))
            {
                var genericArg = eventType.GetGenericArguments()[0];
                var baseArg = genericArg.GetTypeInfo().BaseType;
                if (baseArg != null)
                {
                    var baseEventType = eventType.GetGenericTypeDefinition().MakeGenericType(baseArg);
                    var constructorArgs = ((IEventDataWithInheritableGenericArgument)eventData).GetConstructorArgs();
                    var baseEventData = (IEventData)Activator.CreateInstance(baseEventType, constructorArgs);
                    baseEventData.EventTime = eventData.EventTime;
                    await TriggerAsync(baseEventType, eventData.EventSource, baseEventData);
                }
            }

            if (exceptions.Any())
            {
                if (exceptions.Count == 1)
                {
                    exceptions[0].ReThrow();
                }

                throw new AggregateException("More than one error has occurred while triggering the event: " + eventType, exceptions);
            }
        }

        private void TriggerHandlingException(IEventHandlerFactory handlerFactory, Type eventType, IEventData eventData, List<Exception> exceptions)
        {
            var eventHandler = handlerFactory.GetHandler();
            try
            {
                if (eventHandler == null)
                {
                    throw new ArgumentNullException($"Registered event handler for event type {eventType.Name} is null!");
                }

                var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);

                var method = handlerType.GetMethod(
                    "HandleEvent",
                    new[] { eventType }
                );

                method.Invoke(eventHandler, new object[] { eventData });
            }
            catch (TargetInvocationException ex)
            {
                exceptions.Add(ex.InnerException);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
            finally
            {
                handlerFactory.ReleaseHandler(eventHandler);
            }
        }

        private void TriggerDistributedEventHandlingException(Type eventType, IEventData eventData, List<Exception> exceptions)
        {

            try
            {
                var payloadDictionary = new Dictionary<string, object>
                        {
                            { PayloadKey, eventData }
                        };
                var distributedeventData = new DistributedEventData(eventType.ToString(), payloadDictionary);
                Publish(distributedeventData);
            }

            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }


        private async Task TriggerAsyncHandlingException(IEventHandlerFactory asyncHandlerFactory, Type eventType, IEventData eventData, List<Exception> exceptions)
        {
            var asyncEventHandler = asyncHandlerFactory.GetHandler();

            try
            {
                if (asyncEventHandler == null)
                {
                    throw new ArgumentNullException($"Registered async event handler for event type {eventType.Name} is null!");
                }

                var asyncHandlerType = typeof(IAsyncEventHandler<>).MakeGenericType(eventType);

                var method = asyncHandlerType.GetMethod(
                    "HandleEventAsync",
                    new[] { eventType }
                );

                await (Task)method.Invoke(asyncEventHandler, new object[] { eventData });
            }
            catch (TargetInvocationException ex)
            {
                exceptions.Add(ex.InnerException);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
            finally
            {
                asyncHandlerFactory.ReleaseHandler(asyncEventHandler);
            }
        }

        private bool IsEventHandler(Type handlerType)
        {
            return handlerType.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Any(i => i.GetGenericTypeDefinition() == typeof(IEventHandler<>));
        }

        private bool IsAsyncEventHandler(Type handlerType)
        {
            return handlerType.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Any(i => i.GetGenericTypeDefinition() == typeof(IAsyncEventHandler<>));
        }

        private IEnumerable<EventTypeWithEventHandlerFactories> GetHandlerFactories(Type eventType)
        {
            var handlerFactoryList = new List<EventTypeWithEventHandlerFactories>();

            foreach (var handlerFactory in _handlerFactories.Where(hf => ShouldTriggerEventForHandler(eventType, hf.Key)))
            {
                handlerFactoryList.Add(new EventTypeWithEventHandlerFactories(handlerFactory.Key, handlerFactory.Value));
            }

            return handlerFactoryList.ToArray();
        }

        private static bool ShouldTriggerEventForHandler(Type eventType, Type handlerType)
        {
            //Should trigger same type
            if (handlerType == eventType)
            {
                return true;
            }

            //Should trigger for inherited types
            if (handlerType.IsAssignableFrom(eventType))
            {
                return true;
            }

            return false;
        }

        private List<IEventHandlerFactory> GetOrCreateHandlerFactories(Type eventType)
        {
            return _handlerFactories.GetOrAdd(eventType, (type) => new List<IEventHandlerFactory>());
        }

        private class EventTypeWithEventHandlerFactories
        {
            public Type EventType { get; }

            public List<IEventHandlerFactory> EventHandlerFactories { get; }

            public EventTypeWithEventHandlerFactories(Type eventType, List<IEventHandlerFactory> eventHandlerFactories)
            {
                EventType = eventType;
                EventHandlerFactories = eventHandlerFactories;
            }
        }

        // Reference from
        // https://blogs.msdn.microsoft.com/benwilli/2017/02/09/an-alternative-to-configureawaitfalse-everywhere/
        private struct SynchronizationContextRemover : INotifyCompletion
        {
            public bool IsCompleted
            {
                get { return SynchronizationContext.Current == null; }
            }

            public void OnCompleted(Action continuation)
            {
                var prevContext = SynchronizationContext.Current;
                try
                {
                    SynchronizationContext.SetSynchronizationContext(null);
                    continuation();
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(prevContext);
                }
            }

            public SynchronizationContextRemover GetAwaiter()
            {
                return this;
            }

            public void GetResult()
            {
            }
        }





        public void Publish(IDistributedEventData eventData)
        {
            Publish(_topicSelector.SelectTopic(eventData), eventData);
        }

        public Task PublishAsync(IDistributedEventData eventData)
        {
            return PublishAsync(_topicSelector.SelectTopic(eventData), eventData);
        }

        public void Publish(string topic, IDistributedEventData eventData)
        {
            Trigger(this, new DistributedEventBusPublishingEvent(eventData));
            _publisher.Publish(topic, eventData);
            Logger.Debug($"Event published on topic {topic}");
            Trigger(this, new DistributedEventBusPublishedEvent(eventData));
        }

        public async Task PublishAsync(string topic, IDistributedEventData eventData)
        {
            await TriggerAsync(this, new DistributedEventBusPublishingEvent(eventData));
            await _publisher.PublishAsync(topic, eventData)
                .ContinueWith((s) => Logger.Debug($"Event published on topic {topic}"));
            await TriggerAsync(this, new DistributedEventBusPublishedEvent(eventData));
            await Task.FromResult(0);
        }

        public void Subscribe(string topic)
        {
            Subscribe(new[] { topic });
        }

        public Task SubscribeAsync(string topic)
        {
            return SubscribeAsync(new[] { topic });
        }

        public void Subscribe(IEnumerable<string> topics)
        {
            _subscriber.Subscribe(topics, MessageHandle);
            Logger.Debug($"Subscribed topics {string.Join(",", topics)}");
        }

        public Task SubscribeAsync(IEnumerable<string> topics)
        {
            return _subscriber.SubscribeAsync(topics, MessageHandle)
                .ContinueWith((s) => Logger.Debug($"Subscribed topics {string.Join(",", topics)}"));
        }

        public void Unsubscribe(string topic)
        {
            Unsubscribe(new[] { topic });
        }

        public Task UnsubscribeAsync(string topic)
        {
            return UnsubscribeAsync(new[] { topic });
        }

        public void Unsubscribe(IEnumerable<string> topics)
        {
            _subscriber.Unsubscribe(topics);
            Logger.Debug($"Unsubscribed topics {string.Join(",", topics)}");
        }

        public Task UnsubscribeAsync(IEnumerable<string> topics)
        {
            return _subscriber.UnsubscribeAsync(topics)
                .ContinueWith((s) => Logger.Debug($"Unsubscribed topics {string.Join(",", topics)}"));
        }

        public virtual void MessageHandle(string topic, string message)
        {
            Logger.Debug($"Receive message on topic {topic}");
            try
            {
                var eventData = _remoteEventSerializer.Deserialize<DistributedEventData>(message);
                var eventArgs = new DistributedEventArgs(eventData, topic, message);
                Trigger(this, new DistributedEventBusHandlingEvent(eventArgs));

                if (!string.IsNullOrEmpty(eventData.Type))
                {
                    string pattern = @"(.*?)\[(.*?)\]";
                    Match match = Regex.Match(eventData.Type, pattern);
                    if (match.Success)
                    {

                        var type = match.Groups[1].Value;
                        var type2 = match.Groups[2].Value;

                        var localTriggerType = typeFinder.Find(c => c.FullName == type).FirstOrDefault();
                        var genericType = typeFinder.Find(c => c.FullName == type2).FirstOrDefault();

                        if (localTriggerType != null && genericType != null)
                        {

                            if (localTriggerType.GetTypeInfo().IsGenericType
                                && localTriggerType.GetGenericArguments().Length == 1
                                && !genericType.IsAbstract && !genericType.IsInterface
                                )
                            {
                                var localTriggerGenericType = localTriggerType.GetGenericTypeDefinition().MakeGenericType(genericType);


                                if (eventData.Data.TryGetValue(PayloadKey, out var payload))
                                {
                                    var payloadObject = (payload as JObject).ToObject(localTriggerGenericType);
                                    Trigger(localTriggerGenericType, this, (IEventData)payloadObject);

                                }
                            }
                        }


                    }
                    else
                    {
                        var localTriggerType = typeFinder.Find(c => c.FullName == eventData.Type).FirstOrDefault();
                        if (localTriggerType != null && !localTriggerType.IsAbstract && !localTriggerType.IsInterface)
                        {
                            if (eventData.Data.TryGetValue(PayloadKey, out var payload))
                            {
                                var payloadObject = (payload as JObject).ToObject(localTriggerType);
                                Trigger(localTriggerType, this, (IEventData)payloadObject);

                            }

                        }
                    }
                    Trigger(this, new DistributedEventBusHandledEvent(eventArgs));

                }
            }
            catch (Exception ex)
            {
                Logger.Error("Consume remote message exception", ex);
                Trigger(this, new DistributedEventMessageHandleExceptionData(ex, topic, topic));
            }
        }

        public void UnsubscribeAll()
        {
            _subscriber.UnsubscribeAll();
            Logger.Debug($"Unsubscribed all topics");
        }

        public Task UnsubscribeAllAsync()
        {
            return _subscriber.UnsubscribeAllAsync()
                .ContinueWith((s) => Logger.Debug($"Unsubscribes all topics"));
            ;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                _subscriber?.Dispose();
                _publisher?.Dispose();

                _disposed = true;
            }
        }
    }
}