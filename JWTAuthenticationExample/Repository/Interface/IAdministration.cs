using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace JWTAuthenticationExample.Repository.Interface
{
    interface IAdministration
    {
        DataSet ValidateUser(string UserName, string Password);
    }
}
