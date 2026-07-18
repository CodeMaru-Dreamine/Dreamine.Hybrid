using Dreamine.Hybrid.Interfaces;
using System;

namespace Dreamine.Hybrid.Messaging
{
    /// <summary>
        /// \if KO
        /// <para>고유 식별자와 생성 시각을 제공하는 하이브리드 메시지 기본 구현입니다.</para>
        /// \endif
        /// \if EN
        /// <para>Provides a hybrid-message base implementation with a unique identifier and creation time.</para>
        /// \endif
    /// </summary>
    public abstract class HybridMessageBase : IHybridMessage
    {
        /// <summary>
        /// \if KO
        /// <para>새 식별자와 현재 시각으로 <see cref="HybridMessageBase"/>의 새 인스턴스를 초기화합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Initializes a new <see cref="HybridMessageBase"/> instance with a new identifier and the current time.</para>
        /// \endif
        /// </summary>
        protected HybridMessageBase()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTimeOffset.Now;
        }

        /// <summary>
        /// \if KO
        /// <para>고유 메시지 식별자를 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets the unique message identifier.</para>
        /// \endif
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// \if KO
        /// <para>메시지가 생성된 로컬 시각을 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets the local time at which the message was created.</para>
        /// \endif
        /// </summary>
        public DateTimeOffset CreatedAt { get; }
    }
}
