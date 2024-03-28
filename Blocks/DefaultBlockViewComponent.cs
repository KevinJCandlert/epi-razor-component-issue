using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

[TemplateDescriptor(Inherited = true)]
public class DefaultBlockViewComponent : BlockComponent<BlockData>
{
    protected override IViewComponentResult InvokeComponent(BlockData currentContent)
    {
        var blockName = currentContent.GetOriginalType().Name;
        var blockFolder = blockName.Remove(blockName.IndexOf("Block"));
        return View($"~/Blocks/{blockFolder}/{blockName}.cshtml", currentContent);	
    }
}
