var wnd_addUser;

wnd_addUser = $("#wnd_addUser").kendoWindow({
    title: "Data Karyawan",
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

function back() {
    window.location = $("#urlPath").val() + "/MappingUser";
}

function distrkChange(e) {
    $("#btnInsert").attr("disabled", ($(e).val() == ""));
}

function profileChange(e) {
    var idProfil = $(e).val();
    var param = "";
    param += "sProfile=" + idProfil;

    $.ajax({
        type: "GET",
        url: $("#urlPath").val() + "/AddUser/getDistrikDesc?" + param,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#distrikId").empty();
            if (data.data.length != 1) {
                $('#distrikId').append('<option value="">[Select]</option>');
                $("#btnInsert").attr("disabled", true);
            } else {
                $("#btnInsert").attr("disabled", false);
            }
            $.each(data.data, function (key, r) {
                $('#distrikId').append('<option value="' + r.value + '">' + r.text + '</option>');
            });
        }
    });
}

function insert() {
    var nrp = $("#nrpId").val(),
		gp = $("#profileID").val(),
		distrik = $("#distrikId").val();

    if (nrp.trim() == '') {
        alert('NRP harus diisi.. !!');
        return false;
    }

    $.ajax({
        url: $("#urlPath").val() + "/AddUser/ajaxInsert",
        data: { "nrp": nrp, "gp": gp, "distrik": distrik },
        dataType: 'json',
        type: "POST",
        success: function (data) {
            if (data.status) {
                alert(data.message);
                $("#nrpId").val('');
                $("#distrikId").val('');
                $("#profileID").val('');
            } else {
                alert(data.error);
            }
        }
    });
}

function showPopup() {
    wnd_addUser.center().open();
    loadRecipient();
}

function loadRecipient() {
    var grid = $("#grid").kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: $("#urlPath").val() + "/AddUser/AjaxReadEmployee",
                    contentType: "application/json",
                    type: "POST",
                    cache: false
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
            {
                template: "<a href='javascript:void(0)' onClick='selects(\"#= NRP # \")'>Select</a>", width: "70px"
            }, {
                field: "NRP", title: "NRP", width: "100px"
            }, {
                field: "NAME", title: "Nama", width: "200px"
            }, {
                field: "POS_TITLE", title: "Posisi", width: "200px"
            }, {
                field: "DEPT_DESC", title: "Dept", width: "200px"
            }, {
                field: "DSTRCT_CODE", title: "Distrik", width: "100px"
            }
        ],
        dataBinding: function () {
            window.rowNo = (this.dataSource.page() - 1) * this.dataSource.pageSize();
        }
    });
}

function selects(nrp)
{
    console.log(nrp);
    wnd_addUser.close();
    $("#nrpId").val(nrp);
}