/// \file DashboardAction.cs
/// \brief 대시보드 버튼 동작 구분 enum.
/// \author Dreamine
/// \date 2026-01-28
/// \version 1.0.0
namespace Dreamine.Hybrid.Messaging
{
    /// <summary>Blazor 대시보드에서 요청 가능한 액션 종류입니다.</summary>
    public enum DashboardAction
    {
        /// <summary>프로젝트 열기</summary>
        OpenProject = 0,
        /// <summary>NuGet 관리</summary>
        OpenNuget = 1,
        /// <summary>문서 열기</summary>
        OpenDocs = 2,
        /// <summary>설정 열기</summary>
        OpenSettings = 3,
    }
}
