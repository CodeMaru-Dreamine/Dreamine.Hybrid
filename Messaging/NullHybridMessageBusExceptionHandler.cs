using Dreamine.Hybrid.Interfaces;
using System;

namespace Dreamine.Hybrid.Messaging
{
    /// <summary>
    /// \if KO
    /// <para><see cref="IHybridMessageBusExceptionHandler"/>의 무동작 Null Object 구현입니다. 별도 처리기를 구성하지 않았을 때 기본값으로 사용됩니다.</para>
    /// \endif
    /// \if EN
    /// <para>Provides a no-op null-object implementation of <see cref="IHybridMessageBusExceptionHandler"/> used when no handler is configured.</para>
    /// \endif
    /// </summary>
    public sealed class NullHybridMessageBusExceptionHandler : IHybridMessageBusExceptionHandler
    {
        /// <summary>
        /// \if KO
        /// <para>공유 싱글턴 인스턴스를 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets the shared singleton instance.</para>
        /// \endif
        /// </summary>
        public static readonly NullHybridMessageBusExceptionHandler Instance = new();

        /// <summary>
        /// \if KO
        /// <para>외부 인스턴스 생성을 막고 싱글턴 사용을 강제합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Prevents external construction and enforces use of the singleton.</para>
        /// \endif
        /// </summary>
        private NullHybridMessageBusExceptionHandler() { }

        /// <summary>
        /// \if KO
        /// <para>구독자 예외를 의도적으로 무시합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Intentionally ignores a subscriber exception.</para>
        /// \endif
        /// </summary>
        /// <param name="exception">
        /// \if KO
        /// <para>무시할 구독자 예외입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The subscriber exception to ignore.</para>
        /// \endif
        /// </param>
        /// <param name="messageType">
        /// \if KO
        /// <para>게시 중이던 메시지 형식입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The type of message being published.</para>
        /// \endif
        /// </param>
        public void Handle(Exception exception, Type messageType) { }
    }
}
