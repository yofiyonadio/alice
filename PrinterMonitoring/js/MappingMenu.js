$(document).ready(function () {
    var GroupDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: $("#urlPath").val() + "/AuthUser/AjaxGroup",
                contentType: "application/json",
                type: "POST",
                cache: false
            }
        },
        schema: {
            data: "Data",
            total: "Total"
        }
    });

    var GroupDataSource = $("#txtGrp").kendoDropDownList({
        dataTextField: "Deskripsi",
        dataValueField: "GP_ID",
        dataSource: GroupDataSource,
        optionLabel: "Pilih Group"
    });
    //loadGrid(); 

    $("#btnView").click(function (e) {
        e.preventDefault();
        var GroupID = $("#txtGrp").val();
        loadGrid(GroupID);
        ShowbtnUpdate();
    });

    HidebtnUpdate();
});

function HidebtnUpdate() {
    $("#btnUpdate").hide();
}

function ShowbtnUpdate() {
    $("#btnUpdate").show();
}

function loadGrid(sGPID) {
    var checkbox = "checkbox";
    var rowNo = 0;
    $("#grid").empty();
    var grid = $("#grid").kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: $("#urlPath").val() + "/AuthUser/AjaxRead?sGPID=" + sGPID,
                    contentType: "application/json",
                    type: "POST",
                    cache: false,
                },
                parameterMap: function (data, operation) {
                    return kendo.stringify(data)
                }
            },
            pageSize: 1000,
            serverPaging: true,
            schema: {
                data: "Data",
                total: "Total",
                model: {
                    id: "Primer",
                    fields: {
                        Primer: { type: "int", filterable: true, sortable: true, editable: true },
                        Id: { type: "number", filterable: true, sortable: true, editable: true },
                        Menu: { type: "string", filterable: true, sortable: true, editable: true },
                        Menu_link: { type: "number", filterable: true, sortable: true, editable: true },
                        Link: { type: "string", filterable: true, sortable: true, editable: true },
                        Urutan: { type: "number", filterable: true, sortable: true, editable: true },
                        GP_ID: { type: "int", filterable: true, sortable: true, editable: true },
                        Deskripsi: { type: "string", filterable: true, sortable: true, editable: true },
                        IsCheck: { type: "boolean", filterable: true, sortable: true, editable: true },
                        A: { type: "boolean", filterable: true, sortable: true, editable: true },
                        D: { type: "boolean", filterable: true, sortable: true, editable: true },
                        E: { type: "boolean", filterable: true, sortable: true, editable: true },
                        R: { type: "boolean", filterable: true, sortable: true, editable: true },
                    }
                }
            }
        },
        resizable: true,
        //filterable: true,
        pageable: true,
        height: 400,
        editable: "inline",
        columns: [
             {
                 title: "No",
                 width: "30px",
                 template: "#= ++rowNo #",
                 filterable: false
             }, {
                 field: "Is Allow",
                 width: "70px",
                 template: "<input  id=\"allow\" type=\"checkbox\" #= IsCheck ?\"checked\":\"\" #  />",
             },
            { field: "Menu", title: "Menu" },
            {
                title: "Add",
                field: "A",
                width: "70px",
                template: "<input id=\"add\" type=\"checkbox\" #= A == true ?\"checked\":\"\" #  />",
            }, {
                title: "Delete",
                field: "D",
                width: "70px",
                template: "<input id=\"delete\" type=\"checkbox\" #= D == true ?\"checked\":\"\" #  />",
            }, {
                title: "Edit",
                field: "E",
                width: "70px",
                template: "<input id=\"edit\" type=\"checkbox\" #= E == true ?\"checked\":\"\" #  />",
            }, {
                title: "Read",
                field: "R",
                width: "70px",
                template: "<input id=\"read\" type=\"checkbox\" #= R == true ?\"checked\":\"\" #  />",
            },
        ],
        dataBinding: function () {
            window.rowNo = (this.dataSource.page() - 1) * this.dataSource.pageSize();
        }
    });
}


function dirtyField(a, s) { }

$("#btnUpdate").click(function () {

    var arr_obj = []
    var grid = $("#grid").data("kendoGrid");
    var totalchecked = $("[id='allow']:checked").length;

    //var totalchecked = $("[type='checkbox']:checked").length;

    if (totalchecked <= 0) {
        alert("Belum ada data yang di pilih !");
    } else {

        var i_inc = 0;
        var param = "";


        $("#grid tbody tr").each(function () {
            var isChek = $('td input[id=allow]', this).is(':checked').toString(),
                isAdd = $('td input[id=add]', this).is(':checked'),
                isDelete = $('td input[id=delete]', this).is(':checked'),
                isEdit = $('td input[id=edit]', this).is(':checked'),
                isRead = $('td input[id=read]', this).is(':checked');
            var iIsChek, iIsAdd, iIsDelete, iIsEdit, iIsRead;
            var data = $("#grid").data().kendoGrid.dataSource.data();
            var rows = data[i_inc];

            //   if (isChek == "True" || isChek == "true" || isChek == "False" || isChek == "false") {

            if (isChek == "True" || isChek == "true") {
                iIsChek = 1;
            } else {
                iIsChek = 0;
            }

            var i_cls_data = {
                Primer: rows.Primer,
                GP_ID: rows.GP_ID,
                isChek: iIsChek,
                A: isAdd,
                D: isDelete,
                E: isEdit,
                R: isRead
            }
            arr_obj.push(i_cls_data);

            i_inc++;
        });
        $.ajax({
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            url: $("#urlPath").val() + "/AuthUser/AjaxUpdateCheckBox",
            data: JSON.stringify(arr_obj),
            success: function (response) {
                alert(response.remark)
                $("#grid").data("kendoGrid").dataSource.read()
            },

            error: function (jqXHR, textStatus, errorThrown) {

            }
        });
    }
});

