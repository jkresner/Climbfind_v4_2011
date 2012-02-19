using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Types;
using cf.Entities.Enum;

namespace cf.Entities.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// </remarks>
    public interface IHasCfType
    {
        CfType Type { get; }
    }
}
