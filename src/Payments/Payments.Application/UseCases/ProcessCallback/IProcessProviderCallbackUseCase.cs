using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Messaging;

namespace Payments.Application.UseCases.ProcessCallback
{
    public interface IProcessProviderCallbackUseCase
    {
        Task ExecuteAsync(ProviderCallback callback, CancellationToken ct);
    }
}
