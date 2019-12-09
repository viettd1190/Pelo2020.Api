using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.UserServices;

namespace Pelo.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected IUserService UserService;

        public BaseController(IUserService userService)
        {
            UserService = userService;
        }

        protected async Task<int> GetUserId()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return 0;

            var res = await UserService.CheckToken(token);

            if (res.IsSuccess)
            {
                if (res.Data != null) return res.Data.Id;

                return 0;
            }

            return 0;
        }

        protected int GetPermissionId()
        {
            if (HttpContext?.Request?.Headers != null)
                if (HttpContext.Request.Headers.TryGetValue("Permission",
                    out var bearHeader))
                    return Convert.ToInt32(bearHeader[0]);

            return 0;
        }

        private string GetToken()
        {
            if (HttpContext?.Request?.Headers != null)
                if (HttpContext.Request.Headers.TryGetValue("Authorization",
                    out var bearHeader))
                    if (bearHeader.Any())
                    {
                        var token = bearHeader[0];
                        if (token.StartsWith("Bearer ")) return token.Substring(7);
                    }

            return string.Empty;
        }
    }
}