using Application.Common;
using Application.Dtos;
using Application.Services.Validaciones;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IAppMessage
        where TResponse : IResponse
    {
        private readonly IValidatorService validatorService;

        public ValidationBehavior(IValidatorService validatorService)
        {
            this.validatorService = validatorService;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            validatorService.AplicarValidador(request);
            return await next();
        }
    }
}
