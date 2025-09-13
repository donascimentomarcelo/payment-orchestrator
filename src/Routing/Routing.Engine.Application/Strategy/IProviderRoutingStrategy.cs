using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routing.Engine.Application.Strategy
{
    public interface IProviderRoutingStrategy
    {
        string ChooseProvider(decimal amount, string currency);
    }
}
