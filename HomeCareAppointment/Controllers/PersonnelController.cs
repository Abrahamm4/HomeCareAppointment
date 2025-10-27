using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeCareAppointment.DAL;
using HomeCareAppointment.Models;

namespace HomeCareAppointment.Controllers
{
 public class PersonnelController : Controller
 {
 private readonly IPersonnelRepository _personnels;

 public PersonnelController(IPersonnelRepository personnels)
 {
 _personnels = personnels;
 }

 // GET: /Personnel
 public async Task<IActionResult> Index()
 {
 var list = await _personnels.GetAllAsync() ?? Enumerable.Empty<Personnel>();
 return View(list);
 }

 // GET: /Personnel/Details/5
 public async Task<IActionResult> Details(int? id)
 {
 if (id == null) return NotFound();
 var personnel = await _personnels.GetByIdAsync(id.Value);
 if (personnel == null) return NotFound();
 return View(personnel);
 }

 // GET: /Personnel/Create
 public IActionResult Create() => View(new Personnel());

 // POST: /Personnel/Create
 [HttpPost]
 [ValidateAntiForgeryToken]
 public async Task<IActionResult> Create([Bind("Name")] Personnel model)
 {
 if (!ModelState.IsValid) return View(model);
 var ok = await _personnels.CreateAsync(model);
 if (!ok)
 {
 ModelState.AddModelError(string.Empty, "Could not create personnel.");
 return View(model);
 }
 return RedirectToAction(nameof(Index));
 }

 // GET: /Personnel/Edit/5
 public async Task<IActionResult> Edit(int? id)
 {
 if (id == null) return NotFound();
 var personnel = await _personnels.GetByIdAsync(id.Value);
 if (personnel == null) return NotFound();
 return View(personnel);
 }

 // POST: /Personnel/Edit/5
 [HttpPost]
 [ValidateAntiForgeryToken]
 public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Personnel model)
 {
 if (id != model.Id) return NotFound();
 if (!ModelState.IsValid) return View(model);
 var ok = await _personnels.UpdateAsync(model);
 if (!ok)
 {
 ModelState.AddModelError(string.Empty, "Could not update personnel.");
 return View(model);
 }
 return RedirectToAction(nameof(Index));
 }

 // GET: /Personnel/Delete/5
 public async Task<IActionResult> Delete(int? id)
 {
 if (id == null) return NotFound();
 var personnel = await _personnels.GetByIdAsync(id.Value);
 if (personnel == null) return NotFound();
 return View(personnel);
 }

 // POST: /Personnel/Delete/5
 [HttpPost, ActionName("Delete")]
 [ValidateAntiForgeryToken]
 public async Task<IActionResult> DeleteConfirmed(int id)
 {
 var ok = await _personnels.DeleteAsync(id);
 if (!ok)
 {
 var entity = await _personnels.GetByIdAsync(id);
 ModelState.AddModelError(string.Empty, "Could not delete personnel.");
 return View("Delete", entity);
 }
 return RedirectToAction(nameof(Index));
 }
 }
}
