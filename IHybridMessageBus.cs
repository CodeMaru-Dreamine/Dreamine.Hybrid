/// \file IHybridMessageBus.cs
/// \brief 하이브리드 메시지 버스 인터페이스.
/// \author Dreamine
/// \date 2026-01-28
/// \version 1.0.0
using System;
namespace Dreamine.Hybrid.Messaging
{
    /// <summary>WPF/Blazor 간 메시지 전달을 담당하는 버스 인터페이스입니다.</summary>
    public interface IHybridMessageBus
    {
        /// <summary>메시지를 발행합니다.</summary>
        void Publish<TMessage>(TMessage message) where TMessage : IHybridMessage;

        /// <summary>특정 메시지를 구독합니다.</summary>
        IDisposable Subscribe<TMessage>(Action<TMessage> handler) where TMessage : IHybridMessage;
    }
}
