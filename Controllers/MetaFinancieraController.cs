using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaFinanciero.WebApi.Models;
using SistemaFinanciero.WebApi.Models.DTOs;
using SistemaFinanciero.WebApi.Models.DTOs.PaginationDTOs;
using SistemaFinanciero.WebApi.Models.Pagination;
using SistemaFinanciero.WebApi.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaFinanciero.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class MetaFinancieraController : ControllerBase
    {
        private readonly IMetaFinanciera _metas;
        private readonly IMapper _mapper;
        public MetaFinancieraController(IMetaFinanciera metas, IMapper mapper)
        {
            _metas = metas;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> AltaMetaFinanciera([FromBody] MetaFinanciera metaFinanciera)
        {
            if (ModelState.IsValid)
            {
                bool altaExitosa = await _metas.AltaAsync(metaFinanciera) != null;

                if (altaExitosa)
                {
                    return Ok(metaFinanciera);
                }
            }

            return BadRequest("Los campos ingresados son incorrectos...");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> ModificarMetaFinanciera(int id, [FromBody] MetaFinanciera metaFinanciera)
        {
            if (ModelState.IsValid)
            {
                MetaFinanciera metaFinancieraModificada = await _metas.ModificarAsync(id, metaFinanciera);

                if (metaFinancieraModificada != null)
                {
                    return Ok(metaFinancieraModificada);
                }
            }
            return BadRequest("Los campos ingresados son incorrectos...");
        }


        [HttpPost]
        public async Task<ActionResult> ObtenerPorUserId(MetaFinancieraPayload metaFinancieraPayload, [FromQuery] PaginationPayloadDTO paginationPayload)
        {
            PagedList<MetaFinanciera> metas = await _metas.MetasPorUserId(metaFinancieraPayload, paginationPayload);
            bool metasFinancierasEncontradas = metas.Data != null && metas.Data.Count > 0;
            if (metasFinancierasEncontradas)
            {
                PagedGoalsDTO pagedGoals = _mapper.Map<PagedGoalsDTO>(metas);
                return Ok(pagedGoals);
            }
            return NotFound("No existen metas con el ID solicitado...");
        }

        [HttpPost]
        public async Task<ActionResult> ObtenerPorEstado(MetaFinancieraPayload metaFinancieraPayload, [FromQuery] PaginationPayloadDTO paginationPayload)
        {
            PagedList<MetaFinanciera> metas = await _metas.MetasPorEstado(metaFinancieraPayload, paginationPayload);
            bool metasFinancierasEncontradas = metas.Data != null && metas.Data.Count > 0;
            if (metasFinancierasEncontradas)
            {
                PagedGoalsDTO pagedGoals = _mapper.Map<PagedGoalsDTO>(metas);
                return Ok(pagedGoals);
            }
            return NotFound("No existen metas con el estado solicitado...");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            MetaFinanciera metaFinanciera = await _metas.EliminarAsync(id);

            if (metaFinanciera != null)
            {
                return Ok(metaFinanciera);
            }

            return BadRequest();
        }

        [HttpDelete("{metaId}")]
        public async Task<ActionResult> RetirarMeta(int metaId)
        {
            MetaFinanciera metaFinanciera = await _metas.RetirarMontoMeta(metaId);

            if (metaFinanciera != null)
            {
                return Ok(metaFinanciera);
            }

            return BadRequest();
        }
    }
}
