using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using WebApplication1.Models;

namespace WebApplication1.Pages
{
    public class IndexModel : PageModel
    {
        public List<Tarea> Tareas { get; set; } = new List<Tarea>();
        public int TotalPaginas { get; set; }
        public int PaginaActual { get; set; }
        public string FiltroEstado { get; set; }
        public int TareasPorPagina { get; set; } = 5;

        private readonly string jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tareas.json");

        public void OnGet(int? pagina, int? cantidad, string estado)
        {
            PaginaActual = pagina ?? 1;
            TareasPorPagina = (cantidad == 5 || cantidad == 10 || cantidad == 20) ? cantidad.Value : 5;
            FiltroEstado = estado;

            if (System.IO.File.Exists(jsonPath))
            {
                var jsonData = System.IO.File.ReadAllText(jsonPath);
                var todasLasTareas = JsonSerializer.Deserialize<List<Tarea>>(jsonData) ?? new List<Tarea>();

                if (!string.IsNullOrEmpty(FiltroEstado))
                {
                    todasLasTareas = todasLasTareas
                        .Where(t => t.estado.Equals(FiltroEstado, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                int totalTareas = todasLasTareas.Count;
                TotalPaginas = (int)Math.Ceiling(totalTareas / (double)TareasPorPagina);

                Tareas = todasLasTareas
                    .Skip((PaginaActual - 1) * TareasPorPagina)
                    .Take(TareasPorPagina)
                    .ToList();
            }
        }

        public IActionResult OnPost(string nombreTarea, string fechaVencimiento, string estado)
        {
            if (string.IsNullOrWhiteSpace(nombreTarea) || string.IsNullOrWhiteSpace(fechaVencimiento) || string.IsNullOrWhiteSpace(estado))
            {
                return Page();
            }

            List<Tarea> listaTareas;

            if (System.IO.File.Exists(jsonPath))
            {
                var jsonData = System.IO.File.ReadAllText(jsonPath);
                listaTareas = JsonSerializer.Deserialize<List<Tarea>>(jsonData) ?? new List<Tarea>();
            }
            else
            {
                listaTareas = new List<Tarea>();
            }

            var nuevaTarea = new Tarea
            {
                idTarea = Guid.NewGuid().ToString(),
                nombreTarea = nombreTarea,
                fechaVencimiento = fechaVencimiento,
                estado = estado
            };

            listaTareas.Add(nuevaTarea);

            var opciones = new JsonSerializerOptions { WriteIndented = true };
            var jsonActualizado = JsonSerializer.Serialize(listaTareas, opciones);
            System.IO.File.WriteAllText(jsonPath, jsonActualizado);

            return RedirectToPage(new { pagina = 1, cantidad = TareasPorPagina, estado = FiltroEstado });
        }
    }
}
