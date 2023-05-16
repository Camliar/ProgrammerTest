using Microsoft.AspNetCore.Mvc;
using ProgrammerTest.Order.WebApi.Common;
using System.Net;

namespace ProgrammerTest.Order.WebApi.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        protected Expression<Func<TEntity, object>>[] UpdatingProps<TEntity>(params Expression<Func<TEntity, object>>[] expressions) => expressions;
    }

}
