using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Configuracion;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;

namespace Sistema_Inventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MarcaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        public MarcaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo= unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }

        //metodo Upsert GET
        public async Task<IActionResult> Upsert(int? id)
        {
            Marca marca = new Marca();
            if (id == null)
            {
                //Creamos un nuevo registro
                marca.Estado = true;
                return View(marca);
            }
            marca = await _unidadTrabajo.Marca.Obtener(id.GetValueOrDefault());
            if (marca == null)
            {
                return NotFound();
            }
            return View(marca);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Marca marca)
        {
            if(ModelState.IsValid)
            {
                if (marca.id == 0)
                {
                    await _unidadTrabajo.Marca.Agregar(marca);
                    TempData[DS.Existosa] = "La marca se creo con exito";
                }
                else
                {
                    _unidadTrabajo.Marca.Actualizar(marca);
                    TempData[DS.Existosa] = "La marca se actualizo con exito";
                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar la categoria";
            return View(marca);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var marcaDB = await _unidadTrabajo.Marca.Obtener(id);
            if (marcaDB == null)
            {
                return Json(new { success = false, message = "Error al borrar el registro en la base de datos" });
            }
            _unidadTrabajo.Marca.Remover(marcaDB);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Marca elminiada con exito" });
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos=await _unidadTrabajo.Marca.ObtenerTodos();
            return Json(new {data = todos});
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id=0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Marca.ObtenerTodos();

            if(id==0)
            {
                valor = lista.Any(b => b.Nombre.ToLower().Trim()
                                    ==nombre.ToLower().Trim());
            }
            else
            {
				valor = lista.Any(b => b.Nombre.ToLower().Trim() 
                                    == nombre.ToLower().Trim() && b.id!=id);
			}
            if (valor) {
                return Json(new { data = true });
            }
            return Json(new { data = false });
		}
        #endregion
    }
}
