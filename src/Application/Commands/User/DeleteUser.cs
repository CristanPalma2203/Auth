using Application.Common;

namespace Application.Commands.AppUser
{
    public class DeleteUser : IAppMessage
    {
        public int Id { get; set; }
    }
}
