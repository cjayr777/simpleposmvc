using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSimpleVersionMvc.ViewHelpers;
using Pos.BusinessLogic;
using Pos.ViewModel;
using Proj.Util;
using System.Threading.Tasks;

namespace PointOfSaleSimpleVersionMvc.Controllers;

public class RolesController : Controller
{

    public RolesController(RoleBizz p_biz, IdProtector<RoleVm> p_ip)
    {
        biz = p_biz;
        ip = p_ip;
    }


    RoleBizz biz;
    IdProtector<RoleVm> ip;


    [BindProperty(SupportsGet = true)]
    public RoleVm Entity { get; set; }


    private void ProtectIds(IEnumerable<RoleVm>? items) 
    {
        if (items == null) return;

        foreach (var item in items) 
        {
            item.ProtectedId = ip.Protect(item.RoleId);
        }
    }


    private PagedResult<RoleVm> ToPaged(List<RoleVm> items, int page, int pageSize)
    {
        var pagedItems = items
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ProtectIds(pagedItems);

        return new PagedResult<RoleVm> 
        {
            PageNumber = page,
            PageSize = pageSize,
            TotalItems = items.Count,
            Items = pagedItems
        };
    }


    public async Task<IActionResult> SearhIndex(int page  = 1)
    {
        const int pageSize = 5;

        Entity.Search ??= "";

        var result = await biz.GetAsync(Entity, SortMode.asc);

        if (!result.IsSuccess || result.Data == null)
        {
            TempData[TempDataKey.Error] = result.Message;
            return View(GenPageName.Index, new PagedResult<RoleVm>());
        }

        var paged = ToPaged(result.Data, page, pageSize);

        return View(GenPageName.Index , paged);
    }


    // GET: RolesController
    public async Task<ActionResult> Index(int page = 1)
    {
        const int pageSize = 5;

        var result = await biz.GetAsync(new RoleVm());

        if (!result.IsSuccess || result.Data == null)
        {
            TempData[TempDataKey.Error] = result.Message;
            return View(new PagedResult<RoleVm>());
        }

        var paged = ToPaged(result.Data, page, pageSize);

        return View();
    }

    // GET: RolesController/Details/5
    public ActionResult Details(int id)
    {
        return View();
    }

    // GET: RolesController/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: RolesController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(IFormCollection collection)
    {
        try
        {
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    // GET: RolesController/Edit/5
    public ActionResult Edit(int id)
    {
        return View();
    }

    // POST: RolesController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, IFormCollection collection)
    {
        try
        {
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    // GET: RolesController/Delete/5
    public ActionResult Delete(int id)
    {
        return View();
    }

    // POST: RolesController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, IFormCollection collection)
    {
        try
        {
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }
}
