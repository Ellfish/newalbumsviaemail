using Microsoft.AspNetCore.Rewrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewAlbums.Web.Rules
{
    /// <summary>
    /// From: https://ryanwilliams.io/articles/2017/10/06/redirecting-www-and-non-https-traffic-with-asp-net-core-2-0
    /// </summary>
    public class NonWwwRule : IRule
    {
        public void ApplyRule(RewriteContext context)
        {
            var req = context.HttpContext.Request;
            var currentHost = req.Host;

            if (currentHost.Value.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
            {
                var newHost = currentHost.Value.Substring(4);   //Remove www.
                var newUrl = new StringBuilder().Append("https://").Append(newHost).Append(req.PathBase).Append(req.Path).Append(req.QueryString);
                context.HttpContext.Response.Redirect(newUrl.ToString(), true);
                context.Result = RuleResult.EndResponse;
            }
        }
    }
}
