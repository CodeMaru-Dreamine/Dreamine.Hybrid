using System;

namespace Dreamine.Hybrid.Interfaces
{
    /// <summary>
        /// \if KO
        /// <para>하이브리드 애플리케이션 계층 사이에서 교환할 수 있는 메시지를 나타냅니다.</para>
        /// \endif
        /// \if EN
        /// <para>Represents a message that can be exchanged between hybrid application layers.</para>
        /// \endif
    /// </summary>
    public interface IHybridMessage
    {
        /// <summary>
        /// \if KO
        /// <para>고유 메시지 식별자를 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets the unique message identifier.</para>
        /// \endif
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// \if KO
        /// <para>메시지가 생성된 시각을 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets the time when the message was created.</para>
        /// \endif
        /// </summary>
        DateTimeOffset CreatedAt { get; }
    }
}
