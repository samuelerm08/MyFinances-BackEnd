using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaFinanciero.WebApi.Models;
using SistemaFinanciero.WebApi.Models.DTOs;
using SistemaFinanciero.WebApi.Models.Security;
using SistemaFinanciero.WebApi.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaFinanciero.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]    
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuario _usuarios;
        private readonly IMapper _mapper;
        public UsuarioController(IUsuario user, IMapper mapper)
        {
            _usuarios = user;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Usuario>>> ObtenerTodos()
        {
            IEnumerable<Usuario> usuarios = await _usuarios.ObtenerTodoAsync();
            bool usuariosEncontrados = usuarios != null && usuarios.Count() > 0;
            if (usuariosEncontrados)
            {
                IEnumerable<UsuarioDTO> usuariosDTO = _mapper.Map<IEnumerable<UsuarioDTO>>(usuarios);
                return Ok(usuariosDTO);
            }
            return NotFound();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Usuario")]
        public async Task<ActionResult<Usuario>> ObtenerPorId(int id)
        {
            Usuario usuario = await _usuarios.ObtenerPorIdAsync(id);

            if (usuario != null)
            {
                UsuarioDTO usuarioDTO = _mapper.Map<UsuarioDTO>(usuario);
                return Ok(usuarioDTO);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> RegistroUsuario([FromBody] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                Usuario usuarioCreado = await _usuarios.Registro(usuario);

                if (usuarioCreado != null)
                {
                    return new CreatedAtActionResult(
                        "ObtenerPorId", 
                        "Usuario", 
                        new { id = usuarioCreado.Id },
                        usuarioCreado
                    );
                }
                else
                {
                    return BadRequest("El email ingresado esta en uso. Por favor, ingresar un email distinto...");
                }
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult> Login([FromBody] UsuarioAuth usuario)
        {
            if (ModelState.IsValid)
            {
                UsuarioToken usuarioToken = await _usuarios.Login(usuario);

                if (usuarioToken != null)
                {
                    return Ok(usuarioToken);
                }
            }

            return NotFound("Contraseña y/o correo electrónico incorrecto...");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Modificar(int id, [FromBody] Usuario usuario)
        {            
            if (id != usuario.Id)
            {
                return BadRequest($"El ID no solicitado no coincide con el del usuario a modificar...{System.Environment.NewLine}Por favor volver a intentar");
            }

            Usuario usuarioModificado = await _usuarios.ModificarAsync(usuario);
            return Ok(usuarioModificado);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            Usuario usuario = await _usuarios.BajaAsync(id);

            if (usuario != null)
            {
                return Ok(usuario);
            }

            return NotFound("No existe un usuario con el ID solicitado...");
        }
    }
}
