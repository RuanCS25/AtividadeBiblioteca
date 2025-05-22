using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Data;
using Biblioteca.Models;

namespace Biblioteca.Controllers
{
    public class MovimentacoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovimentacoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Movimentacoes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Movimentacoes.Include(m => m.Livro).Include(m => m.Usuario);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Movimentacoes
        public async Task<IActionResult> Retiradas()
        {
            var reservas = _context.Reservas.Include(m => m.Livro).Include(m => m.Usuario).Where(l => l.Cancelada != true).Where(l => l.LivroRetirado != true);

            var movimentacoesPendentes = await _context.Movimentacoes
            .Include(m => m.Livro)
            .Include(m => m.Usuario)
            .Where(m => !m.LivroDevolvido)
            .ToListAsync();

            ViewBag.MovimentacoesPendentes = movimentacoesPendentes;
            return View(await reservas.ToListAsync());
        }

        // GET: Movimentacoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movimentacao = await _context.Movimentacoes
                .Include(m => m.Livro)
                .Include(m => m.Usuario)
                .FirstOrDefaultAsync(m => m.MovimentacaoId == id);
            if (movimentacao == null)
            {
                return NotFound();
            }

            return View(movimentacao);
        }

        // GET: Movimentacoes/Create
        public IActionResult Create()
        {
            ViewData["LivroId"] = new SelectList(_context.Livros, "LivroId", "LivroId");
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId");
            return View();
        }

        // POST: Movimentacoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovimentacaoId,DataRetirada,DataDevolucao,UsuarioId,LivroId,LivroDevolvido,LivroAtrasado")] Movimentacao movimentacao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movimentacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LivroId"] = new SelectList(_context.Livros, "LivroId", "LivroId", movimentacao.LivroId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", movimentacao.UsuarioId);
            return View(movimentacao);
        }

        // GET: Movimentacoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movimentacao = await _context.Movimentacoes.FindAsync(id);
            if (movimentacao == null)
            {
                return NotFound();
            }
            ViewData["LivroId"] = new SelectList(_context.Livros, "LivroId", "LivroId", movimentacao.LivroId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", movimentacao.UsuarioId);
            return View(movimentacao);
        }

        // POST: Movimentacoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MovimentacaoId,DataRetirada,DataDevolucao,UsuarioId,LivroId,LivroDevolvido,LivroAtrasado")] Movimentacao movimentacao)
        {
            if (id != movimentacao.MovimentacaoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movimentacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovimentacaoExists(movimentacao.MovimentacaoId))
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
            ViewData["LivroId"] = new SelectList(_context.Livros, "LivroId", "LivroId", movimentacao.LivroId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", movimentacao.UsuarioId);
            return View(movimentacao);
        }

        // GET: Movimentacoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movimentacao = await _context.Movimentacoes
                .Include(m => m.Livro)
                .Include(m => m.Usuario)
                .FirstOrDefaultAsync(m => m.MovimentacaoId == id);
            if (movimentacao == null)
            {
                return NotFound();
            }

            return View(movimentacao);
        }

        // POST: Movimentacoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movimentacao = await _context.Movimentacoes.FindAsync(id);
            if (movimentacao != null)
            {
                _context.Movimentacoes.Remove(movimentacao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovimentacaoExists(int id)
        {
            return _context.Movimentacoes.Any(e => e.MovimentacaoId == id);
        }

        [HttpGet]
        public async Task<IActionResult> Busca(string searchTerm)
        {
            var reservasQuery = _context.Reservas
                .Include(r => r.Livro)
                .Include(r => r.Usuario)
                .Where(r => !r.Cancelada && !r.LivroRetirado);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                reservasQuery = reservasQuery.Where(r =>
                    r.Usuario.NomeCompleto.Contains(searchTerm) ||
                    r.Livro.Titulo.Contains(searchTerm) ||
                    r.ReservaId.ToString().Contains(searchTerm)
                );
            }

            var reservas = await reservasQuery.ToListAsync();
            return View("Retiradas", reservas);
        }

        [HttpGet]
        public async Task<IActionResult> Retirar(int id)
        {
            // Busca a reserva pelo id
            var reserva = await _context.Reservas
                .Include(r => r.Livro)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.ReservaId == id);

            if (reserva == null || reserva.Cancelada || reserva.LivroRetirado)
            {
                TempData["SuccessMessage"] = "Reserva não encontrada ou já retirada/cancelada.";
                // Retorna para a lista de retiradas mesmo assim
                var reservas = await _context.Reservas
                    .Include(r => r.Livro)
                    .Include(r => r.Usuario)
                    .Where(r => !r.Cancelada && !r.LivroRetirado)
                    .ToListAsync();
                return View("Retiradas", reservas);
            }


            // Cria a movimentação
            var movimentacao = new Movimentacao
            {
                DataRetirada = DateTime.Now,
                DataDevolucao = DateTime.Now.AddDays(7),
                UsuarioId = reserva.UsuarioId,
                LivroId = reserva.LivroId,
                LivroDevolvido = false,
                LivroAtrasado = false
            };



            _context.Movimentacoes.Add(movimentacao);

            // Marca a reserva como retirada
            reserva.LivroRetirado = true;
            _context.Reservas.Update(reserva);
            await _context.SaveChangesAsync();


            var movimentacoesPendentes = await _context.Movimentacoes
              .Include(m => m.Livro)
              .Include(m => m.Usuario)
              .Where(m => !m.LivroDevolvido)
              .ToListAsync();

            ViewBag.MovimentacoesPendentes = movimentacoesPendentes;



            TempData["SuccessMessage"] = "Retirada Efetuada com Sucesso";


            // Retorna para a lista de retiradas
            var reservasAtivas = await _context.Reservas
                .Include(r => r.Livro)
                .Include(r => r.Usuario)
                .Where(r => !r.Cancelada && !r.LivroRetirado)
                .ToListAsync();

            return View("Retiradas", reservasAtivas);
        }

        [HttpGet]
        public async Task<IActionResult> Devolver(int id)
        {
            var movimentacao = await _context.Movimentacoes
                .Include(m => m.Livro)
                .FirstOrDefaultAsync(m => m.MovimentacaoId == id);

            if (movimentacao == null || movimentacao.LivroDevolvido)
            {
                TempData["SuccessMessage"] = "Movimentação não encontrada ou já devolvida.";

                // Carrega as listas para a view
                var reservas = await _context.Reservas
                    .Include(r => r.Livro)
                    .Include(r => r.Usuario)
                    .Where(r => !r.Cancelada && !r.LivroRetirado)
                    .ToListAsync();

                var movimentacoesPendentes = await _context.Movimentacoes
                    .Include(m => m.Livro)
                    .Include(m => m.Usuario)
                    .Where(m => !m.LivroDevolvido)
                    .ToListAsync();

                ViewBag.MovimentacoesPendentes = movimentacoesPendentes;
                ViewBag.TabAtiva = "devolucao";
                return View("Retiradas", reservas);
            }

            // Marca como devolvido e atualiza a data de devolução
            movimentacao.LivroDevolvido = true;
            movimentacao.DataDevolucao = DateTime.Now;

            // Atualiza a quantidade do livro
            if (movimentacao.Livro != null)
            {
                movimentacao.Livro.Quantidade += 1;
                _context.Livros.Update(movimentacao.Livro);
            }

            _context.Movimentacoes.Update(movimentacao);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Livro devolvido com sucesso!";

            // Carrega as listas para a view
            var reservasAtivas = await _context.Reservas
                .Include(r => r.Livro)
                .Include(r => r.Usuario)
                .Where(r => !r.Cancelada && !r.LivroRetirado)
                .ToListAsync();

            var movimentacoesPendentesAtualizadas = await _context.Movimentacoes
                .Include(m => m.Livro)
                .Include(m => m.Usuario)
                .Where(m => !m.LivroDevolvido)
                .ToListAsync();

            ViewBag.MovimentacoesPendentes = movimentacoesPendentesAtualizadas;
            ViewBag.TabAtiva = "devolucao";
            return View("Retiradas", reservasAtivas);
        }

        [HttpGet]
        public async Task<IActionResult> BuscaRetirada(string searchTerm)
        {
            var movimentacoesQuery = _context.Movimentacoes
                .Include(m => m.Livro)
                .Include(m => m.Usuario)
                .Where(m => !m.LivroDevolvido);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                movimentacoesQuery = movimentacoesQuery.Where(m =>
                    m.Livro.Titulo.Contains(searchTerm) ||
                    m.Usuario.NomeCompleto.Contains(searchTerm)
                );
            }

            var movimentacoesPendentes = await movimentacoesQuery.ToListAsync();

            // Mantém as reservas ativas para a aba de retiradas
            var reservas = await _context.Reservas
                .Include(r => r.Livro)
                .Include(r => r.Usuario)
                .Where(r => !r.Cancelada && !r.LivroRetirado)
                .ToListAsync();

            ViewBag.MovimentacoesPendentes = movimentacoesPendentes;

            ViewBag.TabAtiva = "devolucao";
            return View("Retiradas", reservas);
        }
    }
}
