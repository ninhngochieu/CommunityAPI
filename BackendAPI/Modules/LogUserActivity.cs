using System;
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
            var actionExecutedContext = await next();
            if(actionExecutedContext.HttpContext.User.Identity is {IsAuthenticated: false}) return;

            var userName = actionExecutedContext.HttpContext.User.GetUserName();

            var userRepository = actionExecutedContext.HttpContext.RequestServices.GetService<IUserRepository>();

            if (userRepository != null)
            {
                var userAsync = await userRepository.GetUserAsync(userName);
            
                userAsync.LastActive = DateTime.Now;

                await userRepository.SaveAllAsync();
            }
            
            
        }
    }
}