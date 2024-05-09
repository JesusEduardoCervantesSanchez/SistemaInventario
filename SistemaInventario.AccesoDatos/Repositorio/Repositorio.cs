using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos.Especificaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Quic;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {
        //acceso a los datos, conexion db
        private readonly ApplicationDbContext _db;
        //necesiatmos mandar el conjunto de datos a travez de una variable
        internal DbSet<T> dbSet;
        public Repositorio(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet=_db.Set<T>();
        }
        public async Task Agregar(T entidad)
        {
            await dbSet.AddAsync(entidad);//Insert in to table
        }

        public async Task<T> Obtener(int id)
        {
            return await dbSet.FindAsync(id);//Select * from where id=id
        }

        public async Task<IEnumerable<T>> ObtenerTodos(Expression<Func<T, bool>> filtro = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string incluirPropiedades = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            if (filtro != null)
            {
                query = query.Where(filtro);
            }
            if (incluirPropiedades != null)
            {
                foreach (var incluirProp in incluirPropiedades.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluirProp);
                }
            }
            if (orderBy != null)
            {
                query=orderBy(query);
            }
            if(!isTracking)
            {
                query =query.AsNoTracking();
            }
            return await query.ToListAsync();
        }


        public async Task<T> ObtenerPrimero(Expression<Func<T, bool>> filtro = null, string incluirPropiedades = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            if (filtro != null)
            {
                query = query.Where(filtro);
            }
            if (incluirPropiedades != null)
            {
                foreach (var incluirProp in incluirPropiedades.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluirProp);
                }
            }
            if (!isTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync();
        }

        public void Remover(T entidad)
        {
            dbSet.Remove(entidad);
        }

        public void RemoverRango(IEnumerable<T> entidad)
        {
            dbSet.RemoveRange(entidad);
        }

		public PageList<T> ObtenerTodosPaginado(Parametros para, Expression<Func<T, bool>> filtro = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string incluirPropiedades = null, bool isTracking = true)
		{
			IQueryable<T> query = dbSet;
            if(filtro != null)
            {
                query = query.Where(filtro);
            }
			if (incluirPropiedades != null)
			{
				foreach (var incluirProp in incluirPropiedades.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(incluirProp);
				}
			}
			if (orderBy != null)
			{
				query = orderBy(query);
			}
			if (!isTracking)
			{
				query = query.AsNoTracking();
			}
            return PageList<T>.ToPagesList(query, para.pageNumber, para.pageSize);
		}
	}
}
