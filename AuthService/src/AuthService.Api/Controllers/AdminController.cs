using Cayd.AspNetCore.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthService.Api.Utilities;
using AuthService.Application.Features.Commands.Admin.DeleteUser;
using AuthService.Application.Features.Queries.Admin.GetUser;
using AuthService.Application.Features.Queries.Admin.GetUsers;
using AuthService.Application.Mappings.Controllers.Admin;
using AuthService.Application.Policies;

namespace AuthService.Api.Controllers
{
    [Authorize(Roles = AdminPolicy.RoleName)]
    [ApiController]
    [Route("admin")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] int? page, [FromQuery] int? pageSize, CancellationToken cancellationToken)
        {
            var result = await _mediator.SendAsync(new GetUsersRequest()
            {
                Page = page,
                PageSize = pageSize
            }, cancellationToken);

            return result.Match(
                (code, response, metadata) => JsonUtility.Success(code, AdminMappings.Map(response), metadata),
                (code, errors, metadata) => JsonUtility.Fail(code, errors, metadata)
            );
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUser(Guid? id, CancellationToken cancellationToken)
        {
            var result = await _mediator.SendAsync(new GetUserRequest()
            {
                Id = id
            }, cancellationToken);

            return result.Match(
                (code, response, metadata) => JsonUtility.Success(code, AdminMappings.Map(response), metadata),
                (code, errors, metadata) => JsonUtility.Fail(code, errors, metadata)
            );
        }

        [HttpPost("user/soft-delete")]
        public async Task<IActionResult> DeleteUser(DeleteUserRequest request)
        {
            var result = await _mediator.SendAsync(request);
            return result.Match(
                (code, response, metadata) => JsonUtility.Success(code, metadata),
                (code, errors, metadata) => JsonUtility.Fail(code, errors, metadata)
            );
        }
    }
}
