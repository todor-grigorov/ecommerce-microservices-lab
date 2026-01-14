var dataTable;

$(document).ready(function () { loadDataTable(); });

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        oreder: [[0, "desc"]],
        "ajax": { url: "/order/getall" },
        "columns": [
            { data: 'orderHeaderId', "width": "5%" },
            { data: 'email', "width": "25%" },
            { data: "name", width: "20%" },           // Customer Name
            { data: "phone", width: "15%" },    // Customer Phone
            { data: "status", width: "10%" },    // Status
            { data: "orderTotal", width: "10%" },
            {                                         // Actions (blank header)
                data: "orderHeaderId",
                width: "10%",
                orderable: false,
                searchable: false,
                render: function (data) {
                    return `<div class="w-75 btn-group" role="group"><a class="btn btn-sm btn-primary mx-2" href="/order/orderDetails?orderId=${data}"><i class="bi bi-pencil-square"></i></a></div>`;
                }
            }
        ]
    })
}