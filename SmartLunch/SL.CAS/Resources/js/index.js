$(function(){
    var imgsdata;
    $('.selectType .typeItem').click(function(){
        $(this).addClass('active');
        $(this).siblings('.typeItem').removeClass('active');
    })
    $('.ButtonUp').click(function(){
          $('#inputUpdata').click();  
    })
    $('#inputUpdata').change(
        function(e){
            console.log(e);
            var file = e.target.files || e.dataTransfer.files; 
            console.log(file)
            var file = this.files[0];                
            // 确认选择的文件是图片                
            if(file.type.indexOf("image") == 0) {
                var reader = new FileReader();
                reader.readAsDataURL(file);                    
                reader.onload = function(e) {
                    // 图片base64化
                    var newUrl = this.result;
                };
            }
        }
    )
})