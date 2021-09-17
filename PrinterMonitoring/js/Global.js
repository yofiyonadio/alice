let yy_i = 1;
let prefixs = "#yyform_";

function adds() {
    return yy_i++
}

//-------------------------------------------------------------------------

function logger(log) {
    console.log(log)
}


function modalShow(el) {
    $(el).modal('show');
}

function modalHide(el) {
    $(el).modal('hide');
}


$(document).on('click', 'tr', el => {
    if ($(el.currentTarget).hasClass('costumColor')) {
        $(el.currentTarget).removeClass('costumColor')
    } else {
        $('tr').removeClass('costumColor')
        $(el.currentTarget).addClass('costumColor')
    }
})



$('.yy-qr-icon-form').click(res => {
    const getId = $(res.currentTarget).attr('yyFollow')
    const getRouter = $(res.currentTarget).attr('yyRouter')
    const getPrefix = $(res.currentTarget).attr('yyPrefix')
    qrBarcodeHandler($('#' + getId), getPrefix, getRouter)
})

function yyFormDropdown(option) {
    $(option.target).kendoDropDownList({
        optionLabel: option.label === undefined ? 'Pilih...' : option.label,
        dataTextField: "value",
        dataValueField: "key",
        dataSource: option.datas.map(res => {
            return {
                key: res.key === undefined ? res : res.key,
                value: res.value === undefined ? res : res.value
            }
        }),
        value: option.defaults
    });
}

function formCheck(prefix, array) {
    return new Promise((resolve, reject) => {
        const arr = []
        let getForm = prefix !== undefined ? getAllForm(prefix) : formatIdForm(array)
        getForm.map(val => {
            const className = $(val).attr("class").split(' ');
            const value = $(val).val()
            const isRequired = className.includes('yy-form-required')
            let type = $(val).attr('type')
            isRequired && value.length <= 0 ? arr.push(val) : null
            if (type === 'file' && value.length > 0) {
                fileValidation(val) !== 'FILE_VALID' ? arr.push(val) : null
            }
        })
        arr <= 0 ? resolve(true) : reject(arr)
        /*
        getFrom.forEach(val => {
            className = $(val).attr("class");
            logger(className)
        })
        */
        
        /*
        array.map(res => {
            const val = $('#' + res).val()
            val.length <= 0 ? arr.push('#' + res) : null;
        })
        arr <= 0 ? resolve(true) : reject(arr)
        */
    })
}

$(document).click(function () {
    yy_hide('.yy-notif-success')
});

$('.form-control').keydown(function () {
    textChange(this, 0).then(() => {
        $(this).removeClass('yy-form-invalid')
        yy_hide('.yy-form-invalid-alert')
    })
});

$('.form-control').change(function () {
    let type = $(this).attr('type')
    $(this).removeClass('yy-form-invalid')
    let ids = $(this).attr('id')
    if (type === 'file') {
        $(`[for=${ids}]`).removeClass('yy-form-invalid')
    }
    if (!($('#'+ids).data("kendoDropDownList") === undefined)) {
        $('#' + ids).parent().removeClass('yy-form-invalid')
    }
    yy_hide('.yy-form-invalid-alert')
});

function formInvalid(array) {
    yy_show('.yy-form-invalid-alert')
    array.map(val => {
        let type = $(val).attr('type')
        if(!($(val).data("kendoDropDownList") === undefined)) {
            $(val).parent().addClass('yy-form-invalid')
        } else if (!($(val).data("kendoDatePicker") === undefined)) {
            $(val).parent().addClass('yy-form-invalid')
        } else if (type === 'file') {
            $(`[for=${(val).split('#')[1]}]`).addClass('yy-form-invalid')
        } else {
            $(val).addClass('yy-form-invalid')
        }
    })
}

function formValid(array) {
    yy_show('#yy-form-invalid')
    formatIdForm(array).forEach(val => {
        let type = $(val).attr('type')
        $(val).removeClass('yy-form-invalid')
        if (type === 'file') {
            $(`[for=${(val).split('#')[1]}]`).removeClass('yy-form-invalid')
        }
        if (!($(val).data("kendoDropDownList") === undefined)) {
            $(val).parent().removeClass('yy-form-invalid')
        }
        if (!($(val).data("kendoDatePicker") === undefined)) {
            $(val).parent().removeClass('yy-form-invalid')
        }
    })
}


$('.yy-notif-close').click(() => {
    yy_hide('.yy-notif-success')
})

function yy_show(element) {
    $(element).removeClass('yy-nonvisible')
    $(element).addClass('yy-visible')
}

function yy_hide(element) {
    $(element).removeClass('yy-visible')
    $(element).addClass('yy-nonvisible')
}


function resetForm() {
    let dropdownlist = $("#yyform_license_number").data("kendoDropDownList")

    if (dropdownlist !== undefined) {
        let parrent = $("#yyform_license_number").parent().parent();
        dropdownlist.destroy();
        dropdownlist.wrapper.remove();
        parrent.prepend(`<input type="text" class="form-control yy-form-required" style="width:100%" id="yyform_license_number" autocomplete="off" readonly="readonly">`)
        orderFormElement(allForm)
    }
}

function disableForm(fields) {
    let newFields = formatIdForm(fields)
    for (let item of newFields) {
        let kendoDropDownList = $(item).data("kendoDropDownList")
        let kendoDatePicker = $(item).data("kendoDatePicker")
        if (kendoDropDownList !== undefined) {
            $(item).data("kendoDropDownList").enable(false);
        } else if (kendoDatePicker !== undefined) {
            $(item).data("kendoDatePicker").enable(false);
            $(item).data("kendoDatePicker").destroy();
        } else {
            $(item).attr("readonly", "readonly");
        }
    }
}

function enableForm(fields) {
    let newFields = formatIdForm(fields)
    for (let item of newFields) {
        let kendoDropDownList = $(item).data("kendoDropDownList")
        let kendoDatePicker = $(item).hasClass("yy-kendo-datepicker") //yy-kendo-datepicker
    //console.log(kendoDatePicker)
        if (kendoDropDownList !== undefined) {
            $(item).data("kendoDropDownList").enable(true);
        } else if (kendoDatePicker !== false) {
            kendoDatePickerCreate(item)
            //$(item).data("kendoDatePicker").enable(true);
        } else {
            $(item).removeAttr("readonly", "readonly");
        }
    }
}

function yyloadingHide() {
    $('.yy-loading').removeClass("shows")
}

function yyloadingShow() {
    $('.yy-loading').addClass("shows")
}

function formatIdForm(IdArray, prefix) {
    //logger(IdArray)
    //logger(prefix)
    IdArray = JSON.parse(JSON.stringify(IdArray).replaceAll("yyform_", "").replaceAll("#", ""))
    let prefixx = prefix === undefined ? "#yyform_" : ('#' + prefix + '_').replaceAll('__', '').replaceAll('##', '#');
    let results = IdArray.map(i => prefixx + i)
    return results
}

function alertjson(json) {
    alert(JSON.stringify(json))
}


function reOrderArray(priority) {
    priority = formatIdForm(priority)
    let newOrder = []
    //let match = allForm.filter(i => priority.includes(i));
    let notMatch = allForm.filter(i => !priority.includes(i));
    newOrder.push.apply(newOrder, priority)
    newOrder.push.apply(newOrder, notMatch)
    return newOrder
}

function orderFormElement(arrayForm) {
    arrayForm = JSON.parse(JSON.stringify(arrayForm).replaceAll("yyform_", "").replaceAll("#", ""))
    let newForm = []
    newForm.push.apply(newForm, arrayForm)
    newForm = newForm.reverse();
    for (let item of newForm) {
        let el = $(prefixs + item)
        let targetElement
        let parrent = el.parent()
        let getClass = $(parrent).attr('class')
        let valid = getClass.includes("col-sm-4");
        if (!valid) {
            let parrent = el.parent().parent()
            let getClass = $(parrent).attr('class')
            let valid = getClass.includes("col-sm-4");
            if (!valid) {
                let parrent = el.parent().parent().parent()
                let getClass = $(parrent).attr('class')
                let valid = getClass.includes("col-sm-4");
                if (!valid) {
                } else {
                    targetElement = parrent
                }
            } else {
                targetElement = parrent
            }
        } else {
            targetElement = parrent
        }
        $(targetElement).parent().prependTo('#form_new_license');
    }
}

function getAllForm(prefix) {
    let arr = []
    let el = $(".form-group").find().prevObject
    for (let item of el) {
        let child = $(item).find("div")
        let childs = $(child).find("input")
        if (childs.length > 0) {
            childs = childs
        } else {
            childs = $(child).find("textarea")
        }
        let getId = "#" + $(childs).attr('id')
        !getId.includes(undefined) ? prefix === undefined ? arr.push(getId) : getId.includes(prefix)  ? arr.push(getId) : null : null
    }
    //console.log(prefix)
    return arr
}

function dateSql(date) {
    let dates = new Date(date)
    let dateFormat = dates.toLocaleDateString('id-ID')
    let newDate = new Date(dateFormat)
    let result = newDate.toLocaleDateString("en-CA")
    return result
}

function deleteRows(e) {
    i = 1;
    let grid = $("#grid").data("kendoGrid");
    let dataItem = grid.dataItem($(e.currentTarget).closest("tr"));
    grid.dataSource.remove(dataItem);
}

function isDate(date) {
    date === '' ? date = 'x' : date = date
    date === null ? date = 'x' : date = date
    let isInt = date * 1
    if (!isNaN(isInt)) {
        return false
    } else {
        let dateParse = Date.parse(date)
        if (isNaN(dateParse)) {
            return false
        } else {
            if (!Number.isInteger(dateParse)) {
                return false
            } else {
                return true
            }
        }
    }
}

function ajaxFetch(types, urls, e) {
    return new Promise((resolve, reject) => {
        let formData = new FormData();
        formData.append("datas", typeof e === "object" ? JSON.stringify(e) : String(e));
        $.ajax({
            type: types,
            dataType: "json",
            contentType: false,
            processData: false,
            url: $("#urlPath").val() + urls,
            data: formData,
            success: function (res) {
                resolve(res)
            },
            error: function (error) {
                reject(error)
            }
        });
    });
}

function isJson(string) {
    try {
        jsons = JSON.parse(string);
        return true
    } catch (e) {
        return false;
    }
}

function fetchs(types, urls, e, file) {
    return new Promise((resolve, reject) => {
        const url = $("#urlPath").val() + urls;
        let formData = new FormData();
        formData.append("datas", typeof e === "object" ? JSON.stringify(e) : String(e));
        if (file !== undefined) {
            formData.append("files", file)
        }
        const opts = {
            method: types,
            headers: {
                "Accept": "multipart/form-data"
            },
            body: formData
        };

        fetch(url, opts)
        .then(function (res) {
            res.text()
            .then(ress => {
                if (isJson(ress)) {
                    let jsons = JSON.parse(ress)
                    if (jsons.result === "SUCCESS") {
                        resolve(jsons)
                    } else {
                        reject(ress)
                    }
                } else {
                    reject(ress)
                }
            })
        })
        .catch(function (error) {
            reject(error)
        })
    });
}


function getFormObject(col, prefix) {
    col = formatIdForm(col);
    let obj = {};
    let i = 0;
    for (let item of col) {
        let replaceItem = (prefix === undefined ? item.replaceAll("yyform_", "") : item.replaceAll(prefix + "_", "").replaceAll(prefix, "")).replaceAll("#", "")
        obj[replaceItem] = $(item).val() ? $(item).val()[0] == "0" ? $(item).val() : Number.isNaN($(item).val() * 1) ? $(item).val() : parseInt($(item).val()) : null
        i++;
    }
    return obj
}

function setFormObject(obj, ignore, prefixx) {
    ignore !== undefined ? ignore = JSON.parse(JSON.stringify(ignore).replaceAll("yyform_", "").replaceAll("#", "")) : ignore = ignore
    ignore === undefined ? ignore = [] : ignore = ignore
    let keys = Object.keys(typeof obj === "object" ? obj : JSON.parse(obj))
    let keysFilter = keys.filter(i => !ignore.includes(i));
    let items = {}

    for (let item of keysFilter) {
        items[item] = obj[item]
    }

    let prefix = prefixx === undefined ? "#yyform_" : ('#' + prefixx + '_').replaceAll('__', '').replaceAll('##', '#');
    
    for (let item of Object.keys(items)) {
        let values = items[item]
        let kendoDropDownList = $(prefix + item).data("kendoDropDownList")
        let kendoDatePicker = $(prefix + item).data("kendoDatePicker")
        let type = $(prefix + item).attr('type')
        //logger(prefix + item)
        if (kendoDropDownList !== undefined) {
            let dataSource = $(prefix + item).data("kendoDropDownList").options.filter
            if (dataSource === "contains" &&  (prefix + item) !== '#yyform_license_number') {
               $(prefix + item).data("kendoDropDownList").setDataSource([{ key: values, value: values }]);
            }
            $(prefix + item).data("kendoDropDownList").value(values)
        } else if (kendoDatePicker !== undefined) {
            if (values) {
                $(prefix + item).data("kendoDatePicker").value(new Date(values))
            } else {
                $(prefix + item).data("kendoDatePicker").value(null)
            }
        } else if (type === 'file') {
            if (values) {
                $(`[for=${(prefix + item).split('#')[1]}]`).text(values)
            } else {
                clearFile()
            }
        } else {
            $(prefix + item).val(values)
        }
    }
}

function refreshFormObject(arr, prefix, ignore) {
    arr = formatIdForm(arr, prefix)
    ignore !== undefined ? arr.splice(arr.indexOf(ignore), 1) : null;
    for (let item of arr) {
        let kendoDropDownList = $(item).data("kendoDropDownList")
        let kendoDatePicker = $(item).data("kendoDatePicker")
        let type = $(item).attr('type')
        if (kendoDropDownList !== undefined) {
            $(item).data("kendoDropDownList").value(null);
        } else if (kendoDatePicker !== undefined) {
            $(item).val(null)
            $(item).data("kendoDatePicker").destroy();
            $(item).kendoDatePicker({
                format: "dd/MM/yyyy",
                dateInput: true,
                min: undefined,
                max: undefined
            });
        } else if (type === 'file') {
            clearFile()
        } else {
            $(item).val(null)
        }
    }
}

function textChange(ids, delay) {
    return new Promise((resolve, reject) => {
        let thiss = ids;
        clearTimeout($.data(this, 'scrollTimer'));
        $.data(this, 'scrollTimer', setTimeout(function () {
            //console.log("keyup run...")
            let val = $(thiss).val();
            //console.log(val);
            if (val.length > 0) {
                resolve()
            } else {
                reject()
            }
        }, delay === undefined ? 500 : delay));
    });
}

function initialLoadingForm() {
    let classBox = ".yy-loading-form-box"
    let el = $(classBox).find().prevObject
    for (let item of el) {
        let fields = $(item).find("input")
        let ids = $(fields).attr("id")
        if (fields.length > 0) {
            let idEl = ids + "-loading-form"
            let checkEl = $("#" + idEl).length;
            if (checkEl < 1) {
                $(item).append(`<div id="${idEl}" class="yy-loading-form"><div class="spinner-border text-primary"></div></div>`);
            }

        }
    }
}

function showLoadingForm(e) {
    let suffix = "-loading-form";
    e = e.replaceAll(suffix, "")
    $(e + suffix).addClass("shows");
}

function hideLoadingForm(e) {
    let suffix = "-loading-form";
    e = e.replaceAll(suffix, "")
    $(e + suffix).removeClass("shows");
}

function showLoading() {
    $('.yy-loading').addClass("shows")
}

function hideLoading() {
    $('.yy-loading').removeClass("shows");
}

function yyshow_cb() {
    return new Promise((resolve, reject) => {
        yy_show('.yy-loading')
        setTimeout(function () {
            resolve()
        }, 100);
    })
}

function pdfNewTab() {
    let reducer2 = Object.values(checkObj).reduce((first, item, index, array) => {
        first = [...first, ...item]
        return first
    }, [])
    let datas = Object.values(checkObj).reduce((first, item, index, array) => {
        first = [...first, ...item]
        return first
    }, [])
    if (datas.length > 0) {
        
        yyshow_cb().then(() => {
            generatePDF({
                datas: datas,
                autoPrint: false,
                save: false,
                target: 'NEW_TAB'
            }).then(() => {
                yy_hide('.yy-loading')
            })
        }) 
    }
}

//------------------------------------------ KENDO GRID -----------------------------------------------------

let checkObj = {}

function getElCol(elementArray, colTarget) {
    for (let item of elementArray) {
        let classes = item.className;
        if (classes === colTarget) {
            return item
        }
    }
}


function checkBoxHeader(thiss, colTarget) {
    let allCheck = document.getElementsByClassName('yy-checkbox')
    let index = 0;
    for (let item of allCheck) {
        if (thiss.checked) {
            let el = getElCol(document.getElementsByTagName('tbody')[0].children[index].children, colTarget).children[0].textContent
            item.checked = thiss.checked;
            checkObj[index] = [el]
        } else {
            item.checked = thiss.checked;
            delete checkObj[index];
        }
        index++
    }
    checkboxCallBack(thiss, 'header', checkObj, Object.values(checkObj).length)
    //console.log(JSON.stringify(checkObj))
}

function checkBoxHandler(thiss, colTarget) {
    let index = thiss.id.split('yy-checkbox-')[1]
    let el = getElCol(document.getElementsByTagName('tbody')[0].children[index].children, colTarget).children[0].textContent
    let checkheader = document.getElementById('yy-checkbox-header')
    checkheader.checked = false
    thiss.checked ? checkObj[index] = [el] : delete checkObj[index];
    checkboxCallBack(thiss, 'body', checkObj, Object.values(checkObj).length)
    //console.log(JSON.stringify(checkObj))
}

function createCheckbox(colTarget) {
    return {
        field: 'CheckBox',
        headerTemplate: '<input type="checkbox" id="yy-checkbox-header" class="yy-checkbox-global" onclick="checkBoxHeader(this, ' + "'" + colTarget + "'" + ');">',
        template: '<input type="checkbox" id="yy-checkbox-#= yy_i - 1 #" class="yy-checkbox yy-checkbox-global" onclick="checkBoxHandler(this, ' + "'" + colTarget + "'" + ');">',
        filterable: false,
        sortable: false,
        width: 50
    }
}

function buttonRefreshGrid(e) {
    loadgrid(e());
}



function generateColumns(obj, checkBox) {
    let columns = [];
    checkBox !== undefined ? columns.push(createCheckbox(checkBox)) : null
    for (let item of obj) {
        if (item.field !== undefined) {
            //{ field: "CheckBox", headerTemplate: '<input type="checkbox" id="yy-checkbox-header" onclick="checkBoxHeader(this);">', template: '<input type="checkbox" id="yy-checkbox-#= yy_i - 1 #" class="yy-checkbox" onclick="checkBoxHandler(this);">', width: 50 },
            columns.push({
                field: item.field,
                title: item.title,
                width: item.width !== undefined ? item.width : 130,
                format: item.format, // "{0:dd/MM/yyyy HH:mm:ss}" ====> Date Format
                sortable: item.sort !== undefined ? item.sort : false,
                filterable: item.filter !== undefined ? item.filter : false,
                hidden: item.hide,
                template: item.template !== undefined || item.type === "Date" ? item.template : "<a>#=(" + item.field + " == null) ? '' : " + item.field + " #</a>",
                headerTemplate: item.headerTemplate !== undefined ? item.headerTemplate : null,
                attributes: {
                    "class": item.field
                }
            })
        } else {
            if (item.command !== undefined) {
                columns.push(item);
            }
        }
    }
    //console.log(JSON.stringify(columns));
    return columns;
}

function generateFields(obj) {
    let fields = {};
    for (let item of obj) {
        if (item.field !== undefined) {
            fields[item.field] = { type: item.type !== undefined ? item.type : "string" };
        }
    }
    //console.log(JSON.stringify(fields));
    return fields;
}

function loadgrid(e) {
    yy_i = 1;
    checkObj = {}
    //$("#grid").data('kendoGrid').dataSource.data([]); // Delete Kendo datasource
    $("#grid").remove();
    $(".card-body").append('<div id="grid"></div>');
    e.prefixToolbars === undefined ? e.prefixToolbars = [] : null;
    e.suffixToolbars === undefined ? e.suffixToolbars = [] : null;
    var grid = $("#grid").kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: $("#urlPath").val() + e.url,
                    contentType: "application/json",
                    type: "POST",
                    cache: false,
                },
                parameterMap: function (data, operation) {
                    return kendo.stringify(data)
                }
            },
            pageSize: 50,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true,
            schema: {
                data: "Data",
                total: "Total",
                model: {
                    id: e.primaryKey,
                    fields: generateFields(e.fieldObj)
                }
            }
        },
        height: 500,
        filterable: true,
        sortable: true,
        pageable: true,
        resizable: true,
        editable: {
            mode: "popup"
        },
        toolbar: [
            {
                name: "back",
                template: '<button type="button" id="btn_grid_refresh" class="btn btn-info" style="position:relative;height:16px;width:16px" onclick="buttonRefreshGrid(' + e.btnRefresh + ')"><span class="tiny material-icons" style="position:absolute;top:50%;left:50%;transform:translate(-50%, -50%);">refresh</span></button>'
            },
            ...e.prefixToolbars,
            {
             name: "excel",
             imageClass: '<button type="button" button id="btn_export" class="btn btn-info"><span class="glyphicon glyphicon-export"></span> Export To Excel</button>'
            },
            {
                name: "back",
                template: '<span style="margin-left:400px;font-size:20px;font-weight:bold;">' + (e.title === undefined ? '' : e.title) + '</span>'
            },
            ...e.suffixToolbars
        ],

        excel: {
            fileName: "HoursMachine.xlsx",
            AllPages: true,
            filterable: true
        },
        columns: generateColumns(e.fieldObj, e.checkBox),
        dataBinding: function () {
            window.rowNo = (this.dataSource.page() - 1) * this.dataSource.pageSize();
            //let tes = $("#grid").data("kendoGrid").dataSource._page;
            let page = this.dataSource._page;
            let limit = this.dataSource.pageSize();
            yy_i = (page * limit) - limit + 1;
        },
        dataBound: e.dataBound,
        detailInit: e.detailInit
    });
}

function loadgridExpand(el, e, datas) {
    yy_i = 1;
    var grid = $("<div/>").appendTo(el.detailCell).kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: $("#urlPath").val() + e.url,
                    contentType: "application/json",
                    type: "POST",
                    cache: false,
                    data: {
                        datas: datas
                    }
                },
                parameterMap: function (data, operation) {
                    return kendo.stringify(data)
                }
            },
            pageSize: 10,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true,
            schema: {
                data: "Data",
                total: "Total",
                model: {
                    id: "id",
                    fields: generateFields(e.fieldObj)
                }
            }
        },
        width: 100,
        filterable: true,
        sortable: true,
        pageable: false,
        resizable: true,
        editable: {
            mode: "popup"
        },
        columns: generateColumns(e.fieldObj),
        dataBinding: function () {
            window.rowNo = (this.dataSource.page() - 1) * this.dataSource.pageSize();
            let page = this.dataSource._page;
            let limit = this.dataSource.pageSize();
            yy_i = (page * limit) - limit + 1;
        },
        dataBound: function (e) {
            var data = this.dataSource.view();
            yyloadingHide()
        },
        noRecords: {
            template: "<div style='height:60px;display:flex;flex-direction:column;justify-content:center;padding-left:20px'><div>Belum ada History.</div></div>"
        },
    });
}