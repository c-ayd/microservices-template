using AdminService.Api.Utilities;
using AdminService.Application.Features.Commands.DeleteUser;
using AdminService.Application.Features.Queries.GetUser;
using AdminService.Application.Features.Queries.GetUsers;
using AdminService.Application.Policies;
using Cayd.AspNetCore.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminService.Api.Controllers
{
    [ApiController]
    [Route("admin")]
    [Authorize(Roles = AdminPolicy.RoleName)]
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
                (code, response, metadata) => JsonUtility.Success(code,
                new 
                { 
                    response.Users 
                },
                new
                {
                    Page = page,
                    pageSize = pageSize,
                    response.NumberOfNextPages
                }),
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
                (code, response, metadata) => JsonUtility.Success(code, response, metadata),
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
