using System;
using System.Threading;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Commands.UserCommands
{
    public class CreateAdminOrManagerCommandHandler
        : UserRequestHandlerBase,
          IRequestHandler<CreateAdminOrManagerCommand>
    {
        public CreateAdminOrManagerCommandHandler(UserManager<ApplicationUser> userManager)
            : base(userManager) { }

        public async Task<Unit> Handle(
            CreateAdminOrManagerCommand request,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user is not null)
            {
                throw new ConflictException("The user is already existed.");
            }
            #endregion

            user = new ApplicationUser { UserName = request.UserName };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new DatabaseUpdateException(result.ToString());
            }
            result = await _userManager.AddToRoleAsync(user, request.Role);
            if (!result.Succeeded)
            {
                throw new DatabaseUpdateException(result.ToString());
            }
            return Unit.Value;
        }
    }
}
