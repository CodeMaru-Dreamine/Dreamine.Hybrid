using Dreamine.Hybrid.State;
using System;

namespace Dreamine.Hybrid.Interfaces
{
    /// <summary>
        /// \if KO
        /// <para>하이브리드 WPF 및 Blazor 컨텍스트가 공유하는 상태 컨테이너를 정의합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Defines a state container shared by hybrid WPF and Blazor contexts.</para>
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
    public interface IHybridStateStore<TState>
    {
        /// <summary>
        /// \if KO
        /// <para>저장된 상태가 변경되었을 때 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Occurs when the stored state changes.</para>
        /// \endif
        /// </summary>
        event EventHandler<HybridStateChangedEventArgs<TState>>? StateChanged;

        /// <summary>
        /// \if KO
        /// <para>상태 변경 처리기를 구독하고 해제 가능한 구독 핸들을 반환합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Subscribes a state-change handler and returns a disposable subscription handle.</para>
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
        IDisposable Subscribe(EventHandler<HybridStateChangedEventArgs<TState>> handler);

        /// <summary>
        /// \if KO
        /// <para>현재 상태의 스레드 안전 스냅숏을 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets a thread-safe snapshot of the current state.</para>
        /// \endif
        /// </summary>
        TState State { get; }

        /// <summary>
        /// \if KO
        /// <para>현재 상태를 지정한 값으로 교체합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Replaces the current state with the specified value.</para>
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
        void SetState(TState state);

        /// <summary>
        /// \if KO
        /// <para>지정한 변환 함수를 사용하여 현재 상태를 원자적으로 갱신합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Atomically updates the current state using the specified transformation function.</para>
        /// \endif
        /// </summary>
        /// <param name="updater">
        /// \if KO
        /// <para>현재 상태를 새 상태로 변환하는 함수입니다.</para>
        /// \endif
        /// \if EN
        /// <para>A function that transforms the current state into a new state.</para>
        /// \endif
        /// </param>
        void Update(Func<TState, TState> updater);
    }
}
