using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.ResponseModel
{
    public class UserResponse
    {
        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? FullName { get; set; }

        public string? ExpiredTime { get; set; }
    }

    public class UserLoginResponse
    {
        public Guid? APIKey { get; set; }

        public string? ExpiredTime { get; set; }

        public bool? IsRefresh { get; set; } = false;
    }
}