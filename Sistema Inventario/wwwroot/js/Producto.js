    let datatable;

    $(document).ready(function () {
        loadDataTable();
    });

    function loadDataTable() {
        datatable = $('#tblDatos').DataTable({
            "ajax": {
                "url":"/Admin/Producto/ObtenerTodos"
            },
            "columns": [
                { "data": "numero serie"},
                { "data": "descripcion" },
                { "data": "categoria.nombre" },
                { "data": "marca.nombre" },
                {
                    "data": "precio", "className": "text-end",
                    "render": function (data) {
                        var d = data.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
                        return d
                    }
                },
                {
                    "data": "estado",
                    "render": function (data) {
                        if (data == true) {
                            return "Activo";
                    } else {
                        return "Inactivo";
                    }
                },
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Producto/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a onclick=Delete("/Admin/Producto/Delete/${data}") class="btn btn-danger text-white" style="cursos:pointer">
                                <i class="bi bi-trash"></i>
                            </a>
                        </div>
                    `;
                },
                "width": "20%"
            }
            ],
            "language": {
                "url": "//cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/Spanish.json"
            }
    });

}

function Delete(url) {
    swal({
        title: "¿Estas seguro que deseas eliminar el producto?",
        text: "Este registro no se podra recuperar",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((borrar) => {
        if (borrar) {
            $.ajax({
                type: "POST",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        //actualizar la tabla
                        datatable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}