using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaFinanciero.WebApi.Models;
using SistemaFinanciero.WebApi.Models.DTOs;
using SistemaFinanciero.WebApi.Repository.Interfaces;
using System.Threading.Tasks;

namespace SistemaFinanciero.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BalanceController : ControllerBase
    {
        private readonly IBalance _balances;
        private readonly IMapper _mapper;
        public BalanceController(IBalance balances, IMapper mapper)
        {
            _balances = balances;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Balance>> GetBalanceByUserId(int id)
        {
            Balance balance = await _balances.GetBalanceByUserId(id);

            if (balance != null)
            {
                BalanceDTO balanceDTO = _mapper.Map<BalanceDTO>(balance);
                return Ok(balanceDTO);
            }

            return NotFound();
        }
    }
}
