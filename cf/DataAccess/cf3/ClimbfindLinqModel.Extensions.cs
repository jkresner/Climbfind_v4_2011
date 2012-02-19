using System;
using cf.Entities.Interfaces;

namespace cf.DataAccess.cf3
{
    public partial class ClimbfindLinqModelDataContext : IDATransactionContext { }

    public partial class ClimberProfile : IKeyObject<Guid> { }

    public partial class ClimberProfileExtended : IKeyObject<Guid> { }

    public partial class ClimberProfilePartnerStatus : IKeyObject<byte> { }
}
