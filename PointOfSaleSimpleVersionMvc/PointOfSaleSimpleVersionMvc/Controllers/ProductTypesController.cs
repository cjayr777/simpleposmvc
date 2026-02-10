using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using PointOfSaleSimpleVersionMvc.ViewHelpers;
using Pos.BusinessLogic;
using Pos.ViewModel;
using Proj.Util;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace PointOfSaleSimpleVersionMvc.Controllers;

public class ProductTypesController : Controller
{
    public ProductTypesController(ProductTypeBizz p_biz, IdProtector<ProductTypeVm> p_ip)
    {
        biz = p_biz;
        ip = p_ip;
    }


    private ProductTypeBizz biz;
    private IdProtector<ProductTypeVm> ip;


    [BindProperty(SupportsGet = true)]
    public ProductTypeVm Entity { get; set; }


    private int GetLoggedinUser()
    {
        return 1;
    }

    private void ProtectIds(IEnumerable<ProductTypeVm>? items)
    {
        if (items == null) return;

        foreach (var item in items)
        {
            item.ProtectedId = ip.Protect(item.ProductTypeId);
        }
    }

    private PagedResult<ProductTypeVm> ToPaged(
    List<ProductTypeVm> items,
    int page,
    int pageSize)
    {
        var pagedItems = items
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ProtectIds(pagedItems);

        return new PagedResult<ProductTypeVm>
        {
            PageNumber = page,
            PageSize = pageSize,
            TotalItems = items.Count,
            Items = pagedItems
        };
    }


    public async Task<IActionResult> SearchIndex(int page = 1)
    {
        const int pageSize = 5;

        Entity.Search ??= "";

        var result = await biz.GetAsync(Entity, SortMode.asc);

        if (!result.IsSuccess || result.Data == null)
        {
            TempData[TempDataKey.Error] = result.Message;
            return View(GenPageName.Index, new PagedResult<ProductTypeVm>());
        }

        var paged = ToPaged(result.Data, page, pageSize);

        return View(GenPageName.Index, paged);
    }

    // GET: ProductTypesController
    public async Task<IActionResult> Index(int page = 1)
    {
        const int pageSize = 5;

        var result = await biz.GetAsync(new ProductTypeVm());

        if (!result.IsSuccess || result.Data == null)
        {
            TempData[TempDataKey.Error] = result.Message;
            return View(new PagedResult<ProductTypeVm>());
        }

        var paged = ToPaged(result.Data, page, pageSize);

        return View(paged);
    }

    // GET: ProductTypesController/Details/5
    public async Task<ActionResult> Details(string id)
    {
        try
        {
            Entity.ProductTypeId = ip.Unprotect(id);
        }
        catch (Exception ex)
        {
            return NotFound();
        }

        var dtVm = await biz.GetOneAsync(Entity);

        if (!dtVm.IsSuccess || dtVm.Data is null)
        {
            TempData[TempDataKey.Error] = dtVm.Message;
            return View(GenPageName.Details, new ProductTypeVm());
        }

        dtVm.Data.ProtectedId = id;

        return View(dtVm.Data);
    }

    // GET: ProductTypesController/Create
    public async Task<ActionResult> Create()
    {
        Entity = new ProductTypeVm();
        ModelState.Clear();
        return View(GenPageName.Create, Entity);
    }

    // POST: ProductTypesController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Insert()
    {
        try
        {
            if (Entity is null)
            {
                ModelState.AddModelError("", "Data is empty.");

                return View(GenPageName.Create, Entity);
            }

            Entity.ActionerId = GetLoggedinUser();
            var opResult = await biz.AddAsync(Entity);

            if (!opResult.IsSuccess)
            {
                TempData[TempDataKey.Error] = opResult.Message;
                return View(GenPageName.Create, Entity);
            }
            
            TempData[TempDataKey.Success] = $"[{Entity.Name}] was added successfully.";
            return RedirectToAction(nameof(Index));

        }
        catch (Exception ex)
        {
            TempData[TempDataKey.Error] = ex.Message;
            return View(GenPageName.Create, Entity);
        }
    }

    // GET: ProductTypesController/Edit/5
    public async Task<ActionResult> Edit(string id)
    {
        try
        {
            Entity.ProductTypeId = ip.Unprotect(id);
        }
        catch
        {
            return NotFound();
        }

        var dtVm = await biz.GetOneAsync(Entity);

        if (!dtVm.IsSuccess || dtVm.Data == null) return NotFound();

        ModelState.Clear();
        Entity = dtVm.Data;
        Entity.ProtectedId = id;

        return View(Entity);
    }

    // POST: ProductTypesController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(ProductTypeVm model)
    {
        if (!ModelState.IsValid)
        {
            return View(GenPageName.Edit, model);
        }

        model.ActionerId = GetLoggedinUser();
        model.ProductTypeId = ip.Unprotect(model.ProtectedId);

        var result = await biz.EditAsync(model);

        if (result.IsSuccess)
        {
            TempData[TempDataKey.Success] = $"[{model.Name}] was updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, result.Message);
        return View(GenPageName.Edit, model);
    }

    // GET: ProductTypesController/Delete/5
    public async Task<ActionResult> Delete(string id)
    {
        try
        {
            Entity.ProductTypeId = ip.Unprotect(id);
        }
        catch (Exception ex)
        {
            return NotFound();
        }

        var dtVm = await biz.GetOneAsync(Entity);

        if (!dtVm.IsSuccess || dtVm.Data is null)
        {
            TempData[TempDataKey.Error] = dtVm.Message;
            return View(GenPageName.Delete, new ProductTypeVm());
        }

        Entity = dtVm.Data;
        Entity.ProtectedId = id;

        return View(Entity);
    }

    // POST: ProductTypesController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Erase(ProductTypeVm model)
    {
        if (!ModelState.IsValid)
        {
            return View(GenPageName.Delete, model);
        }

        model.ActionerId = GetLoggedinUser();
        model.ProductTypeId = ip.Unprotect(model.ProtectedId);

        var result = await biz.DeleteAsync(model);

        if (result.IsSuccess)
        {
            TempData[TempDataKey.Success] = $"[{model.Name}] has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, result.Message);
        return View(GenPageName.Delete, model);
    }

}
