$(function(){
    // 新增
    //$(".addbtn").click(function(){
    //     showPropertyM();
    //});
    //对比明细
    //$(".detaildiff").click(function(){
    //    showdetailDiff();
    //});
    // 关闭  取消
    //$(".closemark,.doubtncancel").click(function(){
    //    $(".doubleselect").addClass("none")
    //});
    //显示双列内容
    //$(".widthB").click(function(){
    //    $(".doubleselect").removeClass("none");
    //});
    //全选
    //$(document).on('click','.checkall span',function(){
    //    if($(this).hasClass("check")){
    //        $(this).removeClass("check"); 
    //       $(this).parents('.list').find(".douflmain span").removeClass("check"); 
    //    }else{
    //        $(this).addClass("check"); 
    //        $(this).parents('.list').find(".douflmain span").addClass("check"); 
    //    }
    //})
    ////判断是否全选
    //$(document).on('click','.douflmain span',function(){
    //    var listL = $(this).parents(".douflmain").find(".doufllist").length
    //    if($(this).hasClass("check")){
    //        $(this).removeClass("check"); 
    //    }else{
    //       $(this).addClass("check"); 
    //    }
    //    var checkL = $(this).parents(".douflmain").find(".doufllist span.check").length
    //    if(listL == checkL){
    //        $(this).parents(".list").find(".douflall span").addClass("check");
    //    }else{
    //        $(this).parents(".list").find(".douflall span").removeClass("check");
    //    }
    //})
    //选中左到右
    //$(document).on('click','.doufmone',function(){
    //    var input = $(".douconfl .douflmain").find("span.check")
    //    if(input.length == 0){
    //        alert("请选择所要移动的行")
    //        return
    //    }else{
    //        for(var i=0,len=input.length;i<=len;i++){
    //            if(($(".douconfl .doufllist").eq(i).find("span").hasClass("check"))){
    //                var html = $(".douconfl .doufllist").eq(i).html()
    //                $(".douconfl .doufllist").eq(i).remove()
    //                if($(".douconfr .douflmain").length){
    //                   $(".douconfr .douflmain").append('<div class="doufllist">' + html + '</div>') 
    //                }else{
    //                    $(".douconfr").append('<div class="douflmain"><div class="doufllist">' + html + '</div></div>')
    //                }
    //                i--
    //            }
    //        }
    //    }
    //    var listL = $(".douconfr").find(".doufllist").length;
    //    var checkL = $(".douconfr").find(".doufllist span.check").length;

    //    if(listL == checkL){
    //        $(".douconfr").find(".douflall span").addClass("check");
    //    }else{
    //        $(".douconfr").find(".douflall span").removeClass("check");
    //    }
    //    $(".douconfl").find(".douflall span").removeClass("check");
    //})
    ////全部从左到右
    //$(document).on('click','.doufmtwo',function(){
    //    var html = $(".douconfl .douflmain").html();
    //    $(".douconfl .douflmain").remove()
    //    if($(".douconfr .douflmain").length){
    //       $(".douconfr .douflmain").append(html) 
    //    }else{
    //        $(".douconfr").append('<div class="douflmain">' + html + '</div>')
    //    }
        
    //    var listL = $(".douconfr").find(".doufllist").length;
    //    var checkL = $(".douconfr").find(".doufllist span.check").length;

    //    if(listL == checkL){
    //        $(".douconfr").find(".douflall span").addClass("check");
    //    }else{
    //        $(".douconfr").find(".douflall span").removeClass("check");
    //    }
    //    $(".douconfl").find(".douflall span").removeClass("check");
    //})
    //选中右到左
    //$(document).on('click','.doufmthree',function(){
    //    var input = $(".douconfr .douflmain").find("span.check")
    //    if(input.length == 0){
    //        alert("请选择所要移动的行")
    //        return
    //    }else{
    //        for(var i=0,len=input.length;i<=len;i++){
    //            if($(".douconfr .doufllist").eq(i).find("span").hasClass("check")){
    //                var html = $(".douconfr .doufllist").eq(i).html()
    //                if($(".douconfl .douflmain").length){
    //                   $(".douconfl .douflmain").append('<div class="doufllist">' + html + '</div>') 
    //                }else{
    //                    $(".douconfl").append('<div class="douflmain"><div class="doufllist">' + html + '</div></div>')
    //                }
    //                $(".douconfr .doufllist").eq(i).remove()
    //                i--
    //            }
    //        }
    //    }
    //    var listL = $(".douconfl").find(".doufllist").length;
    //    var checkL = $(".douconfl").find(".doufllist span.check").length;

    //    if(listL == checkL){
    //        $(".douconfl").find(".douflall span").addClass("check");
    //    }else{
    //        $(".douconfl").find(".douflall span").removeClass("check");
    //    }
    //    $(".douconfr").find(".douflall span").removeClass("check");
    //})
    ////全部从右到左
    //$(document).on('click','.doufmfour',function(){
    //    var html = $(".douconfr .douflmain").html();
    //    $(".douconfr .douflmain").remove()
    //    if($(".douconfl .douflmain").length){
    //       $(".douconfl .douflmain").append(html) 
    //    }else{
    //        $(".douconfl").append('<div class="douflmain">' + html + '</div>')
    //    }

    //    var listL = $(".douconfl .douflmain").find(".doufllist").length
    //    var checkL = $(".douconfl .douflmain").find(".doufllist span.check").length
    //    if(listL == checkL){
    //        $(".douconfl").find(".douflall span").addClass("check");
    //    }else{
    //        $(".douconfl").find(".douflall span").removeClass("check");
    //    }
    //    $(".douconfr").find(".douflall span").removeClass("check");
    //});


     var htmlproperty = '<li class="propertyItem">'+
                        '<p> <span class="addSpan">添加属性</span> </p>'+
                        '<p> <span class="addSpan">添加属性</span> </p>'+
                        '<p> <span class="deleteFbtn">删除</span> </p>'+
                    '</li>'
    var htmldetail = '<li class="propertyItem">'+
                            ' <p> <span class="addSpan">添加代码</span> </p>'+
                            '<p> <span class="addSpan">添加名称</span> </p>'+
                            '<p class="widthB"> <span>添加代码</span> </p>'+
                            '<p> <span>添加名称</span> </p>'+
                            '<p> <span class="deleteFbtn">删除</span> </p>'+
                        '</li>'

    // 属性新增------------------------
    //$('.propertyDiffModule .fakerBtn').click(function(){
    //    $('.propertyDiffModule .listProperty').append(htmlproperty);
    //})
    //$(' .propertyDiffModule .listProperty').on('click','.deleteFbtn',function(){
    //    $(this).parents('li.propertyItem').remove();
    //})
    //$('.commonBox .middleContent').on('click','span.addSpan',function(){
    //    showSingleColumnM();
    //})
    // 关闭属性弹窗
    //$('.propertyDiffModule .closeHeavy').click(function(){
    //    $(this).parents('.bgCShade').addClass('none');
    //})
    ////显示属性弹窗
    //function showPropertyM(){
    //    $('.propertyDiffModule').parent('.bgCShade').removeClass('none');
    //}


    //明细新增-----------------------------------------------------------------------------------
    //$('.detailDiffModule .fakerBtn').click(function(){
    //    $('.detailDiffModule .listProperty').append(htmldetail);
    //})
    //$(' .detailDiffModule .listProperty').on('click','.deleteFbtn',function(){
    //    $(this).parents('li.propertyItem').remove();
    //})
 

    // 三级弹窗 关闭
    function closeThreeM(){
        $('.difMode').addClass('none');
    }
    $('.difMode .closeZ').click(function(){
        closeThreeM();
    })
    //按照代码
    $('.difMode .bottomBtn .accordingName').click(function(){
        closeThreeM();
    })
    //按照名称
    $('.difMode .bottomBtn .accordingCode').click(function(){
        closeThreeM();
    })
    // 显示明细对比
    function showdetailDiff(){
        $('.detailDiffModule').parent('.bgCShade').removeClass('none');
    }
    // 关闭明细对比
    //function closedetailDiff(){
    //    $('.detailDiffModule').parent('.bgCShade').addClass('none');
    //}
    //$('.detailDiffModule img.closeHeavy').click(function(){
    //    closedetailDiff();
    //})
    // 一键对比
    $('.middleContent .onekeyDiff').click(function(){
        $(".difMode").removeClass("none");
    })
    //确认
    //$('.middleContent .confirm').click(function(){
    //    closedetailDiff();
    //})

    // 三级单列 选择代码名称弹框-----------------------------------------------------
    //$('.singleColumnM .closeZ').click(function(){
    //    $('.bgCShade .singleColumnM').parent('.bgCShade').addClass('none');
    //})
    //选择
    //$('.selectListArea .selectList').on('click','span.operationBtn',function(){
    //    $('.singleColumnM .closeZ').trigger('click');
    //})
    //显示选择代码名称弹框
    //function showSingleColumnM(){
    //    $('.singleColumnM').parent('.bgCShade').removeClass('none');
    //}
    // 闭合>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
})