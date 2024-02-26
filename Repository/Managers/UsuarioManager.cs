using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SistemaFinanciero.WebApi.Data;
using SistemaFinanciero.WebApi.Models;
using SistemaFinanciero.WebApi.Models.Security;
using SistemaFinanciero.WebApi.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaFinanciero.WebApi.Repository.Managers
{
    public class UsuarioManager : IUsuario
    {
        private readonly DbFinancesContext _context;
        private readonly TokenService _tokenService;
        public UsuarioManager(DbFinancesContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<Usuario> Registro(Usuario usuario)
        {
            try
            {
                bool emailEnUso = await _context.Usuario.FirstOrDefaultAsync(u =>
                    u.Email == usuario.Email) != null;

                if (!emailEnUso)
                {
                    usuario.Contraseña = Encrypt.GetEncryptedPassword(usuario.Contraseña);
                    await _context.Usuario.AddAsync(usuario);
                    await _context.SaveChangesAsync();
                    
                    return usuario;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        public async Task<UsuarioToken> Login(UsuarioAuth login)
        {
            UsuarioToken usuarioToken = null;
            try
            {
                Usuario usuario = await AutenticarUsuario(login);

                if (usuario == null)
                {
                    return usuarioToken;
                }

                string token = _tokenService.GenerarJWT(usuario);

                usuarioToken = new UsuarioToken()
                {
                    Email = usuario.Email,
                    Token = token
                };

                return usuarioToken;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        private async Task<Usuario> AutenticarUsuario(UsuarioAuth usuario)
        {
            Usuario usuarioValido = null;

            usuario.Contraseña = Encrypt.GetEncryptedPassword(usuario.Contraseña);
            usuarioValido = await _context.Usuario.FirstOrDefaultAsync
                                    (u =>
                                    u.Email == usuario.Email &&
                                    u.Contraseña == usuario.Contraseña);
            return usuarioValido;
        }

        public async Task<Usuario> BajaAsync(int id)
        {
            Usuario usuario = null;
            try
            {
                usuario = await _context.Usuario
                    .Include(u => u.Metas)
                    .Where(u => u.Id == id)
                    .SingleOrDefaultAsync();

                if (usuario != null)
                {
                    _context.Usuario.Remove(usuario);
                    await _context.SaveChangesAsync();
                    return usuario;
                }

                return usuario;
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        public async Task<Usuario> ModificarAsync(Usuario usuario)
        {
            try
            {
                Usuario userToModify = await ObtenerPorIdAsync(usuario.Id);
                userToModify.Nombre = usuario.Nombre;
                userToModify.Apellido = usuario.Apellido;
                _context.Entry(userToModify).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return usuario;
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        public async Task<Usuario> ObtenerPorIdAsync(int id)
        {
            try
            {
                Usuario usuario = await _context.Usuario
                    .Include(u => u.Metas)
                    .SingleOrDefaultAsync(t => t.Id == id);
                return usuario;
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        public async Task<IEnumerable<Usuario>> ObtenerTodoAsync()
        {
            try
            {
                return await _context.Usuario
                    .Include(p => p.Metas)
                    .ToListAsync();
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }
    }
}
