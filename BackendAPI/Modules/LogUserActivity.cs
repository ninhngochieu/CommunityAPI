using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BackendAPI.Extentions;
using BackendAPI.Repository.Interface;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace BackendAPI.Modules
{
    public class LogUserActivity: IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (resultContext.HttpContext.User.Identity is {IsAuthenticated: false}) return;
            
            // var userName = resultContext.HttpContext.User.GetUserName();

            var userId = resultContext.HttpContext.User.GetUserId();
            
            
            var userRepository = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();

            Debug.Assert(userRepository != null, nameof(userRepository) + " != null");
            // var user = await userRepository.GetUserAsync(userName);

            var user = await userRepository.GetUserByIdAsync(userId);
            
            if (user is null) return;
            
            user.LastActive = DateTime.Now;

            await userRepository.SaveAllAsync();
        }
    }
}