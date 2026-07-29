using Application.Common;
using Domain.Service;

namespace Application.Commands.AppUser
{
    public class GetUser: IAppMessage
    {
        public int Id { get; set; }
    }
}
