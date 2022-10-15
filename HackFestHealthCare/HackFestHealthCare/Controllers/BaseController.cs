using HackFestHealthCare.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace HackFestHealthCare.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public abstract class BaseController : ControllerBase
    {
        public MyAppUser CurrentUser
        {
            get
            {
                return new MyAppUser(User as ClaimsPrincipal);
            }
        }

        public static string GetModelStateErrors(ModelStateDictionary modelState)
        {
            StringBuilder result = new StringBuilder();
            var err = modelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage);
            foreach (var item in err)
            {
                result.Append(item + Environment.NewLine);
            }
            return result.ToString().Replace(Environment.NewLine, " ");
        }

        public static string GetErrors(ModelStateDictionary modelState)
        {
            StringBuilder result = new StringBuilder();
            var err = modelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage);
            foreach (var item in err)
            {
                result.Append(item + Environment.NewLine);
            }
            return result.ToString();
        }
    }
}
