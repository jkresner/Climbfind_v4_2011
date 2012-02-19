using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Entities.Interfaces
{
    public interface IUserBasicDetail : IKeyObject<Guid>
    {
        byte CountryID { get; set; }
        string FullName { get; set; }
        string Email { get; set; }
        string DisplayName { get; }
        string Avatar { get; set; }
        bool IsMale { get; set; }
        bool HasAvatar { get; }
    }
}
