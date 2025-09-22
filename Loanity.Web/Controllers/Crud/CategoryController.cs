using Loanity.Domain.Dtos.CategoryType;
using Loanity.Web.Controllers.Common; // << This is important

namespace Loanity.Web.Controllers;

public class CategoryController : CrudControllerWeb<CategoryDto>
{
    public CategoryController(IHttpClientFactory factory)
        : base(factory, "categories") // 'categories' matches your API route: [Route("api/categories")]
    {
    }
}
