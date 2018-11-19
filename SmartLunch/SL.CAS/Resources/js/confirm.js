

function promptBox(options){
    var defaults = {
    content: '我是提示框的主体内容',
	title:'提示',
	btn1Txt:'继续识别',
	confirmShow:'inline-block',
	btn2Txt:'商品列表',
	cancelShow:'inline-block',
	contentHeight:'',//主体内容的高度
	width:'',//整个弹窗的宽度
	callback:'',
	center:'center',//是否居中显示
	time:'',//自动关闭弹窗
	};
	var $body=document.getElementsByTagName('body')[0]
	$body.style.overflow='hidden'// 页面太长时  弹窗出现还是可以滚动 效果 不爽  直接body 超出的隐藏
	var $wrap=document.getElementById('wrap')//这个是添加到页面中的弹窗
	if($wrap){//有就删除 等哈下面在添加
		$body.removeChild($wrap)
	}
	for(var key in options){//将传过来的参数替换默认的参数
	   defaults[key]=options[key]
	}
	//*****这种写法兼容ie10+ 不包括ie10*****//
	//** ie10 不支持 es6的写法 如果 想要兼容的话 必须进行拼接*//
	var html = '<div class="model">	<div class="modelContent">	<div class="model-main" style="width:' + defaults.width + 'px">	<div class="modelTitle">	' + defaults.title + '	</div>	<div style="text-center:' + defaults.center + ';max-height:' + defaults.contentHeight + 'px;min-height:70px;padding:20px 4px">	' + defaults.content + '	</div>	<div class="btnContent">	<button class="confirmBtn modelBtn" style="display:' + defaults.confirmShow + '">' + defaults.btn1Txt + '</button>	<button class="cancelBtn modelBtn" style="display:' + defaults.cancelShow + '">' + defaults.btn2Txt + '</button>	</div>	</div>	</div>	<div>';

	var div=document.createElement('div')
	div.setAttribute('id','wrap')
	div.innerHTML=html
	$body.appendChild(div)
	var model=document.getElementsByClassName('model')[0]
	$wrap=document.getElementById('wrap')
	var confirmBtn=document.getElementsByClassName('confirmBtn')
	var cancelBtn=document.getElementsByClassName('cancelBtn')
	//if(model){//点击模态框删除 当前元素的父元素的节点
	//	model.onclick=function(e){
	//		if(e.target.className=='modelContent'){//判断点击的是 哪个遮罩层
	//			this.parentNode.parentNode.removeChild(this.parentNode)//删除节点
	//		}
	//	}
	//}else{
	//	console.log('no')
	//}
	if(defaults.time!=""){//时间不为空就是要自动关闭的
		setTimeout(function(){
			if($wrap){
				$body.removeChild($wrap)
			}
		},defaults.time)
	}
	for(var i=0;i<confirmBtn.length;i++){//循环为确认按钮绑定点击事件
		confirmBtn[i].onclick=function(){
			if(defaults.callback!=''){
				defaults.callback('0')
			}
		}
	}
	for(var i=0;i<cancelBtn.length;i++){//循环为取消按钮绑定点击事件
		cancelBtn[i].onclick=function(){
			if(defaults.callback!=''){
				defaults.callback('1')
			}
		}
	}
}

