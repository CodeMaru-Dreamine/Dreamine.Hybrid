using System;

namespace Dreamine.Hybrid.State
{
    /// <summary>
        /// \if KO
        /// <para>하이브리드 공유 상태 변경 이벤트 데이터를 제공합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Provides event data for hybrid shared-state changes.</para>
        /// \endif
    /// </summary>
        /// <typeparam name="TState">
        /// \if KO
        /// <para>변경된 상태 형식입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The changed-state type.</para>
        /// \endif
        /// </typeparam>
    public sealed class HybridStateChangedEventArgs<TState> : EventArgs
    {
        /// <summary>
        /// \if KO
        /// <para>변경된 상태로 <see cref="HybridStateChangedEventArgs{TState}"/>의 새 인스턴스를 초기화합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Initializes a new <see cref="HybridStateChangedEventArgs{TState}"/> instance with the changed state.</para>
        /// \endif
        /// </summary>
        /// <param name="state">
        /// \if KO
        /// <para>구독자에게 전달할 변경된 상태입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The changed state to deliver to subscribers.</para>
        /// \endif
        /// </param>
        public HybridStateChangedEventArgs(TState state)
        {
            State = state;
        }

        /// <summary>
        /// \if KO
        /// <para>변경된 상태를 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets the changed state.</para>
        /// \endif
        /// </summary>
        public TState State { get; }
    }
}
