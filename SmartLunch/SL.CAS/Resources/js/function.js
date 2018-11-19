function isJsonString(str) {
    try {
        if (typeof JSON.parse(str) == "object") {
            return true;
        }
    } catch (e) {
        console.log("function.js 这不是一个json数据");
    }
    return false;
}
//获取url参数方法
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}
/*
{
        name:"商品浏览",
        url: "/SmartEye/SmartEyeGoodsShow",
    }
*/
function switchTab(obj) {
    debugger
    //obj = {
    //    name:"商品浏览",
    //    url: "/SmartEye/SmartEyeGoodsShow",
    //}
    var doc = $("#page-wrapper", window.parent.document);
    var len = doc.find(".page-tabs-content a").length;

    //是否存在
    var url = obj.url;
    var index = -1;
    $.each(doc.find(".page-tabs-content a"), function (i, item) {
        if (item.dataset.id == url) {
            //存在  
            index = i;
        }
    });
    if (index == -1) {
        //不存在
        $.each(doc.find(".page-tabs-content a"), function (i, item) {
            $(item).removeClass("active");
        });
        doc.find(".page-tabs-content").append("<a href=\"javascript:;\" class=\"J_menuTab active\" data-id=\"" + url + "\">" + obj.name + " <i class=\"fa fa-times-circle\"></i></a>");

        $("#content-main", window.parent.document).find(".J_iframe").each(function () {
            $(this).hide();
        });

        $("#content-main", window.parent.document).append("<iframe class=\"J_iframe\" name=\"iframe" + (len + 1) + "\" width=\"100%\" height=\"100%\" src=\"" + url + "\" frameborder=\"0\" data-id=\"" + url + "\" seamless=\"\"></iframe>");

    } else {
        //存在

        //先去掉 active
        //再加上 active
        $.each(doc.find(".page-tabs-content a"), function (i, item) {
            if ($(item).attr("data-id") != url) {
                $(item).removeClass("active");
            } else {
                $(item).addClass("active");
            }
        });
        $("#content-main", window.parent.document).find(".J_iframe").each(function () {
            if ($(this).index()+1 == index) {
                $(this).show();
                //

                $(this).attr("src", obj.url);
                
            } else {
                $(this).hide();
            }
        });
    }
}