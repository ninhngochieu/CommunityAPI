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


            var unitOfWork = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();

            Debug.Assert(unitOfWork != null, nameof(unitOfWork) + " != null");
            // var user = await unitOfWork.UserRepository.GetUserAsync(userName);

            var user = await unitOfWork.UserRepository.GetUserByIdAsync(userId);
            
            if (user is null) return;
            
            user.LastActive = DateTime.Now;

            await unitOfWork.Complete();
        }
    }
}