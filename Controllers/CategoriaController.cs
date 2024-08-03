using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SistemaFinanciero.WebApi.Models;
using SistemaFinanciero.WebApi.Repository.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaFinanciero.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoria _categoria;

        public CategoriaController(ICategoria categoria)
        {
            _categoria = categoria;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> NuevaCategoria([FromBody] Categoria cat)
        {

            if (ModelState.IsValid)
            {
                bool altaExitosa = await _categoria.AltaAsync(cat) != null;

                if (altaExitosa)
                {
                    return Ok(cat);
                }

            }
            return BadRequest();

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Categoria>> BorrarCategoria(int id)
        {
            bool eliminacionExitosa = await _categoria.BajaAsync(id) != null;

            if (eliminacionExitosa)
            {
                return Ok();
            }

            return NotFound();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> ObtenerPorId(int id)
        {
            Categoria categoria = await _categoria.ObtenerPorIdAsync(id);

            if (categoria != null)
            {
                return Ok(categoria);
            }

            return NotFound("No se encontró la categoría.");

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> ObtenerTodas()
        {
            IEnumerable<Categoria> categorias = await _categoria.ObtenerTodoAsync();
            bool categoriasEncontradas = categorias != null && categorias.Count() > 0;

            if (categoriasEncontradas)
            {
                return Ok(categorias);
            }

            return NotFound("No se encontraron categorías.");

        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Categoria>> ModificarCategoria([FromBody] Categoria cat, int id)
        {
            if(ModelState.IsValid)
            {
                Categoria Categoria = await _categoria.ModificarAsync(id, cat);
                if(Categoria != null)
                {
                    return Ok(Categoria);
                }
            }
            return BadRequest();

        }
    }
}
