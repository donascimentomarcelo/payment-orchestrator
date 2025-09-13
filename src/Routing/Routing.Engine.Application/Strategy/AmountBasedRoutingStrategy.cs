using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routing.Engine.Application.Strategy
{
    public class AmountBasedRoutingStrategy : IProviderRoutingStrategy
    {
        public string ChooseProvider(decimal amount, string currency)
        {
            return amount <= 300m ? "provider-fake" : "stripe";
        }
    }
}
