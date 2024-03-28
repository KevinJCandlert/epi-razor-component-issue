using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Namespace
{
    public class ArticlePageModel : RazorPageModel<ArticlePage>
    {
        public void OnGet()
        {
        }
    }
}
