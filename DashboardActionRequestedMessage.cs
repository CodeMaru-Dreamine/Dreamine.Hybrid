/// \file DashboardActionRequestedMessage.cs
/// \brief Blazor 대시보드에서 WPF 쉘로 보내는 액션 요청 메시지.
/// \author Dreamine
/// \date 2026-01-28
/// \version 1.0.0
namespace Dreamine.Hybrid.Messaging
{
    /// <summary>Blazor 대시보드가 WPF 쉘에 동작을 요청할 때 사용하는 메시지입니다.</summary>
    public sealed class DashboardActionRequestedMessage : IHybridMessage
    {
        /// <summary>요청 액션 종류입니다.</summary>
        public DashboardAction Action { get; }

        /// <summary>메시지를 생성합니다.</summary>
        public DashboardActionRequestedMessage(DashboardAction action) => Action = action;
    }
}
