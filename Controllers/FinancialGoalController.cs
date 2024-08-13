using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFinances.WebApi.Models;
using MyFinances.WebApi.Models.DTOs;
using MyFinances.WebApi.Models.DTOs.PaginationDTOs;
using MyFinances.WebApi.Models.Pagination;
using MyFinances.WebApi.Repository.Interfaces;
using System.Threading.Tasks;

namespace MyFinances.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class FinancialGoalController : ControllerBase
    {
        private readonly IFinancialGoal _goals;
        private readonly IMapper _mapper;
        public FinancialGoalController(IFinancialGoal goals, IMapper mapper)
        {
            _goals = goals;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] FinancialGoal financialGoal)
        {
            if (ModelState.IsValid)
            {
                bool success = await _goals.CreateAsync(financialGoal) != null;
                if (success) return Ok(financialGoal);
            }

            return BadRequest("The entered fields are incorrect...");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Modify(int id, [FromBody] FinancialGoal financialGoal)
        {
            if (ModelState.IsValid)
            {
                FinancialGoal updatedFinancialGoal = await _goals.ModifyAsync(id, financialGoal);
                if (updatedFinancialGoal != null)
                {
                    return Ok(updatedFinancialGoal);
                }
            }
            return BadRequest("The entered fields are incorrect...");
        }


        [HttpPost]
        public async Task<ActionResult> GetByUserId(FinancialGoalPayload financialGoalPayload, [FromQuery] PaginationPayloadDTO paginationPayload)
        {
            PagedList<FinancialGoal> goals = await _goals.GoalsByUserId(financialGoalPayload, paginationPayload);
            bool financialGoalsFound = goals.Data != null && goals.Data.Count > 0;
            if (financialGoalsFound)
            {
                PagedGoalsDTO pagedGoals = _mapper.Map<PagedGoalsDTO>(goals);
                return Ok(pagedGoals);
            }
            return NotFound("No goals exist with the requested ID...");
        }

        [HttpPost]
        public async Task<ActionResult> GetByState(FinancialGoalPayload financialGoalPayload, [FromQuery] PaginationPayloadDTO paginationPayload)
        {
            PagedList<FinancialGoal> goals = await _goals.GoalsByState(financialGoalPayload, paginationPayload);
            bool financialGoalsFound = goals.Data != null && goals.Data.Count > 0;
            if (financialGoalsFound)
            {
                PagedGoalsDTO pagedGoals = _mapper.Map<PagedGoalsDTO>(goals);
                return Ok(pagedGoals);
            }
            return NotFound("No goals exist with the requested ID...");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            FinancialGoal financialGoal = await _goals.DeleteAsync(id);
            if (financialGoal != null) return Ok(financialGoal);
            return BadRequest();
        }

        [HttpDelete("{metaId}")]
        public async Task<ActionResult> Withdraw(int metaId)
        {
            FinancialGoal financialGoal = await _goals.Withdraw(metaId);
            if (financialGoal != null) return Ok(financialGoal);
            return BadRequest();
        }
    }
}
