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
    public class ProductController : BaseApiController
    {
        private readonly DataContext _context;
        public ProductController(DataContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
           return await _context.Products.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            return await _context.Products.FindAsync(id);
        }
        
        

        [HttpPost("createProduct")]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody]ProductDto productDto)
        {
            if(await ProductExist(productDto.Name)) return BadRequest("This Product already in database");

            var pro = new Product
            {
                Name = productDto.Name.ToLower(),
                CategoryId = productDto.CategoryId
            };
            
            var cates = await _context.Categories.FindAsync(pro.CategoryId);

            if(cates == null) return BadRequest("We cant find any category in that id");

            cates.Products.Add(pro);

            _context.Products.Add(pro);

            await _context.SaveChangesAsync();

            return new ProductDto{
                Name = pro.Name,
                CategoryId = pro.CategoryId,
            };
        }
        private async Task<bool> ProductExist(string productName)
        {
            return await _context.Products.AnyAsync(x=>x.Name == productName.ToLower());
        }

        [HttpDelete("remove/{id}")]
        public async Task<ActionResult> RemoveProduct(int id)
        {
            var pro = _context.Products.Find(id);

            if(pro == null) return BadRequest($"Cant Find any product in that {id}");
            
            _context.Products.Remove(pro);

            await _context.SaveChangesAsync();

            return Ok("Product Deleted from Database");
        }

        [HttpPut("updateProduct/{id}")]
        public async Task<ActionResult> UpdateProduct(int id,[FromBody]ProductDto productDto)
        {
            var pro = _context.Products.Find(id);

            if(pro == null) return BadRequest($"Cant Find any product in that {id}");

            if(pro.Name == productDto.Name && pro.CategoryId == productDto.CategoryId) return BadRequest("You cant change anything here");

            pro.Name = productDto.Name;
            pro.CategoryId = productDto.CategoryId;
            
             
            _context.Products.Update(pro);

            await _context.SaveChangesAsync();

            return Ok("Product Updated succesfully");
        }

    }
}