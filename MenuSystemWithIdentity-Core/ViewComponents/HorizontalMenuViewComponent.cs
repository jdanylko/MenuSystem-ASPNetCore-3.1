using MenuSystemWithIdentity_Core.Services;
using MenuSystemWithIdentity_Core.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MenuSystemWithIdentity_Core.ViewComponents
{
    public class HorizontalMenuViewComponent: ViewComponent
    {
        private readonly IMenuService _service;

        public HorizontalMenuViewComponent(IMenuService service)
        {
            _service = service;
        }

        public IViewComponentResult Invoke()
        {
            var menuViewModel = new MenuViewModel
            {
                MenuItems = _service.GetMenuByUser(User)
            };

            return View(menuViewModel);
        }
    }
}