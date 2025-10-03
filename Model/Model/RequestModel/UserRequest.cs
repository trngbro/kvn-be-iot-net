using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.RequestModel
{
    public class UserRequest
    {
        public string? UserNameOrEmail { get; set; }
        public string? Password { get; set; }
    }
}