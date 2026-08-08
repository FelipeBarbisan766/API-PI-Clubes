using API_PI_Clubes.Application.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API_PI_Clubes.Hubs
{
    [AllowAnonymous]
    public class CourtAvailabilityHub : Hub<ICourtAvailabilityHubClient>
    {
        public async Task JoinClubGroup(Guid clubId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(clubId));
        }
        public async Task LeaveClubGroup(Guid clubId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(clubId));
        }
        private static string GroupName(Guid clubId) => $"club-{clubId}";
    }
}