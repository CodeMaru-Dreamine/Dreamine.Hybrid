using Dreamine.Hybrid.Interfaces;
using System;
using System.Threading;

namespace Dreamine.Hybrid.State
{
    /// <summary>
        /// \if KO
        /// <para>하이브리드 애플리케이션을 위한 스레드 안전 인메모리 공유 상태 저장소를 제공합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Provides a thread-safe in-memory shared-state store for hybrid applications.</para>
        /// \endif
    /// </summary>
        /// <typeparam name="TState">
        /// \if KO
        /// <para>저장할 상태 형식입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The state type to store.</para>
        /// \endif
        /// </typeparam>
    public sealed class HybridStateStore<TState> : IHybridStateStore<TState>
    {
        /// <summary>
        /// \if KO
        /// <para>sync Root 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the sync root value.</para>
        /// \endif
        /// </summary>
        private readonly object _syncRoot = new();
        /// <summary>
        /// \if KO
        /// <para>state 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the state value.</para>
        /// \endif
        /// </summary>
        private TState _state;

        /// <summary>
        /// \if KO
        /// <para>지정한 초기 상태로 <see cref="HybridStateStore{TState}"/>의 새 인스턴스를 초기화합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Initializes a new <see cref="HybridStateStore{TState}"/> instance with the specified initial state.</para>
        /// \endif
        /// </summary>
        /// <param name="initialState">
        /// \if KO
        /// <para>저장소가 처음 보유할 상태입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The state initially held by the store.</para>
        /// \endif
        /// </param>
        public HybridStateStore(TState initialState)
        {
            _state = initialState;
        }

        /// <summary>
        /// \if KO
        /// <para>저장된 상태가 변경되었을 때 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Occurs when the stored state changes.</para>
        /// \endif
        /// </summary>
        public event EventHandler<HybridStateChangedEventArgs<TState>>? StateChanged;

        /// <summary>
        /// \if KO
        /// <para>상태 변경 처리기를 등록하고 해제 가능한 구독을 반환합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Registers a state-change handler and returns a disposable subscription.</para>
        /// \endif
        /// </summary>
        /// <param name="handler">
        /// \if KO
        /// <para>등록할 상태 변경 처리기입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The state-change handler to register.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>해제할 때 처리기를 제거하는 구독입니다.</para>
        /// \endif
        /// \if EN
        /// <para>A subscription that removes the handler when disposed.</para>
        /// \endif
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// \if KO
        /// <para><paramref name="handler"/>가 <see langword="null"/>인 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when <paramref name="handler"/> is <see langword="null"/>.</para>
        /// \endif
        /// </exception>
        public IDisposable Subscribe(EventHandler<HybridStateChangedEventArgs<TState>> handler)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            StateChanged += handler;
            return new StateSubscription(this, handler);
        }

        /// <summary>
        /// \if KO
        /// <para>잠금으로 보호된 현재 상태 스냅숏을 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets a lock-protected snapshot of the current state.</para>
        /// \endif
        /// </summary>
        public TState State
        {
            get
            {
                lock (_syncRoot)
                {
                    return _state;
                }
            }
        }

        /// <summary>
        /// \if KO
        /// <para>현재 상태를 교체하고 변경 이벤트를 발생시킵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Replaces the current state and raises the change event.</para>
        /// \endif
        /// </summary>
        /// <param name="state">
        /// \if KO
        /// <para>저장할 새 상태입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The new state to store.</para>
        /// \endif
        /// </param>
        public void SetState(TState state)
        {
            TState snapshot;

            lock (_syncRoot)
            {
                _state = state;
                // Capture the snapshot inside the lock so the event argument always
                // reflects the stored value at the time of assignment, even when
                // concurrent SetState calls race. Subscribers receive the state that
                // was actually committed, not a stale caller-held reference.
                snapshot = _state;
            }

            OnStateChanged(snapshot);
        }

        /// <summary>
        /// \if KO
        /// <para>변환 함수를 잠금 안에서 실행하여 상태를 원자적으로 갱신하고 변경 이벤트를 발생시킵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Atomically updates the state by invoking a transformation under the lock, then raises the change event.</para>
        /// \endif
        /// </summary>
        /// <param name="updater">
        /// \if KO
        /// <para>현재 상태를 새 상태로 변환하는 함수입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The function that transforms the current state into a new state.</para>
        /// \endif
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// \if KO
        /// <para><paramref name="updater"/>가 <see langword="null"/>인 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when <paramref name="updater"/> is <see langword="null"/>.</para>
        /// \endif
        /// </exception>
        public void Update(Func<TState, TState> updater)
        {
            if (updater is null)
            {
                throw new ArgumentNullException(nameof(updater));
            }

            TState snapshot;

            lock (_syncRoot)
            {
                snapshot = updater(_state);
                _state = snapshot;
            }

            OnStateChanged(snapshot);
        }

        /// <summary>
        /// \if KO
        /// <para>지정한 상태 스냅숏을 포함하여 상태 변경 이벤트를 발생시킵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Raises the state-change event with the specified state snapshot.</para>
        /// \endif
        /// </summary>
        /// <param name="state">
        /// \if KO
        /// <para>이벤트 구독자에게 전달할 상태입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The state to deliver to event subscribers.</para>
        /// \endif
        /// </param>
        private void OnStateChanged(TState state)
        {
            StateChanged?.Invoke(this, new HybridStateChangedEventArgs<TState>(state));
        }

        /// <summary>
        /// \if KO
        /// <para>상태 변경 처리기를 정확히 한 번 제거하는 구독 핸들입니다.</para>
        /// \endif
        /// \if EN
        /// <para>Represents a subscription handle that removes a state-change handler exactly once.</para>
        /// \endif
        /// </summary>
        private sealed class StateSubscription : IDisposable
        {
            /// <summary>
            /// \if KO
            /// <para>store 값을 보관합니다.</para>
            /// \endif
            /// \if EN
            /// <para>Stores the store value.</para>
            /// \endif
            /// </summary>
            private readonly HybridStateStore<TState> _store;
            /// <summary>
            /// \if KO
            /// <para>handler 값을 보관합니다.</para>
            /// \endif
            /// \if EN
            /// <para>Stores the handler value.</para>
            /// \endif
            /// </summary>
            private EventHandler<HybridStateChangedEventArgs<TState>>? _handler;

            /// <summary>
            /// \if KO
            /// <para>대상 저장소와 처리기로 새 구독 핸들을 초기화합니다.</para>
            /// \endif
            /// \if EN
            /// <para>Initializes a new subscription handle for the target store and handler.</para>
            /// \endif
            /// </summary>
            /// <param name="store">
            /// \if KO
            /// <para>처리기가 등록된 저장소입니다.</para>
            /// \endif
            /// \if EN
            /// <para>The store on which the handler is registered.</para>
            /// \endif
            /// </param>
            /// <param name="handler">
            /// \if KO
            /// <para>해제할 상태 변경 처리기입니다.</para>
            /// \endif
            /// \if EN
            /// <para>The state-change handler to remove.</para>
            /// \endif
            /// </param>
            public StateSubscription(
                HybridStateStore<TState> store,
                EventHandler<HybridStateChangedEventArgs<TState>> handler)
            {
                _store = store;
                _handler = handler;
            }

            /// <summary>
            /// \if KO
            /// <para>등록된 처리기를 스레드 안전하게 한 번만 제거합니다.</para>
            /// \endif
            /// \if EN
            /// <para>Thread-safely removes the registered handler at most once.</para>
            /// \endif
            /// </summary>
            public void Dispose()
            {
                // Interlocked.Exchange ensures the handler is cleared and unsubscribed
                // exactly once even when Dispose is called concurrently.
                var handler = Interlocked.Exchange(ref _handler, null);
                if (handler is null)
                {
                    return;
                }

                _store.StateChanged -= handler;
            }
        }
    }
}
