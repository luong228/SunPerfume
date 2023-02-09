var dataTable;
$(document).ready(function () {
    loadDataTable();
})
function loadDataTable() {
    dataTable = $('#example1').DataTable({
        "ajax": {
            "url": "/Admin/User/GetAll"
        },
        "responsive": true, "lengthChange": false, "autoWidth": false,
        "columns": [
            { "data": "email", "width": "15%" },
            { "data": "name", "width": "25%" },
            { "data": "phoneNumber", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                        <a href="/Admin/User/Details?userId=${data}"
                        class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> 
                            Detail
                        </a>
                    </div>
                           `
                },
                "width": "5%"
            },
        ]
    });
}
