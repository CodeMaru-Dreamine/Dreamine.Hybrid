using System;

namespace Dreamine.Hybrid.Interfaces
{
    /// <summary>
    /// \if KO
    /// <para>개별 메시지 버스 구독자가 발생시킨 예외 처리기를 정의합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Defines a handler for exceptions thrown by individual message-bus subscribers.</para>
    /// \endif
    /// </summary>
    /// <remarks>
    /// \if KO
    /// <para>메시지 버스를 특정 로깅 프레임워크에 결합하지 않고 구독자 실패를 관찰하려면 이 인터페이스를 구현하고 <see cref="Dreamine.Hybrid.Messaging.InMemoryHybridMessageBus.ExceptionHandler"/>에 할당합니다. 기본 구현은 아무 작업도 하지 않는 Null Object입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Implement this interface to observe subscriber failures without coupling the message bus to a concrete logging framework, then assign it through <see cref="Dreamine.Hybrid.Messaging.InMemoryHybridMessageBus.ExceptionHandler"/>. The default implementation is a no-op null object.</para>
    /// \endif
    /// </remarks>
    public interface IHybridMessageBusExceptionHandler
    {
        /// <summary>
        /// \if KO
        /// <para>구독자 처리기가 처리되지 않은 예외를 발생시켰을 때 호출됩니다.</para>
        /// \endif
        /// \if EN
        /// <para>Called when a subscriber handler throws an unhandled exception.</para>
        /// \endif
        /// </summary>
        /// <param name="exception">
        /// \if KO
        /// <para>구독자가 발생시킨 예외입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The exception thrown by the subscriber.</para>
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
        void Handle(Exception exception, Type messageType);
    }
}
