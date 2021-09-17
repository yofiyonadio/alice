$(document).ready(function () {
    var grid = $("#grid").kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: $("#urlPath").val() + "/ValidBudget/Read",
                    contentType: "application/json",
                    type: "POST",
                    cache: false
                },
                parameterMap: function (data, operation) {
                    return kendo.stringify(data)
                }
            },
            pageSize: 1000,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true,
            schema: {
                data: "Data",
                total: "Total",
                model: {
                    id: "ID",
                    fields: {
                        ID_PENGAJUAN: { type: "string", filterable: true, sortable: true, editable: true },
                        PIC: { type: "string", filterable: true, sortable: true, editable: true },
                        DivisiID: { type: "string", filterable: true, sortable: true, editable: true },
                        Kategori: { type: "string", filterable: true, sortable: true, editable: true },
                        Item: { type: "string", filterable: true, sortable: true, editable: true },
                        RequestDate: { type: "string", filterable: true, sortable: true, editable: true },
                        QTY: { type: "int", filterable: true, sortable: true, editable: true },
                    }
                }
            }
        },
        height: 450,
        filterable: true,
        resizable: true,
        sortable: true,
        pageable: true,
        groupable: true,
        editable: "popup",
        pageable: {
            refresh: true,
            buttonCount: 10,
            input: true,
            pageSizes: [10, 20, 50, 100, 1000, 100000],
            info: true,
            messages: {
            }
        },
        columns: [
            { command: [{ text: "Check", click: Check }], title: "Validasi", width: "40px" },
            {
                field: "ID_PENGAJUAN", title: "ID Pengajuan", width: "50px", hidden: true
            }, {
                field: "PIC", title: "PIC", width: "40px"
            }, {
                field: "DivisiID", title: "Divisi", width: "50px"
            }, {
                field: "Kategori", title: "Kategori", width: "50px"
            }, {
                field: "Item", title: "Item", width: "100px"
            }, {
                field: "RequestDate", title: "Pengajuan", width: "50px"
            }, {
                field: "QTY", title: "QTY", width: "50px"
            }
        ],
        dataBinding: function () {
            window.rowNo = (this.dataSource.page() - 1) * this.dataSource.pageSize();
        }
    });
});

var wnd_addUser;
wnd_addUser = $("#wnd_addUser").kendoWindow({
    title: "QTY detail",
    width: "900px",
    modal: true,
    visible: false,
    draggable: true,
    actions: [
        "Minimize",
        "Close"
    ],
    pinned: true
}).data("kendoWindow");

function Check(e) {
    wnd_addUser.center().open();
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));

    var grid = $("#gridQTY").kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: $("#urlPath").val() + "/ValidBudget/ReadQtyDetailApproved?sidpengaju=" + dataItem.ID_PENGAJUAN,
                    contentType: "application/json",
                    type: "POST",
                    cache: false,
                    complete: function () {
                        loadAproved();
                    }
                },
                parameterMap: function (data, operation) {
                    return kendo.stringify(data)
                }
            },
            pageSize: 15,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true,
            schema: {
                data: "Data",
                total: "Total"
            }
        },
        height: "480px",
        filterable: {
            extra: false
        },
        sortable: true,
        pageable: true,
        editable: "popup",

        pageable: {
            refresh: true,
            buttonCount: 10,
            input: true,
            pageSizes: [10, 20, 50, 100, 1000, 100000],
            info: true,
            messages: {
            }
        },
        columns: [

            { field: 'is_true', width: "60px", filterable: false, sortable: false, template: $('#tmplt_is_check_true').html(), headerTemplate: "<center>Approved</center>", selectable: true },
            { field: 'is_false', width: "60px", filterable: false, sortable: false, template: $('#tmplt_is_check_false').html(), headerTemplate: "<center>Rejected</center>", selectable: true },

            {
                field: "NRP", title: "NRP", width: "100px"
            }, {
                field: "NAMA", title: "Nama", width: "200px"
            }, {
                field: "POS_TITLE", title: "Posisi", width: "200px"
            }, {
                field: "EMAIL_INTERNET", title: "Email", width: "100px"
            }
        ],
        dataBinding: function () {
            window.rowNo = (this.dataSource.page() - 1) * this.dataSource.pageSize();
        }
    });

}


function doProsesAproval() {
    var arr_obj = [];
    var obj_check = 0;
    var sts_aproval = 0;
    var totalchecked = $("[type='checkbox']:checked").length;
    var data = $("#gridQTY").data("kendoGrid").dataSource.data();
    for (i = 0; i < data.length; i++) {
        var rows = data[i];
        console.log(rows);
        if ($("#IS_SELECTED_check_validasi_true" + rows.PID_PENGAJUAN_QTY).is(':checked').toString() == "true") {
            obj_check = 1;
            sts_aproval = 1;
        }
        if ($("#IS_SELECTED_check_validasi_false" + rows.PID_PENGAJUAN_QTY).is(':checked').toString() == "true") {
            obj_check = 0;
            sts_aproval = 99;
        }

        var i_cls_data = {
            PID_PENGAJUAN_QTY: rows.PID_PENGAJUAN_QTY,
            IDPENGAJUAN: rows.IDPENGAJUAN,
            STATUS_APROVAL: sts_aproval,
            APPROVAL_UNIT_FINAL: obj_check
        }
        arr_obj.push(i_cls_data);

    }
    console.log(arr_obj);
    $.ajax({
        url: $("#urlPath").val() + "/ValidBudget/UpdateApproval",
        contentType: "application/json",
        dataType: "json",
        type: "POST",
        data: JSON.stringify(arr_obj),
        complete: function (data) {
            alert(data.responseJSON.remarks);
        }
    });
}


function loadAproved() {
    var s = 0;
    $("#gridQTY tbody tr").each(function () {
        var data = $("#gridQTY").data("kendoGrid").dataSource.data();
        var rows = data[s];
        
        if (rows.APPROVAL_UNIT_FINAL == true) {
            $("#IS_SELECTED_check_validasi_true" + rows.PID_PENGAJUAN_QTY).prop("checked", true);
        } if (rows.APPROVAL_UNIT_FINAL == false && rows.STATUS_APROVAL == 99) {
            $("#IS_SELECTED_check_validasi_false" + rows.PID_PENGAJUAN_QTY).prop("checked", true);
        }
        s++;
    });
}
