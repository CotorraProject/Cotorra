using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Cotorra.Core
{
    public static class RollBackExtensions
    {

        public static void DoRollBack(this Stack<Action> rollbackActions)
        {
            if (rollbackActions != null && rollbackActions.Any())
            {
                while (rollbackActions.Any())
                {
                    try
                    {
                        rollbackActions.Pop()();
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(ex.ToString());
                    }
                }
            }
        }

    }
}
