/*!
 * Datatable JavaScript Library v1.5.8.3
 * 2018-01-17
 */
"use strict";;
var tableFilter = {
    dateFormatFilter: function (value, format) {
        if (bs.isUndefinedOrNull(value)) {
            return "";
        } else if (value == "1900-01-01T00:00:00") {
            return "";
        } else {
            if (!bs.isUndefinedOrNull(format)) {
                var length = format[0].trim().length;
                return value.replace(/T/g, ' ').substr(0, length);;
            } else {
                if (value.length >= 19) {
                    return value.replace(/T/g, ' ').substr(0, 19);
                } else {
                    return value.replace(/T/g, ' ');
                }
            }
        }
    },
    nullOrUnderfinedFilter: function (value) {
        if (bs.isUndefinedOrNull(value)) {
            return '';
        } else {
            return value;
        }
    },
    isStatusFilter: function (value) {
        if (value == 1) {
            return "启用";
        } else {
            return "停用";
        }
    },
    isWorkStatusFilter: function (value) {
        if (value == "1") {
            return "在职";
        } else {
            return "离职";
        }
    }
}

function DataTable(el, options) {
    var defaults = {
        title: "",
        key: "ID",
        sort: {
            isNeedSort: false,
            sort: 'desc',
            columnName: ""
        },
        initCallBack: null,
        ajax: {
            isNeedSerialize: true,
            isNeedLoading: true,
            url: "",
            methods: "post",
            query: {},
            callback: null,
            list: [],
        },
        tables: {
            widthRo: 1,
            isNeedHead: true,
            isNeedOperation: true,
            columns: [],
            heads: [],
            operations: [],
            rowsEvent: {
                isNeedRowsEvent: false,
                callback: null
            },
            columnsEvent: {
                list: [],
                callback: null,
            },
            style: {
                key: 'evenAndOdd',
                val: {
                    even: "",
                    odd: "#F7F7F7;",
                }
            },
            leftFixed: 0,
            rightFixed: 0
        },
        multiSelect: {
            isNeedMultiSelect: false,
            selectMode: 'mutil',
            selectObjList: []
        },
        page: {
            isNeedPage: true,
            totalPages: 0,
            pageIndex: 1,
            pageSize: 15,
            dataCount: 0
        },
        complateCallBack: null
    }
    if (window.jQuery == undefined) {
        throw '未引入jquery';
    }
    if (window.bs == undefined) {
        throw '未引入common';
    }
    var opts = bs.extend(true, {}, defaults, options);
    var rootNode = null;
    this.init = function (data) {
        if (bs.isObject(data)) {
            opts.ajax.query = bs.cloneDeep(true, {}, opts.ajax.query, data);
        }
        if (bs.isUndefinedOrNull(rootNode)) {
            if (typeof el === "object") {
                rootNode = el;
            } else if (typeof el === "string") {
                rootNode = document.querySelector(el);
            } else {
                bs.error("参数异常！");
                return false;
            }
        }
        rootNode.innerHTML = "";
        var contentDom = document.createElement("div");
        contentDom.className = "datatables-content";
        if (opts.ajax.isNeedLoading) {
            contentDom.appendChild(modalRender());
        }
        if (opts.tables.isNeedHead) {
            var headOperDom = document.createElement("div");
            headOperDom.className = "heads";
            headOperDom.innerHTML = '<div class="title">' + opts.title + '</div><div class="operation">' + datatableHeadRender() + '</div>';
            $bs(headOperDom).findAll(' .heads .operation a').on('click', function (e, v, i) {
                if (e.target.getAttribute("disabled") == "true") {
                    return;
                }
                opts.tables.heads[e.target.name].callback(v, e);
            })
            contentDom.appendChild(headOperDom);
        }
        var bodyDom = document.createElement("div");
        bodyDom.className = "list-content";
        bodyDom.style.overflowY = "hidden";
        bodyDom.style.overflowX = "auto";
        bodyDom.innerHTML = '<table class="main-table" style="width:' + opts.tables.widthRo * 100 + '%"><thead>' + tableHeadRender() + '</thead><tbody></tbody></table></div>';
        rootNode.appendChild(contentDom).appendChild(bodyDom);
        if (opts.initCallBack) {
            opts.initCallBack();
        }
        if (opts.sort.isNeedSort) {
            tableSort();
        }
        getData();
    }

    function modalRender() {
        var modalDom = document.createElement("div");
        modalDom.className = 'bs-loading-mask';
        modalDom.innerHTML = '<div class="bs-loading-spinner">\
        <svg viewBox="25 25 50 50" class="circular"><circle cx="50" cy="50" r="20" fill="none" class="path"></circle></svg> <p class="bs-loading-text">正在加载中</p></div>';
        return modalDom;
    }

    function datatableHeadRender() {
        var headBuffer = new bs.stringBuffer();
        opts.tables.heads.forEach(function (v, k) {
            if (!bs.isUndefinedOrNull(v.hide)) {
                if (!v.hide) {
                    headBuffer.append('<a class="bs-btn btn_active" href="javascript:;" name="' + k + '" title="' + v.text + '">' + v.text + '</a >');
                }
            } else if (!bs.isUndefinedOrNull(v.disabled)) {
                headBuffer.append('<a class="bs-btn ' + (v.disabled ? 'btn_disabled' : 'btn_active') + '" ' + (v.disabled ? 'disabled=true' : '') + ' href="javascript:;" name="' + k + '" title="' + v.text + '">' + v.text + '</a >');
            } else {
                headBuffer.append('<a class="bs-btn btn_active" href="javascript:;" name="' + k + '" title="' + v.text + '">' + v.text + '</a >');
            }
        })
        return headBuffer.toString();
    }

    function tableHeadRender() {
        var headBuffer = new bs.stringBuffer();
        headBuffer.append("<tr>")
        if (opts.multiSelect.isNeedMultiSelect) {
            headBuffer.append("<th style='width:50px;' type='checkbox'>" + (opts.multiSelect.selectMode == "single" ? "" : "<div><input type='checkbox' name='all' value='all' />全选</div>") + "</th>");
        }
        opts.tables.columns.forEach(function (v, k) {
            headBuffer.append("<th " + ((v.type || "").length > 0 ? ('type = ' + v.type) : "") + " style='" + (v.style || "") + "' class='" + (v.class || "") + "'>" + (opts.sort.isNeedSort ? ("<em>" + v.text + "</em>") : v.text) + " <input type='hidden' value='" + v.value + "'/></th>");
        })
        if (opts.tables.isNeedOperation) {
            headBuffer.append("<th style='width: " + (opts.tables.operations.length * 45) + "px;'>操作</th>");
        }
        headBuffer.append("</tr>");
        return headBuffer.toString();
    }

    function tableSort() {
        // var i = opts.multiSelect.isNeedMultiSelect ? 1 : 0;
        // for (i; i < document.querySelectorAll(el + ' .list-content th').length; i++) {
        //     document.querySelectorAll(el + ' .list-content th')[i].onclick = (function (v) {
        //         return function (e) {
        //             if (bs.hasClass(v.querySelector("em"), "selected")) {
        //                 v.querySelector("em").removeClass("selected").addClass("current");
        //                 $(this).siblings().find("em").removeClass("selected").removeClass("current");
        //                 opts.sort.sort = 'desc';
        //             } else {
        //                 bs.addClass(bs.removeClass(v.querySelector("em"), "current"), "selected")
        //                 // $(this).find("em").removeClass("current").addClass("selected");
        //                 bs.siblings(v)
        //                 $(this).siblings().find("em").removeClass("selected").removeClass("current");
        //                 opts.sort.sort = 'asc';
        //             }
        //             opts.sort.columnName = $(this).find('input').val();
        //         }
        //     }(document.querySelectorAll(el + ' .list-content th')[i]))
        // }
    }

    function getData(flag) {
        if (opts.page.isNeedPage) {
            opts.ajax.query.pageIndex = opts.page.pageIndex;
            if (bs.isUndefinedOrNull(flag)) {
                opts.ajax.query.pageSize = 15;
            } else {
                opts.ajax.query.pageSize = opts.page.pageSize;
            }
        }
        if (opts.sort.isNeedSort) {
            opts.ajax.query.sort = opts.sort.sort;
            opts.ajax.query.columnName = opts.sort.columnName;
        }
        if (opts.ajax.isNeedLoading) {
            rootNode.querySelector(".bs-loading-mask").style.opacity = 0.5;
            rootNode.querySelector(".bs-loading-mask").style.display = "block";
        }
        $.ajax({
            url: opts.ajax.url,
            type: opts.ajax.methods,
            dataType: 'json',
            contentType: opts.ajax.isNeedSerialize ? 'application/json; charset=utf-8' : 'application/x-www-form-urlencoded; charset=UTF-8',
            data: opts.ajax.isNeedSerialize ? JSON.stringify({
                model: opts.ajax.query
            }) : opts.ajax.query,
            success: function (data) {
                if (opts.ajax.isNeedLoading) {
                    $bs(rootNode.querySelector(".bs-loading-mask")).fadeOut(20, 0, 50);
                }
                if (opts.ajax.callback) {
                    var obj = opts.ajax.callback(data);
                    opts.ajax.list = obj.list;
                    ajaxSuccess();
                    if (opts.page.isNeedPage) {
                        opts.page.totalPages = obj.totalPages;
                        opts.page.dataCount = obj.dataCount;
                        pageRender();
                        if (opts.multiSelect.isNeedMultiSelect && opts.multiSelect.selectMode != "single") {
                            if (rootNode.querySelectorAll(".list-content .main-table tbody input[type='checkbox']:checked").length == opts.page.pageSize) {
                                rootNode.querySelector("input[name='all']").checked = true;
                            } else {
                                rootNode.querySelector("input[name='all']").checked = false;
                            }
                        }
                    }
                } else {
                    bs.error("回调方法异常");
                    return false;
                }
            },
            error: function (e, a, v) {
                var evt = window.top.event || top.arguments.callee.caller.arguments[0];
                var src = evt.srcElement || evt.target;
                if (src.outerHTML != "<i class=\"sysmenu\"></i>") {
                    window.top.bs.error("请求异常");
                }
                return false;
            }
        })
    }

    function ajaxSuccess() {
        if (opts.ajax.list.length > 0) {
            dataRender();
        } else {
            dataRender('noData');
        }
    }

    function pageRender() {
        $bs(rootNode.firstElementChild).pager({
            pageIndex: opts.page.pageIndex,
            pageSize: opts.page.pageSize,
            dataCount: opts.page.dataCount,
            totalPages: opts.page.totalPages,
            isNeedSelect: true,
            callback: function (pageIndex, pageSize) {
                opts.page.pageSize = pageSize;
                opts.page.pageIndex = pageIndex;
                getData(1);
            }
        });
    }

    function dataRender(flag) {
        var tbody = rootNode.querySelector("tbody");
        if (rootNode.querySelector(".leftFixedP")) {
            rootNode.querySelector(".leftFixedP").remove();
        }
        if (rootNode.querySelector(".rightFixedP")) {
            rootNode.querySelector(".rightFixedP").remove();
        }
        if (flag == 'noData') {
            var tbodyBuffer = new bs.stringBuffer();
            tbodyBuffer.append("<tr><td colspan=" + (opts.tables.columns.length + Number(opts.tables.isNeedOperation ? 1 : 0) + Number(opts.multiSelect.isNeedMultiSelect ? 1 : 0)) + ">暂无数据</td></tr>")
            tbody.innerHTML = tbodyBuffer.toString();
        } else {
            var tbodyBuffer = new bs.stringBuffer();
            opts.ajax.list.forEach(function (v, k) {
                var rowbackgroundColor = ";background-color:" + (opts.tables.style.key == "evenAndOdd" ? (k % 2 == 0 ? opts.tables.style.val.even : opts.tables.style.val.odd) : "");
                tbodyBuffer.append("<tr data-rowid='" + v[opts.key] + "' name=" + k + " style='" + rowbackgroundColor + "'>")
                if (opts.multiSelect.isNeedMultiSelect) {
                    tbodyBuffer.append("<td><input type='checkbox' name=" + v[opts.key] + " value='" + k + "'/></td>"); //v[opts.key]
                }
                opts.tables.columns.forEach(function (value, key) {
                    if (value.type == "image") {
                        tbodyBuffer.append("<td " + (bs.isEmpty(value.type) ? "" : ('type="' + value.type + '"')) + " data-sort='" + key + "' data-columns=" + value.value + "  class='" + (value.className || "") + "'><img style='" + (value.style || "") + "' src=" + (value.filter ? value.filter(v[value.value], value.para) : v[value.value]) + "></td>")
                    } else if (value.type == "tags") {
                        tbodyBuffer.append("<td " + (bs.isEmpty(value.type) ? "" : ('type="' + value.type + '"')) + " data-sort='" + key + "'  data-columns=" + value.value + "  style='" + (value.style || "") + (value.textAlign ? (';text-align:' + value.textAlign + ';padding:0 5px;') : '') + "' class='" + (value.className || "") + (bs.isEmpty(String(v[value.value])) ? "" : " title-tags") +
                            "'><p style=" + rowbackgroundColor + ">" + (value.filter ? value.filter(v[value.value], value.para) : v[value.value]) + "</p></td > ") //<div>" + v[value.value] + "</div>
                    } else if (value.type == "link") {
                        tbodyBuffer.append("<td " + (bs.isEmpty(value.type) ? "" : ('type="' + value.type + '"')) + " data-sort='" + key + "' data-columns=" + value.value + "  style='" + (value.style || "") + (value.textAlign ? (';text-align:' + value.textAlign + ';padding:0 5px;') : '') + "' class='" + (value.className || "") + "'><a class='link-callback' id=" + key + " name=" + k + ">" + (value.filter ? value.filter(v[value.value], value.para) : v[value.value]) + "</a></td>")
                    } else if (value.type == "title") {
                        tbodyBuffer.append("<td " + (bs.isEmpty(value.type) ? "" : ('type="' + value.type + '"')) + " data-sort='" + key + "' data-columns=" + value.value + "  style='" + (value.style || "") + (value.textAlign ? (';text-align:' + value.textAlign + ';padding:0 5px;') : '') + "' title='" + v[value.value] + "'  class='" + (value.className || "") + "'>" + (value.filter ? value.filter(v[value.value], value.para) : v[value.value]) + "</td>");
                    } else {
                        tbodyBuffer.append("<td " + (bs.isEmpty(value.type) ? "" : ('type="' + value.type + '"')) + " data-sort='" + key + "' data-columns=" + value.value + "  style='" + (value.style || "") + (value.textAlign ? (';text-align:' + value.textAlign + ';padding:0 5px;') : '') + "' class='" + (value.className || "") + "'>" + (value.filter ? value.filter(v[value.value], value.para) : v[value.value]) + "</td>");
                    }
                })
                if (opts.tables.isNeedOperation) {
                    tbodyBuffer.append("<td>");
                    opts.tables.operations.forEach(function (value, key) {
                        if (bs.isUndefinedOrNull(value.filter)) {
                            tbodyBuffer.append("<a title=" + value.text + " id=" + key + "  key=" + v[opts.key] + " name=" + k + " class='datatable-operation-link " + (value.className || "") + "' " + (value.copyColumnName ? ('data-clipboard-text="' + v[value.copyColumnName] + '"') : '') + ">" + value.text + "</a>")
                        } else {
                            if (v[value.filter]) {
                                tbodyBuffer.append("<a title=" + value.text + " id=" + key + "  key=" + v[opts.key] + "  name=" + k + " class='datatable-operation-link " + (value.className || "") + "' " + (value.copyColumnName ? ('data-clipboard-text="' + v[value.copyColumnName] + '"') : '') + ">" + value.text + "</a>")
                            }
                        }
                    })
                    tbodyBuffer.append("</td>");
                }
                tbodyBuffer.append("</tr>");
            })
            tbody.innerHTML = tbodyBuffer.toString();
            if (opts.tables.leftFixed > 0) {
                var leftDom = document.createElement("table");
                leftDom.className = 'leftFixed';
                leftDom.style.position = 'absolute';
                leftDom.style.backgroundColor = "#fff";
                leftDom.style.zIndex = 10;
                leftDom.style.top = 0;
                leftDom.style.left = 0;
                leftDom.style.boxShadow = "1px 0px 2px #DDDDDD";
                if (opts.tables.isNeedHead) {
                    leftDom.style.marginTop = "50px";
                }
                leftDom.style.height = rootNode.querySelector(" .main-table").offsetHeight + "px";
                var width = 0;
                var leftHtml = '<thead><tr>';
                var checkboxwidth = 0;
                for (var i = 0; i < opts.tables.leftFixed; i++) {
                    width += rootNode.querySelectorAll(" .main-table thead tr th")[i].offsetWidth;
                    var type = rootNode.querySelectorAll(" .main-table thead th")[i].getAttribute("type");
                    if (type == "checkbox") {
                        checkboxwidth = 50;
                    }
                    leftHtml += '<th ' + (bs.isEmpty(type) ? "" : ('type="' + type + '"')) + ' style="' + (i == 0 ? (checkboxwidth > 0 ? "width:50px" : "") : '') + '" class="' +
                        rootNode.querySelectorAll(" .main-table thead tr th")[i].className + '">' +
                        rootNode.querySelectorAll(" .main-table thead tr th")[i].innerHTML + '</th>';
                }
                leftDom.style.width = width + "px";
                leftHtml += '</tr></thead><tbody>'
                for (var i = 0; i < rootNode.querySelectorAll(" .main-table tbody tr").length; i++) {
                    leftHtml += '<tr style="background-color:' + rootNode.querySelectorAll(" .main-table tbody tr")[i].style.backgroundColor + ';height:' + (rootNode.querySelectorAll(" .main-table tbody tr")[i].offsetHeight - 0.6) + 'px;">'
                    for (var j = 0; j < opts.tables.leftFixed; j++) {
                        leftHtml += '<td  class="' +
                            rootNode.querySelectorAll(" .main-table tbody tr")[i].children[j].className + '">' +
                            rootNode.querySelectorAll(" .main-table tbody tr")[i].children[j].innerHTML + "</td>";
                    }
                    leftHtml += '</tr>'
                }
                leftHtml += '</tbody>'
                leftDom.innerHTML = leftHtml;
                var els = document.createElement("div");
                els.className = "list-content leftFixedP";
                leftDom.querySelector("thead tr").style.height = rootNode.querySelector(" .main-table thead tr").offsetHeight + 'px';
                rootNode.querySelector(' .datatables-content').appendChild(els).appendChild(leftDom);
            }
            if (opts.tables.rightFixed > 0) {
                var rightDom = document.createElement("table");
                rightDom.className = 'rightFixed';
                rightDom.style.position = 'absolute';
                rightDom.style.zIndex = 10;
                rightDom.style.top = 0;
                rightDom.style.right = '-1px';
                if (opts.tables.isNeedHead) {
                    rightDom.style.marginTop = "50px";
                }
                rightDom.style.boxShadow = "-1px 0px 2px #DDDDDD";
                rightDom.style.backgroundColor = "#fff";
                rightDom.style.height = rootNode.querySelector(".main-table").offsetHeight + "px";
                var width = 0;
                var rightHtml = '<thead><tr>';
                var len = rootNode.querySelectorAll(".main-table thead tr th").length - 1;
                for (var i = opts.tables.rightFixed; i > 0; i--) {
                    var type = rootNode.querySelectorAll(".main-table thead th")[len - i + 1].getAttribute("type");
                    width += rootNode.querySelectorAll(".main-table thead tr th")[len - i + 1].offsetWidth;
                    rightHtml += '<th ' + (type ? ("type=" + type) : '') + ' class="' +
                        rootNode.querySelectorAll(".main-table thead tr th")[len - i + 1].className + '">' +
                        rootNode.querySelectorAll(".main-table thead tr th")[len - i + 1].innerHTML + '</th>';
                }
                rightDom.style.width = width + "px";
                rightHtml += '</tr></thead><tbody>'

                for (var i = 0; i < rootNode.querySelectorAll(".main-table tbody tr").length; i++) {
                    rightHtml += '<tr style="background-color:' + rootNode.querySelectorAll(".main-table tbody tr")[i].style.backgroundColor + ';height:' + (rootNode.querySelectorAll(" .main-table tbody tr")[i].offsetHeight - 0.65) + 'px;">'
                    for (var j = opts.tables.rightFixed - 1; j >= 0; j--) {
                        rightHtml += '<td  class="' +
                            rootNode.querySelectorAll(".main-table tbody tr")[i].children[len - j].className + '">' +
                            rootNode.querySelectorAll(".main-table tbody tr")[i].children[len - j].innerHTML + "</td>";
                    }
                    rightHtml += '</tr>'
                }
                rightHtml += '</tbody>'
                rightDom.innerHTML = rightHtml;
                var els = document.createElement("div");
                els.className = "list-content rightFixedP";
                rightDom.querySelector("thead tr").style.height = rootNode.querySelector(".main-table thead tr").offsetHeight + 'px';
                rootNode.querySelector('.datatables-content').appendChild(els).appendChild(rightDom);
            }
            $bs(rootNode).findAll(' tbody .datatable-operation-link').on('click', function (e, v, i) {
                e.stopPropagation();
                if (opts.tables.operations[e.target.id].callback) {
                    opts.tables.operations[e.target.id].callback(opts.ajax.list[e.target.name], e);
                }
            })
            $bs(rootNode).findAll(' tbody .link-callback').on('click', function (e, v, i) {
                e.stopPropagation();
                if (opts.tables.columns[e.target.id].linkCallback) {
                    opts.tables.columns[e.target.id].linkCallback(opts.ajax.list[e.target.name], e)
                }
            })
            opts.tables.columnsEvent.list.forEach(function (v, k) {
                $bs(tbody).findAll("tr").each(function (key, row) {
                    $bs(row.cells[v - 1]).addClass("click-active");
                    row.cells[v - 1].setAttribute("title", row.cells[v - 1].innerText)
                    row.cells[v - 1].onclick = function (e) {
                        e.stopPropagation();
                        var columnData = opts.ajax.list[this.parentNode.getAttribute("name")];
                        var type = opts.tables.columns[Number(this.getAttribute("data-sort"))].type;
                        if (opts.multiSelect.isNeedMultiSelect) {
                            var isSelect = false;
                            for (var i = 0; i < opts.multiSelect.selectObjList.length; i++) {
                                if (columnData[opts.key] == opts.multiSelect.selectObjList[i][opts.key]) {
                                    isSelect = true;
                                    break;
                                }
                            }
                            opts.tables.columnsEvent.callback(columnData, this.getAttribute("data-columns"), e, this, type, isSelect)
                        } else {
                            opts.tables.columnsEvent.callback(columnData, this.getAttribute("data-columns"), e, this, type)
                        }
                    }
                })
            })
            if (opts.tables.rowsEvent.isNeedRowsEvent) {
                $bs(el).findAll(' tbody tr').on('click', function (e, v, i) {
                    e.stopPropagation();
                    opts.tables.rowsEvent.callback(opts.ajax.list[e.target.parentNode.getAttribute("name")], e)
                })
            }
            $bs(rootNode.querySelectorAll("tbody input[type='checkbox']")).forEach(function (key, value) {
                var index = opts.multiSelect.selectObjList.findIndex(function (o) {
                    return o[opts.key] == value.name;
                })
                if (index != -1) {
                    value.checked = true;
                }
            })
            multiSelect();
        }
        if (opts.tables.leftFixed > 0 || opts.tables.rightFixed > 0) {
            document.body.onresize = function (e) {
                var heights = [];
                var width = 0;
                $bs(rootNode.querySelectorAll(".main-table tbody tr")).forEach(function (k, v) {
                    heights.push(v.offsetHeight);
                })
                if (rootNode.querySelector(".leftFixedP")) {
                    width = 0;
                    $bs(rootNode.querySelectorAll(".leftFixedP table tbody tr")).forEach(function (k, v) {
                        v.style.height = heights[k] + "px"
                    });
                    for (var i = 0; i < opts.tables.leftFixed; i++) {
                        width += rootNode.querySelectorAll(".main-table thead tr th")[i].offsetWidth;
                    }
                    rootNode.querySelector(" .leftFixedP table").style.width = width + "px";
                    rootNode.querySelector(" .leftFixedP table thead tr").style.height = (rootNode.querySelector(" .main-table thead tr").offsetHeight - 1) + 'px';
                    rootNode.querySelector(" .leftFixedP table").style.height = rootNode.querySelector(" .main-table").offsetHeight + "px";
                }
                if (rootNode.querySelector(" .rightFixedP")) {
                    width = 0;
                    $bs(" .rightFixedP table tbody tr").forEach(function (k, v) {
                        v.style.height = heights[k] + "px"
                    })
                    var len = rootNode.querySelectorAll(" .main-table thead tr th").length;
                    for (var i = opts.tables.rightFixed; i > 0; i--) {
                        width += convertPiexlToNum(rootNode.querySelectorAll(" .main-table thead tr th")[len - i]);
                    }
                    rootNode.querySelector(" .rightFixedP table").style.width = width + "px";
                    rootNode.querySelector(" .rightFixedP table thead tr").style.height = (rootNode.querySelector(" .main-table thead tr").offsetHeight - 1) + 'px';
                    rootNode.querySelector(" .rightFixedP table").style.height = rootNode.querySelector(" .main-table").offsetHeight + "px";
                }
            }
        }

        bs.infoPreview(rootNode.querySelectorAll(" .list-content table tbody td[type='tags']"))
        bs.imagePreview(rootNode.querySelectorAll(" .list-content table tbody td[type='image'] img"))

        if (opts.complateCallBack) {
            opts.complateCallBack(opts.ajax.list, opts.page);
        }
        var clipboard = new Clipboard(rootNode.querySelectorAll(" .list-content a[data-clipboard-text]"));
        clipboard.on('success', function (e) {
            console.log(e);
            bs.success("复制成功！  " + e.text)
            e.clearSelection();
        });

        clipboard.on('error', function (e) {
            bs.error("复制失败！")
        });
    }

    function convertPiexlToNum(el) {
        if (!bs.isEmpty(el.style.width)) {
            return Number(el.style.width.substring(0, el.style.width.length - 2));
        } else {
            return el.offsetWidth;
        }
    }

    function multiSelect() {
        if (opts.multiSelect.isNeedMultiSelect) {
            var className = ' table'
            if (opts.tables.leftFixed > 0) {
                className = ' .leftFixed'
            }
            $bs(rootNode.querySelectorAll(className + ' input[type="checkbox"]')).on('click', function (e, v, i) {
                var self = e.target;
                e.stopPropagation();
                if (e.target.checked) {
                    if (opts.multiSelect.selectMode == "single") {
                        bs.makeArray(rootNode.querySelectorAll(className + ' tbody input[type="checkbox"]')).forEach(function (v, k) {
                            if (e.target.name == v.name) {
                                v.checked = true;
                            } else {
                                v.checked = false;
                            }
                        })
                        opts.multiSelect.selectObjList.length = 0;
                        opts.multiSelect.selectObjList.push(opts.ajax.list[e.target.value]);
                    } else {
                        if (e.target.value == "all") {
                            $bs(rootNode.querySelectorAll(className + ' tbody input[type="checkbox"]')).forEach(function (k, v) {
                                v.checked = true;
                            })
                            opts.multiSelect.selectObjList = bs.unique(opts.multiSelect.selectObjList.concat(opts.ajax.list), opts.key)
                            selectAll();
                        } else {
                            opts.multiSelect.selectObjList.push(opts.ajax.list[e.target.value]);
                            opts.multiSelect.selectObjList = bs.unique(opts.multiSelect.selectObjList, opts.key)
                        }
                    }
                } else {
                    if (e.target.value == "all") {
                        unSelectAll();
                        opts.ajax.list.forEach(function (v, k) {
                            opts.multiSelect.selectObjList.remove(function (o) {
                                return o[opts.key] == v[opts.key];
                            });
                        })
                    } else {
                        opts.multiSelect.selectObjList.remove(function (o) {
                            return o[opts.key] == opts.ajax.list[e.target.value][opts.key];
                        });
                        if (opts.multiSelect.selectMode != "single") {
                            rootNode.querySelector(className + " input[name='all']").checked = false;
                        } else {
                            e.target.checked = false;
                        }

                    }
                }
            })
        }
    }

    function selectAll() {
        bs.makeArray(rootNode.querySelectorAll(' input[type="checkbox"]')).forEach(function (value, key) {
            value.checked = true;
        })
    }

    function unSelectAll() {
        bs.makeArray(rootNode.querySelectorAll(' input[type="checkbox"]')).forEach(function (value, key) {
            value.checked = false;
        })
    }
    this.selectAll = function () {
        bs.makeArray(rootNode.querySelectorAll(' input[type="checkbox"]')).forEach(function (value, key) {
            value.checked = true;
            value.disabled = true;
        })
    }
    this.unSelectAll = function () {
        bs.makeArray(rootNode.querySelectorAll(' input[type="checkbox"]')).forEach(function (value, key) {
            value.checked = false;
            value.disabled = false;
        })
    }
    this.getSelected = function () {
        var arr = [];
        arr.push({
            SelectList: []
        })
        arr[0].SelectList = [].concat(opts.multiSelect.selectObjList);
        return arr;
    }
    this.refresh = function () {
        opts.multiSelect.selectObjList.length = 0;
        if (opts.sort.isNeedSort) {
            TableSort();
        }
        if (opts.multiSelect.isNeedMultiSelect && opts.multiSelect.selectMode != "single") {
            rootNode.querySelector(" input[name='all']").checked = false;
        }
        getData(10);
    }
    this.search = function (query, list) {
        opts.page.pageIndex = 1;
        opts.multiSelect.selectObjList.length = 0;
        if (!bs.isUndefinedOrNull(list)) {
            opts.multiSelect.selectObjList = list;
        }
        if (!bs.isUndefinedOrNull(query)) {
            for (var item in query) {
                opts.ajax.query[item] = query[item]
            }
        }
        if (opts.sort.isNeedSort) {
            TableSort();
        }
        if (opts.multiSelect.isNeedMultiSelect && opts.multiSelect.selectMode != "single") {
            rootNode.querySelector(" input[name='all']").checked = false;
        }
        getData(2);
    }
    this.query = function (query) {
        opts.page.pageIndex = 1;
        if (!bs.isUndefinedOrNull(query)) {
            for (var item in query) {
                opts.ajax.query[item] = query[item]
            }
        }
        if (opts.sort.isNeedSort) {
            TableSort();
        }
        if (opts.multiSelect.isNeedMultiSelect && opts.multiSelect.selectMode != "single") {
            rootNode.querySelector(" input[name='all']").checked = false;
        }
        getData(2);
    }
    this.updateTableHead = function (columns) {
        if (!bs.isUndefinedOrNull(columns)) {
            opts.tables.columns = columns;
        }
        var headDom = rootNode.querySelector(" .list-content .main-table thead");
        headDom.innerHTML = tableHeadRender();
        if (opts.sort.isNeedSort) {
            tableSort();
        }
    }
    this.updateHeadOper = function (heads) {
        if (!bs.isUndefinedOrNull(heads)) {
            opts.tables.heads = heads;
        }
        var headDom = rootNode.querySelector(" .heads .operation");
        headDom.innerHTML = datatableHeadRender();
        $bs(rootNode.querySelectorAll(' .heads .operation a')).on('click', function (e, v, i) {
            if (e.target.getAttribute("disabled") == "true") {
                return;
            }
            opts.tables.heads[e.target.name].callback(e, v);
        })
    }
    this.updateTable = function (tables, query) {
        opts.tables.operations.length = 0;
        opts.tables.columns.length = 0;
        opts.tables.heads.length = 0;
        opts.tables = bs.extend(true, {}, opts.tables, tables);
        this.updateTableHead();
        this.updateHeadOper();
        this.search(query);
    }
    this.getCount = function () {
        return opts.multiSelect.selectObjList.length;
    }
    this.getSelectList = function () {
        return opts.multiSelect.selectObjList;
    }
    this.updateLine = function (data) {
        if (!bs.isUndefinedOrNull(data)) {
            var index = opts.ajax.list.findIndex(function (o) {
                return o[opts.key] == data[opts.key];
            })
            for (var item in data) {
                opts.ajax.list[index][item] = data[item]
            }
            ajaxSuccess();
        }

    }
}
/*!
 * clipboard.js v1.7.1
 * https://zenorocha.github.io/clipboard.js
 *
 * Licensed MIT © Zeno Rocha
 */
(function (f) {
    if (typeof exports === "object" && typeof module !== "undefined") {
        module.exports = f()
    } else if (typeof define === "function" && define.amd) {
        define([], f)
    } else {
        var g;
        if (typeof window !== "undefined") {
            g = window
        } else if (typeof global !== "undefined") {
            g = global
        } else if (typeof self !== "undefined") {
            g = self
        } else {
            g = this
        }
        g.Clipboard = f()
    }
})(function () {
    var define, module, exports;
    return (function e(t, n, r) {
        function s(o, u) {
            if (!n[o]) {
                if (!t[o]) {
                    var a = typeof require == "function" && require;
                    if (!u && a) return a(o, !0);
                    if (i) return i(o, !0);
                    var f = new Error("Cannot find module '" + o + "'");
                    throw f.code = "MODULE_NOT_FOUND", f
                }
                var l = n[o] = {
                    exports: {}
                };
                t[o][0].call(l.exports, function (e) {
                    var n = t[o][1][e];
                    return s(n ? n : e)
                }, l, l.exports, e, t, n, r)
            }
            return n[o].exports
        }
        var i = typeof require == "function" && require;
        for (var o = 0; o < r.length; o++) s(r[o]);
        return s
    })({
        1: [function (require, module, exports) {
            var DOCUMENT_NODE_TYPE = 9;

            /**
             * A polyfill for Element.matches()
             */
            if (typeof Element !== 'undefined' && !Element.prototype.matches) {
                var proto = Element.prototype;

                proto.matches = proto.matchesSelector ||
                    proto.mozMatchesSelector ||
                    proto.msMatchesSelector ||
                    proto.oMatchesSelector ||
                    proto.webkitMatchesSelector;
            }

            /**
             * Finds the closest parent that matches a selector.
             *
             * @param {Element} element
             * @param {String} selector
             * @return {Function}
             */
            function closest(element, selector) {
                while (element && element.nodeType !== DOCUMENT_NODE_TYPE) {
                    if (typeof element.matches === 'function' &&
                        element.matches(selector)) {
                        return element;
                    }
                    element = element.parentNode;
                }
            }

            module.exports = closest;

        }, {}],
        2: [function (require, module, exports) {
            var closest = require('./closest');

            /**
             * Delegates event to a selector.
             *
             * @param {Element} element
             * @param {String} selector
             * @param {String} type
             * @param {Function} callback
             * @param {Boolean} useCapture
             * @return {Object}
             */
            function delegate(element, selector, type, callback, useCapture) {
                var listenerFn = listener.apply(this, arguments);

                element.addEventListener(type, listenerFn, useCapture);

                return {
                    destroy: function () {
                        element.removeEventListener(type, listenerFn, useCapture);
                    }
                }
            }

            /**
             * Finds closest match and invokes callback.
             *
             * @param {Element} element
             * @param {String} selector
             * @param {String} type
             * @param {Function} callback
             * @return {Function}
             */
            function listener(element, selector, type, callback) {
                return function (e) {
                    e.delegateTarget = closest(e.target, selector);

                    if (e.delegateTarget) {
                        callback.call(element, e);
                    }
                }
            }

            module.exports = delegate;

        }, {
            "./closest": 1
        }],
        3: [function (require, module, exports) {
            /**
             * Check if argument is a HTML element.
             *
             * @param {Object} value
             * @return {Boolean}
             */
            exports.node = function (value) {
                return value !== undefined &&
                    value instanceof HTMLElement &&
                    value.nodeType === 1;
            };

            /**
             * Check if argument is a list of HTML elements.
             *
             * @param {Object} value
             * @return {Boolean}
             */
            exports.nodeList = function (value) {
                var type = Object.prototype.toString.call(value);

                return value !== undefined &&
                    (type === '[object NodeList]' || type === '[object HTMLCollection]') &&
                    ('length' in value) &&
                    (value.length === 0 || exports.node(value[0]));
            };

            /**
             * Check if argument is a string.
             *
             * @param {Object} value
             * @return {Boolean}
             */
            exports.string = function (value) {
                return typeof value === 'string' ||
                    value instanceof String;
            };

            /**
             * Check if argument is a function.
             *
             * @param {Object} value
             * @return {Boolean}
             */
            exports.fn = function (value) {
                var type = Object.prototype.toString.call(value);

                return type === '[object Function]';
            };

        }, {}],
        4: [function (require, module, exports) {
            var is = require('./is');
            var delegate = require('delegate');

            /**
             * Validates all params and calls the right
             * listener function based on its target type.
             *
             * @param {String|HTMLElement|HTMLCollection|NodeList} target
             * @param {String} type
             * @param {Function} callback
             * @return {Object}
             */
            function listen(target, type, callback) {
                if (!target && !type && !callback) {
                    throw new Error('Missing required arguments');
                }

                if (!is.string(type)) {
                    throw new TypeError('Second argument must be a String');
                }

                if (!is.fn(callback)) {
                    throw new TypeError('Third argument must be a Function');
                }

                if (is.node(target)) {
                    return listenNode(target, type, callback);
                } else if (Object.prototype.toString.call(target) === '[object NodeList]') {
                    return listenNodeList(target, type, callback);
                } else if (is.string(target)) {
                    return listenSelector(target, type, callback);
                } else {
                    throw new TypeError('First argument must be a String, HTMLElement, HTMLCollection, or NodeList');
                }
            }

            /**
             * Adds an event listener to a HTML element
             * and returns a remove listener function.
             *
             * @param {HTMLElement} node
             * @param {String} type
             * @param {Function} callback
             * @return {Object}
             */
            function listenNode(node, type, callback) {
                node.addEventListener(type, callback);

                return {
                    destroy: function () {
                        node.removeEventListener(type, callback);
                    }
                }
            }

            /**
             * Add an event listener to a list of HTML elements
             * and returns a remove listener function.
             *
             * @param {NodeList|HTMLCollection} nodeList
             * @param {String} type
             * @param {Function} callback
             * @return {Object}
             */
            function listenNodeList(nodeList, type, callback) {
                Array.prototype.forEach.call(nodeList, function (node) {
                    node.addEventListener(type, callback);
                });

                return {
                    destroy: function () {
                        Array.prototype.forEach.call(nodeList, function (node) {
                            node.removeEventListener(type, callback);
                        });
                    }
                }
            }

            /**
             * Add an event listener to a selector
             * and returns a remove listener function.
             *
             * @param {String} selector
             * @param {String} type
             * @param {Function} callback
             * @return {Object}
             */
            function listenSelector(selector, type, callback) {
                return delegate(document.body, selector, type, callback);
            }

            module.exports = listen;

        }, {
            "./is": 3,
            "delegate": 2
        }],
        5: [function (require, module, exports) {
            function select(element) {
                var selectedText;

                if (element.nodeName === 'SELECT') {
                    element.focus();

                    selectedText = element.value;
                } else if (element.nodeName === 'INPUT' || element.nodeName === 'TEXTAREA') {
                    var isReadOnly = element.hasAttribute('readonly');

                    if (!isReadOnly) {
                        element.setAttribute('readonly', '');
                    }

                    element.select();
                    element.setSelectionRange(0, element.value.length);

                    if (!isReadOnly) {
                        element.removeAttribute('readonly');
                    }

                    selectedText = element.value;
                } else {
                    if (element.hasAttribute('contenteditable')) {
                        element.focus();
                    }

                    var selection = window.getSelection();
                    var range = document.createRange();

                    range.selectNodeContents(element);
                    selection.removeAllRanges();
                    selection.addRange(range);

                    selectedText = selection.toString();
                }

                return selectedText;
            }

            module.exports = select;

        }, {}],
        6: [function (require, module, exports) {
            function E() {
                // Keep this empty so it's easier to inherit from
                // (via https://github.com/lipsmack from https://github.com/scottcorgan/tiny-emitter/issues/3)
            }

            E.prototype = {
                on: function (name, callback, ctx) {
                    var e = this.e || (this.e = {});

                    (e[name] || (e[name] = [])).push({
                        fn: callback,
                        ctx: ctx
                    });

                    return this;
                },

                once: function (name, callback, ctx) {
                    var self = this;

                    function listener() {
                        self.off(name, listener);
                        callback.apply(ctx, arguments);
                    };

                    listener._ = callback
                    return this.on(name, listener, ctx);
                },

                emit: function (name) {
                    var data = [].slice.call(arguments, 1);
                    var evtArr = ((this.e || (this.e = {}))[name] || []).slice();
                    var i = 0;
                    var len = evtArr.length;

                    for (i; i < len; i++) {
                        evtArr[i].fn.apply(evtArr[i].ctx, data);
                    }

                    return this;
                },

                off: function (name, callback) {
                    var e = this.e || (this.e = {});
                    var evts = e[name];
                    var liveEvents = [];

                    if (evts && callback) {
                        for (var i = 0, len = evts.length; i < len; i++) {
                            if (evts[i].fn !== callback && evts[i].fn._ !== callback)
                                liveEvents.push(evts[i]);
                        }
                    }

                    // Remove event from queue to prevent memory leak
                    // Suggested by https://github.com/lazd
                    // Ref: https://github.com/scottcorgan/tiny-emitter/commit/c6ebfaa9bc973b33d110a84a307742b7cf94c953#commitcomment-5024910

                    (liveEvents.length) ?
                    e[name] = liveEvents : delete e[name];

                    return this;
                }
            };

            module.exports = E;

        }, {}],
        7: [function (require, module, exports) {
            (function (global, factory) {
                if (typeof define === "function" && define.amd) {
                    define(['module', 'select'], factory);
                } else if (typeof exports !== "undefined") {
                    factory(module, require('select'));
                } else {
                    var mod = {
                        exports: {}
                    };
                    factory(mod, global.select);
                    global.clipboardAction = mod.exports;
                }
            })(this, function (module, _select) {
                'use strict';

                var _select2 = _interopRequireDefault(_select);

                function _interopRequireDefault(obj) {
                    return obj && obj.__esModule ? obj : {
                        default: obj
                    };
                }

                var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) {
                    return typeof obj;
                } : function (obj) {
                    return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj;
                };

                function _classCallCheck(instance, Constructor) {
                    if (!(instance instanceof Constructor)) {
                        throw new TypeError("Cannot call a class as a function");
                    }
                }

                var _createClass = function () {
                    function defineProperties(target, props) {
                        for (var i = 0; i < props.length; i++) {
                            var descriptor = props[i];
                            descriptor.enumerable = descriptor.enumerable || false;
                            descriptor.configurable = true;
                            if ("value" in descriptor) descriptor.writable = true;
                            Object.defineProperty(target, descriptor.key, descriptor);
                        }
                    }

                    return function (Constructor, protoProps, staticProps) {
                        if (protoProps) defineProperties(Constructor.prototype, protoProps);
                        if (staticProps) defineProperties(Constructor, staticProps);
                        return Constructor;
                    };
                }();

                var ClipboardAction = function () {
                    /**
                     * @param {Object} options
                     */
                    function ClipboardAction(options) {
                        _classCallCheck(this, ClipboardAction);

                        this.resolveOptions(options);
                        this.initSelection();
                    }

                    /**
                     * Defines base properties passed from constructor.
                     * @param {Object} options
                     */


                    _createClass(ClipboardAction, [{
                        key: 'resolveOptions',
                        value: function resolveOptions() {
                            var options = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : {};

                            this.action = options.action;
                            this.container = options.container;
                            this.emitter = options.emitter;
                            this.target = options.target;
                            this.text = options.text;
                            this.trigger = options.trigger;

                            this.selectedText = '';
                        }
                    }, {
                        key: 'initSelection',
                        value: function initSelection() {
                            if (this.text) {
                                this.selectFake();
                            } else if (this.target) {
                                this.selectTarget();
                            }
                        }
                    }, {
                        key: 'selectFake',
                        value: function selectFake() {
                            var _this = this;

                            var isRTL = document.documentElement.getAttribute('dir') == 'rtl';

                            this.removeFake();

                            this.fakeHandlerCallback = function () {
                                return _this.removeFake();
                            };
                            this.fakeHandler = this.container.addEventListener('click', this.fakeHandlerCallback) || true;

                            this.fakeElem = document.createElement('textarea');
                            // Prevent zooming on iOS
                            this.fakeElem.style.fontSize = '12pt';
                            // Reset box model
                            this.fakeElem.style.border = '0';
                            this.fakeElem.style.padding = '0';
                            this.fakeElem.style.margin = '0';
                            // Move element out of screen horizontally
                            this.fakeElem.style.position = 'absolute';
                            this.fakeElem.style[isRTL ? 'right' : 'left'] = '-9999px';
                            // Move element to the same position vertically
                            var yPosition = window.pageYOffset || document.documentElement.scrollTop;
                            this.fakeElem.style.top = yPosition + 'px';

                            this.fakeElem.setAttribute('readonly', '');
                            this.fakeElem.value = this.text;

                            this.container.appendChild(this.fakeElem);

                            this.selectedText = (0, _select2.default)(this.fakeElem);
                            this.copyText();
                        }
                    }, {
                        key: 'removeFake',
                        value: function removeFake() {
                            if (this.fakeHandler) {
                                this.container.removeEventListener('click', this.fakeHandlerCallback);
                                this.fakeHandler = null;
                                this.fakeHandlerCallback = null;
                            }

                            if (this.fakeElem) {
                                this.container.removeChild(this.fakeElem);
                                this.fakeElem = null;
                            }
                        }
                    }, {
                        key: 'selectTarget',
                        value: function selectTarget() {
                            this.selectedText = (0, _select2.default)(this.target);
                            this.copyText();
                        }
                    }, {
                        key: 'copyText',
                        value: function copyText() {
                            var succeeded = void 0;

                            try {
                                succeeded = document.execCommand(this.action);
                            } catch (err) {
                                succeeded = false;
                            }

                            this.handleResult(succeeded);
                        }
                    }, {
                        key: 'handleResult',
                        value: function handleResult(succeeded) {
                            this.emitter.emit(succeeded ? 'success' : 'error', {
                                action: this.action,
                                text: this.selectedText,
                                trigger: this.trigger,
                                clearSelection: this.clearSelection.bind(this)
                            });
                        }
                    }, {
                        key: 'clearSelection',
                        value: function clearSelection() {
                            if (this.trigger) {
                                this.trigger.focus();
                            }

                            window.getSelection().removeAllRanges();
                        }
                    }, {
                        key: 'destroy',
                        value: function destroy() {
                            this.removeFake();
                        }
                    }, {
                        key: 'action',
                        set: function set() {
                            var action = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : 'copy';

                            this._action = action;

                            if (this._action !== 'copy' && this._action !== 'cut') {
                                throw new Error('Invalid "action" value, use either "copy" or "cut"');
                            }
                        },
                        get: function get() {
                            return this._action;
                        }
                    }, {
                        key: 'target',
                        set: function set(target) {
                            if (target !== undefined) {
                                if (target && (typeof target === 'undefined' ? 'undefined' : _typeof(target)) === 'object' && target.nodeType === 1) {
                                    if (this.action === 'copy' && target.hasAttribute('disabled')) {
                                        throw new Error('Invalid "target" attribute. Please use "readonly" instead of "disabled" attribute');
                                    }

                                    if (this.action === 'cut' && (target.hasAttribute('readonly') || target.hasAttribute('disabled'))) {
                                        throw new Error('Invalid "target" attribute. You can\'t cut text from elements with "readonly" or "disabled" attributes');
                                    }

                                    this._target = target;
                                } else {
                                    throw new Error('Invalid "target" value, use a valid Element');
                                }
                            }
                        },
                        get: function get() {
                            return this._target;
                        }
                    }]);

                    return ClipboardAction;
                }();

                module.exports = ClipboardAction;
            });

        }, {
            "select": 5
        }],
        8: [function (require, module, exports) {
            (function (global, factory) {
                if (typeof define === "function" && define.amd) {
                    define(['module', './clipboard-action', 'tiny-emitter', 'good-listener'], factory);
                } else if (typeof exports !== "undefined") {
                    factory(module, require('./clipboard-action'), require('tiny-emitter'), require('good-listener'));
                } else {
                    var mod = {
                        exports: {}
                    };
                    factory(mod, global.clipboardAction, global.tinyEmitter, global.goodListener);
                    global.clipboard = mod.exports;
                }
            })(this, function (module, _clipboardAction, _tinyEmitter, _goodListener) {
                'use strict';

                var _clipboardAction2 = _interopRequireDefault(_clipboardAction);

                var _tinyEmitter2 = _interopRequireDefault(_tinyEmitter);

                var _goodListener2 = _interopRequireDefault(_goodListener);

                function _interopRequireDefault(obj) {
                    return obj && obj.__esModule ? obj : {
                        default: obj
                    };
                }

                var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) {
                    return typeof obj;
                } : function (obj) {
                    return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj;
                };

                function _classCallCheck(instance, Constructor) {
                    if (!(instance instanceof Constructor)) {
                        throw new TypeError("Cannot call a class as a function");
                    }
                }

                var _createClass = function () {
                    function defineProperties(target, props) {
                        for (var i = 0; i < props.length; i++) {
                            var descriptor = props[i];
                            descriptor.enumerable = descriptor.enumerable || false;
                            descriptor.configurable = true;
                            if ("value" in descriptor) descriptor.writable = true;
                            Object.defineProperty(target, descriptor.key, descriptor);
                        }
                    }

                    return function (Constructor, protoProps, staticProps) {
                        if (protoProps) defineProperties(Constructor.prototype, protoProps);
                        if (staticProps) defineProperties(Constructor, staticProps);
                        return Constructor;
                    };
                }();

                function _possibleConstructorReturn(self, call) {
                    if (!self) {
                        throw new ReferenceError("this hasn't been initialised - super() hasn't been called");
                    }

                    return call && (typeof call === "object" || typeof call === "function") ? call : self;
                }

                function _inherits(subClass, superClass) {
                    if (typeof superClass !== "function" && superClass !== null) {
                        throw new TypeError("Super expression must either be null or a function, not " + typeof superClass);
                    }

                    subClass.prototype = Object.create(superClass && superClass.prototype, {
                        constructor: {
                            value: subClass,
                            enumerable: false,
                            writable: true,
                            configurable: true
                        }
                    });
                    if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass;
                }

                var Clipboard = function (_Emitter) {
                    _inherits(Clipboard, _Emitter);

                    /**
                     * @param {String|HTMLElement|HTMLCollection|NodeList} trigger
                     * @param {Object} options
                     */
                    function Clipboard(trigger, options) {
                        _classCallCheck(this, Clipboard);

                        var _this = _possibleConstructorReturn(this, (Clipboard.__proto__ || Object.getPrototypeOf(Clipboard)).call(this));

                        _this.resolveOptions(options);
                        _this.listenClick(trigger);
                        return _this;
                    }

                    /**
                     * Defines if attributes would be resolved using internal setter functions
                     * or custom functions that were passed in the constructor.
                     * @param {Object} options
                     */


                    _createClass(Clipboard, [{
                        key: 'resolveOptions',
                        value: function resolveOptions() {
                            var options = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : {};

                            this.action = typeof options.action === 'function' ? options.action : this.defaultAction;
                            this.target = typeof options.target === 'function' ? options.target : this.defaultTarget;
                            this.text = typeof options.text === 'function' ? options.text : this.defaultText;
                            this.container = _typeof(options.container) === 'object' ? options.container : document.body;
                        }
                    }, {
                        key: 'listenClick',
                        value: function listenClick(trigger) {
                            var _this2 = this;

                            this.listener = (0, _goodListener2.default)(trigger, 'click', function (e) {
                                return _this2.onClick(e);
                            });
                        }
                    }, {
                        key: 'onClick',
                        value: function onClick(e) {
                            var trigger = e.delegateTarget || e.currentTarget;

                            if (this.clipboardAction) {
                                this.clipboardAction = null;
                            }

                            this.clipboardAction = new _clipboardAction2.default({
                                action: this.action(trigger),
                                target: this.target(trigger),
                                text: this.text(trigger),
                                container: this.container,
                                trigger: trigger,
                                emitter: this
                            });
                        }
                    }, {
                        key: 'defaultAction',
                        value: function defaultAction(trigger) {
                            return getAttributeValue('action', trigger);
                        }
                    }, {
                        key: 'defaultTarget',
                        value: function defaultTarget(trigger) {
                            var selector = getAttributeValue('target', trigger);

                            if (selector) {
                                return document.querySelector(selector);
                            }
                        }
                    }, {
                        key: 'defaultText',
                        value: function defaultText(trigger) {
                            return getAttributeValue('text', trigger);
                        }
                    }, {
                        key: 'destroy',
                        value: function destroy() {
                            this.listener.destroy();

                            if (this.clipboardAction) {
                                this.clipboardAction.destroy();
                                this.clipboardAction = null;
                            }
                        }
                    }], [{
                        key: 'isSupported',
                        value: function isSupported() {
                            var action = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : ['copy', 'cut'];

                            var actions = typeof action === 'string' ? [action] : action;
                            var support = !!document.queryCommandSupported;

                            actions.forEach(function (action) {
                                support = support && !!document.queryCommandSupported(action);
                            });

                            return support;
                        }
                    }]);

                    return Clipboard;
                }(_tinyEmitter2.default);

                /**
                 * Helper function to retrieve attribute value.
                 * @param {String} suffix
                 * @param {Element} element
                 */
                function getAttributeValue(suffix, element) {
                    var attribute = 'data-clipboard-' + suffix;

                    if (!element.hasAttribute(attribute)) {
                        return;
                    }

                    return element.getAttribute(attribute);
                }

                module.exports = Clipboard;
            });

        }, {
            "./clipboard-action": 7,
            "good-listener": 4,
            "tiny-emitter": 6
        }]
    }, {}, [8])(8)
});