function treeTable(option) {
    var defaults = {
        el: '',
        tree: {
            key: '',
            text: '',
            label: '',
            parentId: 'ParentId',
            currentId: '',
            isNeedSerialize: true,
            ajax: {},
            list: []
        },
        table: {
            isNeedSerialize: true,
            list: [],
            ajax: {},
            columns: [],
            operations: [],
            heads: []
        },
        page: {
            isNeedPage: false,
            totalPages: 0,
            pageIndex: 1,
            pageSize: 15,
            dataCount: 0
        },
        config: {
            widthRo: 1,
            isMask: true,
            treeWidth: 30,
            operWidth: 30,
            isEnableCheckbox: false
        }
    }
    var opts = bs.cloneDeep(true, {}, defaults, option);
    opts.config.widthRo = opts.config.widthRo * 100;
    var root = document.createElement("div");

    function init() {
        root.className = "tree-table";
        var contentNode = document.createElement("div");
        contentNode.className = 'tree-table-content';
        if (opts.config.isMask) {
            contentNode.appendChild(renderMask());
        }
        contentNode.appendChild(renderHeader());
        var treeNode = document.createElement("div");
        treeNode.className = 'bs-tree';
        treeNode.style.width = opts.config.widthRo + "%";
        contentNode.appendChild(treeNode);
        root.appendChild(contentNode);
        document.querySelector(opts.el).appendChild(root);
        getData();
    }

    function renderHeader() {
        var headerNode = document.createElement("div");
        headerNode.className = "tree-header";
        var data = {
            columns: opts.table.columns,
            heads: opts.table.heads,
            lineWidth: (opts.config.widthRo - opts.config.treeWidth - opts.config.operWidth) / opts.table.columns.length,
            treeWidth: opts.config.treeWidth,
            operWidth: opts.config.operWidth,
            treeName: opts.tree.label,
            width: opts.config.widthRo
        }
        var templateHtml = '<div class="header-opers">{{# d.heads.forEach(function(v,k){ }}<a class="bs-btn {{v.disabled ?"disabled":"default"}}" data-SerialNo={{k}} title={{v.label}}>{{v.label}}</a>{{# })}}</div>\
        <div class="tree-columns flex" style="width:{{d.width}}%"><div style="width:{{d.treeWidth}}%">{{d.treeName}}</div>{{# d.columns.forEach(function(v,k){ }}\
        <div style="width:{{d.lineWidth}}%">{{v.label}}</div>{{# })}}<div style="width:{{d.operWidth}}%">操作</div></div>';
        bs.render(templateHtml, data, function (html) {
            headerNode.innerHTML = html;
        });
        return headerNode;
    }

    function getData() {
        var ajax = bs.cloneDeep({}, opts.table.ajax);
        if (opts.page.isNeedPage) {
            ajax.data.pageIndex = opts.page.pageIndex;
            ajax.data.pageSize = opts.page.pageSize;
        }
        ajax.data = opts.table.isNeedSerialize ? JSON.stringify(ajax.data) : ajax.data;
        $.when($.ajax(ajax)).then(function (result) {
            result = JSON.parse(result.d);
            if (result.IsSuccess) {
                root.querySelector(".bs-tree").innerHTML = "";
                var data = opts.table.ajax.callback(result);
                if (data.list.length == 0) {
                    var noDataNode = document.createElement("div");
                    noDataNode.className = "nodata";
                    noDataNode.innerHTML = "暂无数据！";
                    root.querySelector(".bs-tree").appendChild(noDataNode);
                } else {
                    opts.table.list = data.list;
                    render(0);
                }
                if (opts.page.isNeedPage) {
                    opts.page.totalPages = data.totalPages;
                    opts.page.dataCount = data.dataCount;
                    $bs(root.querySelector(".tree-table-content")).pager({
                        pageIndex: opts.page.pageIndex,
                        pageSize: opts.page.pageSize,
                        dataCount: opts.page.dataCount,
                        totalPages: opts.page.totalPages,
                        isNeedSelect: true,
                        callback: function (pageIndex, pageSize) {
                            opts.page.pageSize = pageSize;
                            opts.page.pageIndex = pageIndex;
                            opts.tree.currentId = "";
                            openMack();
                            getData(1);
                        }
                    });
                }
                closeMack();

            } else {
                bs.error("请求失败");
                return false;
            }
        }, function (err) {
            error();
        })
    }

    function getChildData(level) {
        openMack();
        opts.tree.ajax.data = opts.tree.isNeedSerialize ? JSON.stringify({
            parentId: opts.tree.currentId
        }) : {
            parentId: opts.tree.currentId
        };
        $.when($.ajax(opts.tree.ajax)).then(function (result) {
            result = JSON.parse(result.d);
            if (result.IsSuccess) {
                opts.tree.list = opts.tree.ajax.callback(result);
                render(level + 1);
                closeMack();
            } else {
                bs.error("请求失败");
                return false;
            }
        }, function (err) {
            error();
        })
    }

    function render(level) {
        var data = {
            list: bs.isEmpty(opts.tree.currentId) ? opts.table.list : opts.tree.list,
            columns: opts.table.columns,
            opers: opts.table.operations,
            tree: {
                key: opts.tree.key,
                text: opts.tree.text,
                parentId: opts.tree.parentId,
            },
            level: level,
            width: (opts.config.widthRo - opts.config.treeWidth - opts.config.operWidth) / opts.table.columns.length,
            treeWidth: opts.config.treeWidth,
            operWidth: opts.config.operWidth
        }
        bs.render(bs.getTemplate(tableViewTpl), data, function (html) {
            var div = document.createElement("div");
            div.innerHTML = html;
            if (bs.isEmpty(opts.tree.currentId)) {
                root.querySelector(".bs-tree").appendChild(div);
            } else {
                div.setAttribute("data-child", true);
                root.querySelector(".bs-tree [data-nodeId='el" + opts.tree.currentId + "']").appendChild(div);
            }
        }, true);
        registEvent();
    }

    function renderMask() {
        var modalDom = document.createElement("div");
        modalDom.className = 'bs-loading-mask fade in';
        modalDom.innerHTML = '<div class="bs-loading-spinner">\
        <svg viewBox="25 25 50 50" class="circular"><circle cx="50" cy="50" r="20" fill="none" class="path"></circle></svg> <p class="bs-loading-text">正在加载中</p></div>';
        return modalDom;
    }

    function closeMack() {
        if (opts.config.isMask) {
            $bs(root.querySelector(".bs-loading-mask")).removeClass("in");
            bs.delay(function () {
                $bs(root.querySelector(".bs-loading-mask")).addClass("none");
            }, 300)
        }
    }

    function openMack() {
        if (opts.config.isMask) {
            $bs(root.querySelector(".bs-loading-mask")).removeClass("none");
            $bs(root.querySelector(".bs-loading-mask")).addClass("in");
        }
    }

    function registEvent() {
        $bs(root.querySelectorAll(".tree-table .bs-tree-arrow:not(.nochild)")).on("click", function () {
            opts.tree.currentId = this.parentNode.parentNode.getAttribute("data-currentId");
            if ($(this).hasClass("open")) {
                $bs(root).findAll(".bs-tree li[data-nodeId='el" + opts.tree.currentId + "']>div[data-child='true']").remove();
                $(this).removeClass("open");
            } else {
                $(this).addClass("open");
                getChildData(Number(this.getAttribute("data-level")));
            }
        });
        $bs(root.querySelectorAll(".tree-table .bs-tree .bs-tree-children .bs-link")).on("click", function () {
            var serialno = Number(this.getAttribute("data-serialno"));
            if (opts.table.operations[serialno].callback) {
                opts.table.operations[serialno].callback.call(this, this.parentNode.parentNode.getAttribute("data-currentId"));
            }
        });
        $bs(root.querySelectorAll(".tree-table .tree-header .header-opers .bs-btn")).on("click", function () {
            if (!$bs(this).hasClass("disabled")) {
                var serialno = Number(this.getAttribute("data-serialno"));
                if (opts.table.operations[serialno].callback) {
                    opts.table.heads[serialno].callback.call(this);
                }
            }
        });
    }
    init();
    this.refresh = function () {
        opts.tree.currentId = "";
        getData();
    }
    this.search = function (query) {
        opts.page.pageIndex = 1;
        if (!bs.isUndefinedOrNull(query)) {
            for (var item in query) {
                opts.table.ajax.data[item] = query[item]
            }
        }
        this.refresh();
    }
    this.disableHeaderOper = function (obj) {
        $bs(obj).removeClass("default").addClass("disabled");
    }
    var tableViewTpl = function () {
        /* {{# d.list.forEach(function(item,key){ }}
        <ul class="bs-tree-children">
            <li data-nodeId=el{{item[d.tree.key]}} data-parentId={{item[d.tree.parentId]}}>
                <div class="tree-tr hover flex" data-currentId={{item[d.tree.key]}}>
                    <div style="width:{{d.treeWidth}}%;padding-left:{{d.level*26}}px" class="tree-content">
                        <span class="bs-tree-arrow {{item.IsChild?'':'nochild'}}" data-level={{d.level}}></span>
                        <span class="bs-tree-title">{{item[d.tree.text]}}</span>
                    </div>
                    {{# d.columns.forEach(function(v,k){ }}
                    <div class="tree-td" style="width:{{d.width}}%">
                        {{item[v.value]}}
                    </div>
                    {{# })}}
                    <div class="tree-td" style="width:{{d.operWidth}}%">
                        {{# d.opers.forEach(function(v,k){ }}
                        <a class="bs-link" data-SerialNo={{k}}>{{v.label}}</a>
                        {{# })}}
                    </div>
                </div>
            </li>
        </ul>
        {{# })}} */
    }

    function error() {
        bs.error("请求异常");
        return false;
    }
}