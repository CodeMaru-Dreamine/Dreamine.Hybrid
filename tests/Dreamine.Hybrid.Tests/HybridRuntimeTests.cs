using Dreamine.Hybrid.Interfaces;
using Dreamine.Hybrid.Messaging;
using Dreamine.Hybrid.State;
using Xunit;

namespace Dreamine.Hybrid.Tests;

public sealed class HybridRuntimeTests
{
    [Fact]
    public void MessageBaseCreatesIdentityAndTimestamp()
    {
        var before = DateTimeOffset.Now;
        var message = new TestMessage();

        Assert.NotEqual(Guid.Empty, message.Id);
        Assert.InRange(message.CreatedAt, before, DateTimeOffset.Now);
    }

    [Fact]
    public async Task MessageBusPublishesInSubscriptionOrderAndSupportsDisposal()
    {
        var bus = new InMemoryHybridMessageBus();
        var received = new List<int>();
        using var first = bus.Subscribe<TestMessage>((_, _) =>
        {
            received.Add(1);
            return Task.CompletedTask;
        });
        var second = bus.Subscribe<TestMessage>((_, _) =>
        {
            received.Add(2);
            return Task.CompletedTask;
        });

        await bus.PublishAsync(new TestMessage());
        second.Dispose();
        second.Dispose();
        await bus.PublishAsync(new TestMessage());

        Assert.Equal([1, 2, 1], received);
    }

    [Fact]
    public async Task MessageBusIsolatesSubscriberFailures()
    {
        var handler = new RecordingExceptionHandler();
        var bus = new InMemoryHybridMessageBus { ExceptionHandler = handler };
        var delivered = false;
        bus.Subscribe<TestMessage>((_, _) => throw new ApplicationException("failure"));
        bus.Subscribe<TestMessage>((_, _) =>
        {
            delivered = true;
            return Task.CompletedTask;
        });

        await bus.PublishAsync(new TestMessage());

        Assert.True(delivered);
        Assert.IsType<ApplicationException>(handler.Exception);
        Assert.Equal(typeof(TestMessage), handler.MessageType);
    }

    [Fact]
    public async Task MessageBusHonorsCancellationAndRemovesInvalidSubscriber()
    {
        var bus = new InMemoryHybridMessageBus();
        var calls = 0;
        bus.Subscribe<TestMessage>((_, _) =>
        {
            calls++;
            throw new ObjectDisposedException("subscriber");
        });

        await bus.PublishAsync(new TestMessage());
        await bus.PublishAsync(new TestMessage());

        using var cancelled = new CancellationTokenSource();
        cancelled.Cancel();
        await bus.PublishAsync(new TestMessage(), cancelled.Token);
        Assert.Equal(1, calls);
    }

    [Fact]
    public async Task MessageBusValidatesArgumentsAndAllowsNoSubscribers()
    {
        var bus = new InMemoryHybridMessageBus();
        await bus.PublishAsync(new TestMessage());

        Assert.Throws<ArgumentNullException>(() => bus.ExceptionHandler = null!);
        Assert.Throws<ArgumentNullException>(() =>
            bus.Subscribe<TestMessage>(null!));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            bus.PublishAsync<TestMessage>(null!));
    }

    [Fact]
    public void StateStoreRaisesSnapshotsAndSubscriptionCanBeDisposed()
    {
        var store = new HybridStateStore<int>(1);
        var snapshots = new List<int>();
        var subscription = store.Subscribe((_, args) => snapshots.Add(args.State));

        store.SetState(2);
        store.Update(value => value + 3);
        subscription.Dispose();
        subscription.Dispose();
        store.SetState(9);

        Assert.Equal(9, store.State);
        Assert.Equal([2, 5], snapshots);
    }

    [Fact]
    public void StateStoreValidatesHandlersAndUpdater()
    {
        var store = new HybridStateStore<string>("initial");

        Assert.Throws<ArgumentNullException>(() => store.Subscribe(null!));
        Assert.Throws<ArgumentNullException>(() => store.Update(null!));
        Assert.Equal("initial", store.State);
    }

    [Fact]
    public void StateChangedEventArgsExposeState()
    {
        var args = new HybridStateChangedEventArgs<string>("ready");
        Assert.Equal("ready", args.State);
    }

    private sealed class TestMessage : HybridMessageBase;

    private sealed class RecordingExceptionHandler : IHybridMessageBusExceptionHandler
    {
        public Exception? Exception { get; private set; }
        public Type? MessageType { get; private set; }

        public void Handle(Exception exception, Type messageType)
        {
            Exception = exception;
            MessageType = messageType;
        }
    }
}
