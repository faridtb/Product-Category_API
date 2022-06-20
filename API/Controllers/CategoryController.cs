using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
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
        
        

        [HttpPost("createCategory")]
        public async Task<ActionResult<CategoryReturnDto>> CreateCategory([FromBody]CategoryReturnDto categoryDto)
        {
            if(await CategoryExist(categoryDto.Name)) return BadRequest("This Category already have in database");

            var cat = new Category
            {
                Name = categoryDto.Name.ToLower()
            };

            _context.Categories.Add(cat);

            await _context.SaveChangesAsync();

            return new CategoryReturnDto{
                Name = cat.Name
            };
        }
        private async Task<bool> CategoryExist(string categoryName)
        {
            return await _context.Categories.AnyAsync(x=>x.Name == categoryName.ToLower());
        }

        [HttpDelete("remove/{id}")]
        public async Task<ActionResult> RemoveCategory(int id)
        {
            var cat = _context.Categories.Find(id);

            if(cat == null) return BadRequest($"Cant Find any catagory in that {id}");
            
            _context.Categories.Remove(cat);

            await _context.SaveChangesAsync();

            return Ok("Category Deleted from Database");
        }

         [HttpPut("updateCategory/{id}")]
        public async Task<ActionResult> UpdateCategory(int id,[FromBody]CategoryReturnDto categoryReturnDto)
        {
            var cat = _context.Categories.Find(id);

            if(cat == null) return BadRequest($"Cant Find any Category in that {id}");
            
            if(cat.Name == categoryReturnDto.Name) return BadRequest("You cant change anything here");

            cat.Name = categoryReturnDto.Name;            
             
            _context.Categories.Update(cat);

            await _context.SaveChangesAsync();

            return Ok("Category Updated succesfully");
        }

        
    }
}