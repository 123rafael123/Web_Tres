using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<IndexModel> _logger;

        public List<Tarea> Tareas { get; set; } = new List<Tarea>();
        public int TotalPaginas { get; set; }
        public int PaginaActual { get; set; }
        public string FiltroEstado { get; set; }

        public int TareasPorPagina { get; set; } = 5;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet(int? pagina, int? cantidad, string estado)
        {
            PaginaActual = pagina ?? 1;
            TareasPorPagina = cantidad.HasValue && (cantidad == 5 || cantidad == 10 || cantidad == 20) ? cantidad.Value : 5;
            FiltroEstado = estado;

            string jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tareas.json");

            if (System.IO.File.Exists(jsonPath))
            {
                var jsonData = System.IO.File.ReadAllText(jsonPath);
                var todasLasTareas = JsonSerializer.Deserialize<List<Tarea>>(jsonData) ?? new List<Tarea>();

                if (!string.IsNullOrEmpty(estado))
                {
                    todasLasTareas = todasLasTareas
                        .Where(t => t.estado.Equals(estado, StringComparison.OrdinalIgnoreCase))
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
    }
}
