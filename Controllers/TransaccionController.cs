using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaFinanciero.WebApi.Models;
using SistemaFinanciero.WebApi.Models.DTOs;
using SistemaFinanciero.WebApi.Models.DTOs.PaginationDTOs;
using SistemaFinanciero.WebApi.Models.Pagination;
using SistemaFinanciero.WebApi.Models.Security;
using SistemaFinanciero.WebApi.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaFinanciero.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TransaccionController : ControllerBase
    {
        private readonly ITransacciones _transacciones;
        private readonly IMapper _mapper;
        public TransaccionController(ITransacciones transacciones, IMapper mapper)
        {
            _transacciones = transacciones;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<TransaccionDTO>>> ObtenerTodas()
        {
            IEnumerable<Transaccion> transacciones = await _transacciones.ObtenerTodoAsync();
            bool transaccionesEncontradas = transacciones != null && transacciones.Count() > 0;

            if (transaccionesEncontradas)
            {
                IEnumerable<TransaccionDTO> transaccionesDTO = _mapper.Map<IEnumerable<TransaccionDTO>>(transacciones);
                return Ok(transaccionesDTO);
            }

            return NotFound("No hay transacciones...");
        }

        [HttpPost]
        public async Task<ActionResult<PagedTransactionsDTO>> ObtenerTodasUsuario([FromBody] TransaccionPayloadDTO transaccionPayload, [FromQuery] PaginationPayloadDTO parameters)
        {
            PagedList<Transaccion> transacciones = await _transacciones.GetTransaccionesUserId(transaccionPayload, parameters);
            bool transaccionesEncontradas = transacciones.Data != null && transacciones.Data.Count > 0;

            if (transaccionesEncontradas)
            {
                PagedTransactionsDTO pagedTransacciones = _mapper.Map<PagedTransactionsDTO>(transacciones);
                return Ok(pagedTransacciones);
            }

            return NotFound("No hay transacciones...");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransaccionDTO>> ObtenerPorId(int id)
        {
            Transaccion transaccionEncontrada = await _transacciones.ObtenerPorIdAsync(id);

            if (transaccionEncontrada != null)
            {
                TransaccionDTO transaccionDTO = _mapper.Map<TransaccionDTO>(transaccionEncontrada);
                return Ok(transaccionDTO);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> NuevaTransaccion([FromBody] Transaccion transaccion)
        {
            if (ModelState.IsValid)
            {
                bool altaExitosa = await _transacciones.AltaAsync(transaccion) != null;

                if (altaExitosa)
                {
                    return new CreatedAtActionResult(
                        "ObtenerPorId", 
                        "Transaccion", 
                        new { id = transaccion.Id }, 
                        transaccion
                    );
                }
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> BorrarTransaccion(int id)
        {
            Transaccion transaccionDesactivada = await _transacciones.BajaAsync(id);

            if (transaccionDesactivada != null)
            {
                TransaccionDTO transaccionDTO = _mapper.Map<TransaccionDTO>(transaccionDesactivada);
                return Ok(transaccionDTO);
            }

            return NotFound();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> ModificarTransaccion(int id, [FromBody] Transaccion transaccion)
        {
            if (ModelState.IsValid)
            {
                Transaccion transaccionModificada = await _transacciones.ModificarAsync(id, transaccion);

                if (transaccionModificada != null)
                {
                    TransaccionDTO transaccionDTO = _mapper.Map<TransaccionDTO>(transaccionModificada);
                    return Ok(transaccionDTO);
                }
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult<PagedTransactionsDTO>> GetFilteredTransactions([FromBody] TransaccionPayloadDTO transaccionPayload, [FromQuery] PaginationPayloadDTO paginationPayload)
        {
            PagedList<Transaccion> transaccionesPorFecha =
                await _transacciones.GetFilteredTransactions(transaccionPayload, paginationPayload);

            bool transaccionesEncontradas = transaccionesPorFecha != null && transaccionesPorFecha.Data.Count > 0;

            if (transaccionesEncontradas)
            {
                PagedTransactionsDTO pagedTransactions = _mapper.Map<PagedTransactionsDTO>(transaccionesPorFecha);
                return Ok(pagedTransactions);
            }
            return NotFound("No existen transacciones para los filtros aplicados...");
        }

        [HttpPost]
        public async Task<ActionResult<PagedTransactionsDTO>> FiltrarPorTipo([FromBody] TransaccionPayloadDTO transaccionPayload, [FromQuery] PaginationPayloadDTO paginationPayload)
        {
            PagedList<Transaccion> transaccionesPorTipo = await _transacciones.GetTransaccionesTipo(transaccionPayload, paginationPayload);
            bool transaccionesEncontradas = transaccionesPorTipo.Data != null && transaccionesPorTipo.Data.Count > 0;

            if (transaccionesEncontradas)
            {
                PagedTransactionsDTO pagedTransactions = _mapper.Map<PagedTransactionsDTO>(transaccionesPorTipo);
                return Ok(pagedTransactions);
            }
            return NotFound("No existen transacciones para el tipo solicitado...");
        }
    }
}
