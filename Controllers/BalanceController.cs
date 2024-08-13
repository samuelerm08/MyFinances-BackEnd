using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyFinances.WebApi.Models;
using MyFinances.WebApi.Models.DTOs;
using MyFinances.WebApi.Repository.Interfaces;
using System.Threading.Tasks;

namespace MyFinances.WebApi.Controllers
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
        public async Task<ActionResult<Balance>> GetByUserId(int id)
        {
            Balance balance = await _balances.GetByUserId(id);
            if (balance != null)
            {
                BalanceDTO balanceDTO = _mapper.Map<BalanceDTO>(balance);
                return Ok(balanceDTO);
            }

            return NotFound();
        }
    }
}
