const currentController = BaseUrl + 'ProductType';


/// Table
function LoadTable()
{
    var table = $('#data_table').DataTable({
        "lengthMenu": [[5, 10, 25, 50, 100], ["5", "10", "25", "50", "100"]],
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        sortable: true,
        "orderMulti": false, // for disable multiple column at once
        dom: 'Bfrtip',
        aaSorting: [],
        buttons: [
            'pageLength'
        ],
        ajax: {
            url: currentController + '/GetProductTypes',
            type: "POST",            
        },
        select: {
            style: 'single',
            info: false
        },

        columns: [

        {
            data: "Seq_Id",
            name: "Seq_Id",
            render: function (data, type, row, meta) {
                var res = "";
                res += "<div style='text-align: left'>";
                res += "<button class='btn btn-default' id='delete_" + data + "' onclick='Delete(\"" + row.Product_Type_Code + "\")'><i class='glyphicon glyphicon-trash' title='Delete'></i></button>";
                res += "<button class='btn btn-default' id='edit_" + data + "' onclick='Edit_Show(\"" + row.Product_Type_Code + "\")'><i class='glyphicon glyphicon-pencil' title='Edit'></i></button>";
                res += "</div>";
                return res;
            },
            "searchable": false,
            "sortable": false
        },
            {
                data: "Product_Type_Code",
                name: "Product Type Code",
                "searchable": true
            },
            {
                data: "Type_Name",
                name: "Type Name",
                "searchable": true
            },
            {
                data: "Type_Description",
                name: "Type Description",
                "searchable": true
            },
        {
            data: "Create_Date",
            name: "Create Date",
            "searchable": true
        },
        {
            data: "Update_Date",
            name: "Update_Date",
            "searchable": true
        },
        {
            data: "Category_Name",
            name: "Category Name",
            "searchable": true
        }
        ],
    });
    $('.dataTables_filter').addClass('hidden');
    return table;
}

function UnLoadTable(table) {
    table.destroy();    
}
///Modals

///Filter Panel
function Delete(code) {
    $.confirm({
        title: 'Delete Product Type',
        content: '<p>Are you sure you want to delete this product Type?</p>'
                 ,
        //content: 'You are about to finalize this request, Continue!!',
        type: 'blue',
        typeAnimated: true,
        buttons: {
            Send: {
                text: 'Delete',
                btnClass: 'btn-danger',
                action: function () {                    
                    $.ajax({
                        url: currentController + '/DeleteProductType',
                        data: { code: code },
                        type: "GET"
                    }).done(function (data) {
                        //alert(data.IsSuccess);
                        if (data.result == 0) {
                            alert('Deleted Successfully')
                            table.ajax.reload();
                        }
                        else {
                            alert("Error: " + data.message)
                        }
                    });
                }
            },            
            Cancel: {
                text: 'Cancel',
                btnClass: 'btn-default',
                action: function () {
                }
            }
        }
    });
}

///ImportFromUnits Panel
function Create_Show() {
    $.ajax({
        url: currentController + '/CreateProductType',
        type: 'GET'
    }).done(function (data) {
        $('#CreatePanel .modal-body').html(data);
        $('#CreatePanel').modal();
    }).error(function (data) {
        alert('Error');
        $('#CreatePanel').modal('hide');

    });    
}
function Create_Continue() {
    $.ajax({
        url: currentController + '/CreateProductType',
        data: $("#form_CreateProductType").serialize(),
        type: 'POST'        
    }).done(function (data) {
        if (data.result == 0) {
            alert('Created Successfully')
            $('#CreatePanel').modal('hide');
            table.ajax.reload();
        }
        else {
            alert("Error!");
        }
    }).error(function (data) {
        alert("Error!");
        $('#CreatePanel').modal('hide');
        
    });
    
}

function Edit_Show(code) {
    $.ajax({
        url: currentController + '/EditProductType?code=' + code,
        type: 'GET',
    }).done(function (data) {
        $('#EditPanel .modal-body').html(data);
        $('#EditPanel').modal();
    }).error(function (data) {
        alert('Error');
        $('#EditPanel').modal('hide');

    });
}
function Edit_Continue() {
    $.ajax({
        url: currentController + '/EditProductType',
        data: $("#form_EditProductType").serialize(),
        type: 'POST'
    }).done(function (data) {
        if (data.result == 0) {
            alert('Created Successfully')
            $('#EditPanel').modal('hide');
            table.ajax.reload();
        }
        else {
            alert("Error!");
        }
    }).error(function (data) {
        alert("Error!");
        $('#CreatePanel').modal('hide');

    });

}
