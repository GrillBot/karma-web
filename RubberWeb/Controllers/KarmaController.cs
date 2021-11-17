using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RubberWeb.Models;
using RubberWeb.Models.Karma;
using RubberWeb.Services;

namespace RubberWeb.Controllers
{
    [Route("api/karma")]
    [ApiController]
    public class KarmaController : Controller
    {
        private UserService UserService { get; }
        private AppDbContext Context { get; }

        public KarmaController(AppDbContext context, UserService userService)
        {
            Context = context;
            UserService = userService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedData<KarmaItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetData([FromQuery] PaginatedRequest request)
        {
            var query = Context.Karma.AsQueryable();
            if (request.Sort == PageSort.Asc) query = query.OrderBy(o => o.Karma);
            else query = query.OrderByDescending(o => o.Karma);
            var data = PaginatedData<KarmaItem>.Create(query, request, entity => new KarmaItem(entity));

            var position = PaginationHelper.CountSkipValue(request) + 1;
            foreach (var item in data.Data)
            {
                var user = await UserService.GetUserAsync(item.UserID);

                if (user == null)
                {
                    item.User = SimpleUserInfo.DefaultUser;
                    item.User.ID = item.UserID;
                }
                else
                {
                    item.User = new SimpleUserInfo()
                    {
                        AvatarUrl = user.GetAvatarUrl(Discord.ImageFormat.Auto, 128) ?? user.GetDefaultAvatarUrl(),
                        Discriminator = user.Discriminator,
                        ID = user.Id,
                        Name = user.Username
                    };
                }

                item.Position = position;
                position++;
            }

            data.Data = data.Data.ToList();
            return Ok(data);
        }
    }
}
