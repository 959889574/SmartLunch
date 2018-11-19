

$(function () {
    //公用变量
    var ActiveType = {};//当前选中的索引的类型
    ActiveType.indexseType = 0;
    ActiveType.nameType = '';
    $('.secondImgBox .againUpimg').change(function () {
        var imgfile = this.files;
        for (var i = 0, len = imgfile.length; i < len; i++) {
            var imageType = /^image\//;
            if (!imageType.test(imgfile[i].type)) {
                console.log('文件：' + imgfile[i].name + '不是图片，请选择图片！');
                continue;
            } else {

                var img = $('#secondImgBox div img')[0];

                reader = new FileReader();
                reader.onload = (function (img) {
                    return function (e) {
                        var kbs = e.total / 1024;
                        if (kbs > 1024) {
                            /// 图片大于1M，需要压缩
                            var quality = 1024 / kbs;
                            var img1 = new Image();
                            img1.src = window.URL.createObjectURL(imgfile[0]);
                            img1.onload = function () {
                                // IOS 设备中，如果的照片是竖屏拍摄的，虽然实际在网页中显示出的方向也是垂直，但图片数据依然是以横屏方向展示
                                var sourceWidth = this.naturalWidth; // 在没有加入文档前，jQuery无法获得正确宽高，但可以通过原生属性来读取
                                var realityHeight = this.naturalHeight;
                                var angleOffset = 0;
                                if (sourceWidth == realityHeight) {
                                    angleOffset = 90;
                                }
                                // 将图片进行压缩
                                var newDataURL = compressImg(
                                  this,
                                  quality,
                                  angleOffset,
                                  imgfile[0].type
                                );

                                $(img).attr("mark", newDataURL);
                            };
                        } else {
                            $(img).attr("mark", e.target.result);
                        }


                        img.src = e.target.result;                        
                        $(img).show();
                    }
                })(img);
                reader.readAsDataURL(imgfile[i]);
            }
        }
    });
    function compressImg(sourceImgObj, quality, angleOffset, outputFormat) {

        quality = quality || .8;
        angleOffset = angleOffset || 0;
        var mimeType = outputFormat || "image/jpeg";

        var drawWidth = sourceImgObj.naturalWidth,
            drawHeight = sourceImgObj.naturalHeight;
        // IOS 设备上 canvas 宽或高如果大于 1024，就有可能导致应用崩溃闪退
        // 因此这里需要缩放
        var maxSide = Math.max(drawWidth, drawHeight);
        if (maxSide > 1024) {
            var minSide = Math.min(drawWidth, drawHeight);
            minSide = minSide / maxSide * 1024;
            maxSide = 1024;
            if (drawWidth > drawHeight) {
                drawWidth = maxSide;
                drawHeight = minSide;
            } else {
                drawWidth = minSide;
                drawHeight = maxSide;
            }
        }
        var cvs = document.createElement('canvas');
        var ctx = cvs.getContext("2d");
        if (angleOffset) {
            cvs.width = drawHeight;
            cvs.height = drawWidth;
            ctx.translate(drawHeight, 0);
            ctx.rotate(angleOffset * Math.PI / 180);
        } else {
            cvs.width = drawWidth;
            cvs.height = drawHeight;
        }
        ctx.drawImage(sourceImgObj, 0, 0, drawWidth, drawHeight);
        var newImageData = cvs.toDataURL(mimeType, quality || .8);
        return newImageData;
    }
    $('div.bottomAreaCON').on('change', '#inputUpdata', function (e) {

        var imgfile = this.files;
        for (var i = 0, len = imgfile.length; i < len; i++) {
            var imageType = /^image\//;
            if (!imageType.test(imgfile[i].type)) {
                console.log('文件：' + imgfile[i].name + '不是图片，请选择图片！');
                continue;
            } else {
                    var img = $('#imgBox > img')[0];
                    reader = new FileReader();
                    reader.onload = (function (img) {
                        return function (e) {
                           
                            var kbs = e.total / 1024;
                            if (kbs > 1024) {
                                /// 图片大于1M，需要压缩
                                var quality = 1024 / kbs;
                                var img1 = new Image();
                                img1.src = window.URL.createObjectURL(imgfile[0]);
                                img1.onload = function () {
                                    // IOS 设备中，如果的照片是竖屏拍摄的，虽然实际在网页中显示出的方向也是垂直，但图片数据依然是以横屏方向展示
                                    var sourceWidth = this.naturalWidth; // 在没有加入文档前，jQuery无法获得正确宽高，但可以通过原生属性来读取
                                    var realityHeight = this.naturalHeight;
                                    var angleOffset = 0;
                                    if (sourceWidth == realityHeight) {
                                        angleOffset = 90;
                                    }
                                    // 将图片进行压缩
                                    var newDataURL = compressImg(
                                      this,
                                      quality,
                                      angleOffset,
                                      imgfile[0].type
                                    );
                                    $("#inputUpdata").attr("mark", newDataURL);
                                };
                            } else {
                                $("#inputUpdata").attr("mark", e.target.result);
                            }

                            var li = '<li class="showImgItem"><img src=' + e.target.result + ' alt=""><div class="iconClose"><img src="' + CloseImgIcon_03 + '" alt=""></div></li>';
                            $('#showImgArea').append(li);
                        }
                    })(img);
                    reader.readAsDataURL(imgfile[i]);
                }
            }
    });

    $('#uploadImgModuleContent #showImgArea').on('mouseover', 'li.showImgItem', function () {
        $(this).children('.iconClose').addClass('show');
    })
    $('#uploadImgModuleContent #showImgArea').on('mouseout', 'li.showImgItem', function () {
        $(this).children('.iconClose').removeClass('show');
    })

    $('#uploadImgModuleContent #showImgArea').on('click', '.showImgItem .iconClose', function () {
        $(this).parent('li.showImgItem').remove();
        $("#inputUpdata").attr("mark", "");

    });

    // 确认识别
    $('.distinguishBtn').click(function () {

    })
    //点击选择图片
    $('#UpLoadImg').click(function () {
        if ($('#showImgArea li.showImgItem').length >= 1) {
            return
        }
        $('#inputUpdata').remove();
        var input = '<input id="inputUpdata"  type="file" accept="image/jpeg,image/jpg,image/png"  hidden>'
        $('.bottomAreaCON').append(input);
        $('#inputUpdata').click();
    })
    //
    $('#UpLoadImg').mousedown(function () {
        if ($('#showImgArea li.showImgItem').length >= 1) {
            $('#uploadImgModuleContent h1.MTitle').addClass('active')
        }

    }).mouseup(function () {
        $('#uploadImgModuleContent h1.MTitle').removeClass('active')
    })
    // $('#UpLoadImg').
    $('#uploadImgModuleContent .closeUploadImgModule').click(function () {
        $('#uploadImgModule').addClass('hide');
    })

    //$('.bottomBtnArea .ButtonUp').click(function () {
    //    $('#uploadImgModule').removeClass('hide');
    //})

    // 公用 关闭类型模态框
    function closeTypeM() {
        $('.selectCategoryM').addClass('hide');
    }
    function openTypeM() {
        $('.selectCategoryM').removeClass('hide');
    }
    //关闭
    $('.selectCategoryM .header .iconfont.icon-guanbi').click(function () {
        closeTypeM();
    })
    // 取消
    $('.bottomBtnA .centerBox .cancelB').click(function () {
        closeTypeM();
    })
    //确认
    $('.bottomBtnA .centerBox .trueB').click(function () {
        var array = $('.typeLis .typeItem');
        for (var index = 0; index < array.length; index++) {
            var element = array[index];
            if ($(element).hasClass('active')) {
                ActiveType.indexseType = index;
                ActiveType.nameType = $(element).children('.typeName')[0].textContent;
                $('.selectType .typeTxt').text(ActiveType.nameType);
                break;
            }
        }
        closeTypeM();
    })


    //选择类别弹窗
    $('.selectType .typeTxt').click(function () {
        openTypeM();
    })
    $('.selectType .icon-bianji').click(function () {
        openTypeM();
    })
    //选择类别
    //$('.typeLis').on('click','.typeItemOpen',function(){
    //    $(this).siblings('.typeItem').removeClass('active');
    //    $(this).addClass('active');
    //})

    $('.pictureExampleModule .picexample > .icon-guanbi').click(function () {
        $('.pictureExampleModule').addClass('hide');
    })
    $('.icon-iconset0143').click(function () {
        $('.pictureExampleModule').removeClass('hide');
    })
});