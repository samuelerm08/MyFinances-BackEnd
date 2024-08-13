using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFinances.WebApi.Models;
using MyFinances.WebApi.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategory _category;

        public CategoryController(ICategory category)
        {
            _category = category;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateCategory([FromBody] Category cat)
        {
            if (ModelState.IsValid)
            {
                bool success = await _category.CreateAsync(cat) != null;
                if (success) return Ok(cat);
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> DeleteCategory(int id)
        {
            bool success = await _category.DeleteAsync(id) != null;
            if (success) return Ok();
            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            Category category = await _category.GetByIdAsync(id);
            if (category != null) return Ok(category);
            return NotFound("Category not found.");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            IEnumerable<Category> categories = await _category.GetAllAsync();
            bool categoriesFound = categories != null && categories.Count() > 0;
            if (categoriesFound) return Ok(categories);
            return NotFound("Categories not found.");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Category>> ModifyCategory([FromBody] Category cat, int id)
        {
            if (ModelState.IsValid)
            {
                Category category = await _category.ModifyAsync(id, cat);
                if (category != null) return Ok(category);
            }
            return BadRequest();
        }
    }
}
