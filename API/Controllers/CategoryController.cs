using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class CategoryController : BaseApiController
    {
         private readonly DataContext _context;
        public CategoryController(DataContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
           return await _context.Categories.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            return await _context.Categories.FindAsync(id);
        }
        
        

         [HttpPost("create")]
        public async Task<ActionResult<Category>> CreateCategory(Category category)
        {
            if(await CategoryExist(category.Name)) return BadRequest("This Category already have in database");

            var cat = new Category
            {
                Name = category.Name.ToLower()
            };

            _context.Categories.Add(cat);

            await _context.SaveChangesAsync();

            return cat;
        }
        private async Task<bool> CategoryExist(string categoryName)
        {
            return await _context.Products.AnyAsync(x=>x.Name == categoryName.ToLower());
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveCategory(int id)
        {
            var cat = _context.Categories.Find(id);

            if(cat == null) return BadRequest($"Cant Find any product in that {id}");
            
            _context.Categories.Remove(cat);

            await _context.SaveChangesAsync();

            return Ok("Product Deleted from Database");
        }

        
    }
}