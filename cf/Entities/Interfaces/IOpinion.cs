using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Types;

namespace cf.Entities.Interfaces
{
    /// <summary>
    /// Interface to bridge the gap between Opinions & MediaOpinions
    /// </summary>
    public interface IOpinion
    {
        Guid ID { get; set; }
        Guid UserID { get; set; }
        DateTime Utc { get; set; }
        byte Rating { get; set; }
        string Comment { get; set; }
    }
}
