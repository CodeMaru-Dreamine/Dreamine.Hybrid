/// \file CounterChangedMessage.cs
/// \brief 카운트 변경 통지 메시지(샘플).
/// \author Dreamine
/// \date 2026-01-28
/// \version 1.0.0
namespace Dreamine.Hybrid.Messaging
{
    /// <summary>카운트가 변경되었음을 알리는 메시지입니다.</summary>
    public sealed class CounterChangedMessage : IHybridMessage
    {
        /// <summary>변경된 카운트 값입니다.</summary>
        public int Count { get; }

        /// <summary>메시지를 생성합니다.</summary>
        public CounterChangedMessage(int count) => Count = count;
    }
}
