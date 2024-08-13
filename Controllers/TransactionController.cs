using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFinances.WebApi.Models;
using MyFinances.WebApi.Models.DTOs;
using MyFinances.WebApi.Models.DTOs.PaginationDTOs;
using MyFinances.WebApi.Models.Pagination;
using MyFinances.WebApi.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactions _transactions;
        private readonly IMapper _mapper;
        public TransactionController(ITransactions transactions, IMapper mapper)
        {
            _transactions = transactions;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<TransactionDTO>>> GetAll()
        {
            IEnumerable<Transaction> transactions = await _transactions.GetAllAsync();
            bool transactionsFound = transactions != null && transactions.Count() > 0;
            if (transactionsFound)
            {
                IEnumerable<TransactionDTO> transactionsDTO = _mapper.Map<IEnumerable<TransactionDTO>>(transactions);
                return Ok(transactionsDTO);
            }

            return NotFound("Transactions not found...");
        }

        [HttpPost]
        public async Task<ActionResult<PagedTransactionsDTO>> GetAllByUserId([FromBody] TransactionPayloadDTO transactionPayloadDTO, [FromQuery] PaginationPayloadDTO parameters)
        {
            PagedList<Transaction> transactions = await _transactions.GetByUserIdAsync(transactionPayloadDTO, parameters);
            bool transactionsFound = transactions.Data != null && transactions.Data.Count > 0;
            if (transactionsFound)
            {
                PagedTransactionsDTO pagedTransacciones = _mapper.Map<PagedTransactionsDTO>(transactions);
                return Ok(pagedTransacciones);
            }

            return NotFound("Transactions not found...");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDTO>> GetById(int id)
        {
            Transaction foundTransaction = await _transactions.GetByIdAsync(id);
            if (foundTransaction != null)
            {
                TransactionDTO TransactionDTO = _mapper.Map<TransactionDTO>(foundTransaction);
                return Ok(TransactionDTO);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                bool success = await _transactions.CreateAsync(transaction) != null;
                if (success)
                {
                    return new CreatedAtActionResult(
                        "GetById", 
                        "Transaction", 
                        new { id = transaction.Id }, 
                        transaction
                    );
                }
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            Transaction removedTransaction = await _transactions.DeleteAsync(id);
            if (removedTransaction != null)
            {
                TransactionDTO transactionDTO = _mapper.Map<TransactionDTO>(removedTransaction);
                return Ok(transactionDTO);
            }

            return NotFound();
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Modify(int id, [FromBody] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                Transaction updatedTransaction = await _transactions.ModifyAsync(id, transaction);

                if (updatedTransaction != null)
                {
                    TransactionDTO TransactionDTO = _mapper.Map<TransactionDTO>(updatedTransaction);
                    return Ok(TransactionDTO);
                }
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult<PagedTransactionsDTO>> Filter([FromBody] TransactionPayloadDTO transactionPayloadDTO, [FromQuery] PaginationPayloadDTO paginationPayload)
        {
            PagedList<Transaction> transactionsByDate =
                await _transactions.FilterAsync(transactionPayloadDTO, paginationPayload);

            bool transactionsFound = transactionsByDate != null && transactionsByDate.Data.Count > 0;
            if (transactionsFound)
            {
                PagedTransactionsDTO pagedTransactions = _mapper.Map<PagedTransactionsDTO>(transactionsFound);
                return Ok(pagedTransactions);
            }
            return NotFound("No transactions exist for the applied filters...");
        }

        [HttpPost]
        public async Task<ActionResult<PagedTransactionsDTO>> GetByType([FromBody] TransactionPayloadDTO transactionPayloadDTO, [FromQuery] PaginationPayloadDTO paginationPayload)
        {
            PagedList<Transaction> transactionsByType = await _transactions.GetByTypeAsync(transactionPayloadDTO, paginationPayload);
            bool transactionsFound = transactionsByType.Data != null && transactionsByType.Data.Count > 0;

            if (transactionsFound)
            {
                PagedTransactionsDTO pagedTransactions = _mapper.Map<PagedTransactionsDTO>(transactionsByType);
                return Ok(pagedTransactions);
            }
            return NotFound("No transactions exist for the requested type...");
        }
    }
}
