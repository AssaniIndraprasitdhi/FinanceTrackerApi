using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceTracker.Api.Data;
using FinanceTracker.Api.DTOs;
using FinanceTracker.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // GET: api/Role
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        // GET: api/Role/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoleDto>> GetRoleById(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound(); // 404 ถ้าไม่เจอ

            return Ok(role);
        }

        // POST: api/Role
        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole([FromBody] RoleDto dto)
        {
            if (dto == null)
                return BadRequest(); // 400 ถ้า body เป็น null

            var created = await _roleService.CreateRoleAsync(dto);

            // คืน 201 Created พร้อม Location header ชี้ไปยัง GET by id
            return CreatedAtAction(
                nameof(GetRoleById),
                new { id = created.Id },
                created
            );
        }

        // PUT: api/Role/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleDto dto)
        {
            if (dto == null || id != dto.Id)
                return BadRequest(); // 400 ถ้าข้อมูลไม่ถูกต้อง

            var updated = await _roleService.UpdateRoleAsync(id, dto);
            if (!updated)
                return NotFound(); // 404 ถ้าไม่เจอ entity

            return NoContent(); // 204 ถ้าอัพเดตสำเร็จ
        }

        // DELETE: api/Role/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var deleted = await _roleService.DeleteRoleAsync(id);
            if (!deleted)
                return NotFound(); 

            return NoContent(); 
        }
    }
}
