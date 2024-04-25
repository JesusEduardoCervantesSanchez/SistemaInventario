using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using SistemaInventario.AccesoDatos.Configuracion;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;

namespace Sistema_Inventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductoController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductoController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment webHostEnvironment)
        {
            _unidadTrabajo= unidadTrabajo;
            _webHostEnvironment= webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        //metodo Upsert GET
        public async Task<IActionResult> Upsert(int? id)
        {
            ProductoVM productoVM = new ProductoVM()
            {
                Producto = new Producto(),
                CategoriaLista = _unidadTrabajo.Producto.ObtenerTodosDropDownList("Categoria"),
                MarcaLista = _unidadTrabajo.Producto.ObtenerTodosDropDownList("Marca"),
                PadreLista = _unidadTrabajo.Producto.ObtenerTodosDropDownList("Producto")
            };
            if (id == null)
            {
                //Crear un producto nuevo
                return View(productoVM);
            }
            else
            {
                //Actualizar un producto existente
                productoVM.Producto = await _unidadTrabajo.Producto.Obtener(id.GetValueOrDefault());
                if(productoVM.Producto == null)
                {
                    return NotFound();
                }
                return View(productoVM);
            }
        }

        #region API
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductoVM productoVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productoVM.Producto.id == 0)
                {
                    //crear nuevo producto
                    string upload = webRootPath + DS.ImagenRuta;
                    //crear un id unico en mi sistema
                    string fileName = Guid.NewGuid().ToString();
                    //creamos una variable para conocer la extension de archivo
                    string extension = Path.GetExtension(files[0].FileName);
                    //habilitar el filestream para crear el archivo de la imgen en tiempo real
                    using (var filestream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }
                    productoVM.Producto.ImagenUrl = fileName + extension;
                    await _unidadTrabajo.Producto.Agregar(productoVM.Producto);
                }
                else
                {
                    //Actualizar al producto
                    var objProducto = await _unidadTrabajo.Producto.ObtenerPrimero(p => p.id == productoVM.Producto.id
                    , isTracking: false);
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + DS.ImagenRuta;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        //borrar la imagen anterior
                        var anteriorFile = Path.Combine(upload, objProducto.ImagenUrl);
                        //verificamos que la imagen exista
                        if (System.IO.File.Exists(anteriorFile))
                        {
                            System.IO.File.Delete(anteriorFile);
                        }
                        //Creamos la nueva imagen
                        using (var filestream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(filestream);
                        }
                        productoVM.Producto.ImagenUrl = fileName + extension;
                    }//sino eleigen imagen
                    else
                    {
                        productoVM.Producto.ImagenUrl = objProducto.ImagenUrl;
                    }
                    _unidadTrabajo.Producto.Actualizar(productoVM.Producto);
                }
                TempData[DS.Existosa] = "Producto reguistrado de manera existosa";
                await _unidadTrabajo.Guardar();
                return View("Index");
            }
            TempData[DS.Error] = "Error al registrar el producto";
            productoVM.CategoriaLista = _unidadTrabajo.Producto.ObtenerTodosDropDownList("Categoria");
            productoVM.MarcaLista = _unidadTrabajo.Producto.ObtenerTodosDropDownList("Marca");
            productoVM.PadreLista = _unidadTrabajo.Producto.ObtenerTodosDropDownList("Producto");
            return View(productoVM);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos=await _unidadTrabajo.Producto.ObtenerTodos(incluirPropiedades: "Categoria,Marca");
            return Json(new {data = todos});
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string serie, int id=0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Producto.ObtenerTodos();

            if(id==0)
            {
                valor = lista.Any(b => b.NumeroSerie.ToLower().Trim()
                                    ==serie.ToLower().Trim());
            }
            else
            {
				valor = lista.Any(b => b.NumeroSerie.ToLower().Trim() 
                                    == serie.ToLower().Trim() && b.id!=id);
			}
            if (valor) {
                return Json(new { data = true });
            }
            return Json(new { data = false });
		}

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var productoDB = await _unidadTrabajo.Producto.Obtener(id);
            if (productoDB == null)
            {
                return Json(new { success = false, message = "Error al borrar el registro en la base de datos" });
            }
            //Borrar imagen
            string upload = _webHostEnvironment.WebRootPath + DS.ImagenRuta;
            var anteriorFile = Path.Combine(upload, productoDB.ImagenUrl);
            if (System.IO.File.Exists(anteriorFile))
            {
                System.IO.File.Delete(anteriorFile);
            }
            _unidadTrabajo.Producto.Remover(productoDB);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Producto elminiado con exito" });
        }
        #endregion
    }
}
