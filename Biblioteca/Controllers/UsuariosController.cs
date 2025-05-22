using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Data;
using Biblioteca.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Biblioteca.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UsuariosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            var rolesPorUsuario = new Dictionary<int, string>();

            foreach (var usuario in usuarios)
            {
                if (usuario.AppUserId.HasValue)
                {
                    var identityUser = await _userManager.FindByIdAsync(usuario.AppUserId.ToString());
                    if (identityUser != null)
                    {
                        var roles = await _userManager.GetRolesAsync(identityUser);
                        rolesPorUsuario[usuario.UsuarioId] = roles.FirstOrDefault() ?? "Nenhuma";
                    }
                    else
                    {
                        rolesPorUsuario[usuario.UsuarioId] = "Nenhuma";
                    }
                }
                else
                {
                    rolesPorUsuario[usuario.UsuarioId] = "Nenhuma";
                }
            }

            ViewBag.RolesPorUsuario = rolesPorUsuario;
            return View(usuarios);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Buscar(string searchTerm)
        {
            var usuarios = string.IsNullOrWhiteSpace(searchTerm)
                ? await _context.Usuarios.ToListAsync()
                : await _context.Usuarios
                    .Where(u => u.NomeCompleto.Contains(searchTerm) || u.CPF.Contains(searchTerm))
                    .ToListAsync();

            // Monta o dicionário de roles igual ao Index
            var rolesPorUsuario = new Dictionary<int, string>();
            foreach (var usuario in usuarios)
            {
                if (usuario.AppUserId.HasValue)
                {
                    var identityUser = await _userManager.FindByIdAsync(usuario.AppUserId.ToString());
                    if (identityUser != null)
                    {
                        var roles = await _userManager.GetRolesAsync(identityUser);
                        rolesPorUsuario[usuario.UsuarioId] = roles.FirstOrDefault() ?? "Nenhuma";
                    }
                    else
                    {
                        rolesPorUsuario[usuario.UsuarioId] = "Nenhuma";
                    }
                }
                else
                {
                    rolesPorUsuario[usuario.UsuarioId] = "Nenhuma";
                }
            }
            ViewBag.RolesPorUsuario = rolesPorUsuario;

            return View("Index", usuarios);
        }



        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsuarioId,NomeCompleto,CPF,Celular,DataNascimento,UrlFoto,AppUserId")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return NotFound();

                var existingUser = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.AppUserId == Guid.Parse(userId));
                if (existingUser != null)
                {
                    ModelState.AddModelError("AppUserId", "E-mail já cadastrado.");
                    return View(usuario);
                }

                usuario.AppUserId = Guid.Parse(userId);

                var identityUser = await _context.Users.FindAsync(userId);

                if (identityUser != null)
                {
                    usuario.IdentityUser = identityUser;
                }
                else
                {
                    ModelState.AddModelError("AppUserId", "Usuário não encontrado.");
                    return View(usuario);
                }
                _context.Add(usuario);
                await _context.SaveChangesAsync();

                // Adiciona o usuário à role "Aluno"
                await _userManager.AddToRoleAsync(identityUser, "Aluno");

                return RedirectToAction("Index", "Home");
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.AppUserId.ToString() == userId);
            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioId,NomeCompleto,CPF,Celular,DataNascimento,UrlFoto,AppUserId")] Usuario usuario)
        {
            if (id != usuario.UsuarioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.UsuarioId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PromoverParaAdmin(int id)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == id);
            if (usuario == null)
                return NotFound();

            var identityUser = await _userManager.FindByIdAsync(usuario.AppUserId.ToString());
            if (identityUser == null)
                return NotFound();

            // Adiciona à role Admin
            if (!await _userManager.IsInRoleAsync(identityUser, "Admin"))
            {
                await _userManager.AddToRoleAsync(identityUser, "Admin");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoverAdmin(int id)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == id);
            if (usuario == null)
                return NotFound();

            var identityUser = await _userManager.FindByIdAsync(usuario.AppUserId.ToString());
            if (identityUser == null)
                return NotFound();

            // Remove da role Admin se estiver nela
            if (await _userManager.IsInRoleAsync(identityUser, "Admin"))
            {
                await _userManager.RemoveFromRoleAsync(identityUser, "Admin");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
