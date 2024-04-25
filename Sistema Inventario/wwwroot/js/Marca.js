    let datatable;

    $(document).ready(function () {
        loadDataTable();
    });

    function loadDataTable() {
        datatable = $('#tblDatos').DataTable({
            "ajax": {
                "url":"/Admin/Marca/ObtenerTodos"
            },
            "columns": [
                { "data": "nombre", "width": "20%" },
                { "data": "descripcion", "width": "40%" },
                {
                    "data": "estado",
                    "render": function (data) {
                        if (data == true) {
                            return "Activo";
                    } else {
                        return "Inactivo";
                    }
                },
                "width": "20%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Marca/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a onclick=Delete("/Admin/Marca/Delete/${data}") class="btn btn-danger text-white" style="cursos:pointer">
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
        title: "¿Estas seguro que deseas eliminar la marca?",
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