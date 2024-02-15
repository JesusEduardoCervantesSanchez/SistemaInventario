using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
    public class Bodega
    {
        [Key]
        public int id { get; set; }
        [Required(ErrorMessage ="El campo nombre es requerido")]
        [MaxLength(60, ErrorMessage = "El nombre solo se compone de 60 caracteres como maximo")]
        public String Nombre { get; set; }
        [Required(ErrorMessage = "El campo descripción es requerido")]
        [MaxLength(100, ErrorMessage = "La descrición solo se compone de 100 caracteres como maximo")]
        public String Descripcion { get; set; }
        [Required(ErrorMessage = "El estado de la bodega es requerido")]
        public bool Estado { get; set; }
    }
}
