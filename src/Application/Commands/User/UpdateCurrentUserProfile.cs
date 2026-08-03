using Application.Common;

namespace Application.Commands.AppUser
{
    public class UpdateCurrentUserProfile : IAppMessage
    {
        public int? ProfileFileId { get; set; }
    }
}
