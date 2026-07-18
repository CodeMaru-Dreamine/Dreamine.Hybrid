using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dreamine.Hybrid.Interfaces
{
    /// <summary>
        /// \if KO
        /// <para>하이브리드 애플리케이션 계층 간 통신용 메시지 버스를 정의합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Defines a message bus for communication between hybrid application layers.</para>
        /// \endif
    /// </summary>
    public interface IHybridMessageBus
    {
        /// <summary>
        /// \if KO
        /// <para>지정한 메시지를 등록된 구독자에게 비동기적으로 게시합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Asynchronously publishes the specified message to registered subscribers.</para>
        /// \endif
        /// </summary>
        /// <typeparam name="TMessage">
        /// \if KO
        /// <para>게시할 메시지 형식입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The message type to publish.</para>
        /// \endif
        /// </typeparam>
        /// <param name="message">
        /// \if KO
        /// <para>게시할 메시지 인스턴스입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The message instance to publish.</para>
        /// \endif
        /// </param>
        /// <param name="cancellationToken">
        /// \if KO
        /// <para>게시 작업 취소 토큰입니다.</para>
        /// \endif
        /// \if EN
        /// <para>A token used to cancel publishing.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>비동기 게시 작업입니다.</para>
        /// \endif
        /// \if EN
        /// <para>A task representing the asynchronous publish operation.</para>
        /// \endif
        /// </returns>
        Task PublishAsync<TMessage>(
            TMessage message,
            CancellationToken cancellationToken = default)
            where TMessage : IHybridMessage;

        /// <summary>
        /// \if KO
        /// <para>지정한 메시지 형식에 처리기를 구독시킵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Subscribes a handler to the specified message type.</para>
        /// \endif
        /// </summary>
        /// <typeparam name="TMessage">
        /// \if KO
        /// <para>구독할 메시지 형식입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The message type to subscribe to.</para>
        /// \endif
        /// </typeparam>
        /// <param name="handler">
        /// \if KO
        /// <para>메시지를 비동기적으로 처리할 대리자입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The delegate that asynchronously handles messages.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>해제할 때 구독을 제거하는 핸들입니다.</para>
        /// \endif
        /// \if EN
        /// <para>A handle that removes the subscription when disposed.</para>
        /// \endif
        /// </returns>
        IDisposable Subscribe<TMessage>(
            Func<TMessage, CancellationToken, Task> handler)
            where TMessage : IHybridMessage;
    }
}
