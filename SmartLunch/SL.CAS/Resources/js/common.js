/*!
 * new common JavaScript Library v1.4.1
 * 2018-01-29
 */
;
"use strict";
(function (global, undefined) {
    //底层方法
    global.bs = (function () {
        var cloneDeep = function () {
            var options, name, src, copy, copyIsArray, clone,
                target = arguments[0] || {},
                i = 1,
                length = arguments.length,
                deep = false;
            // Handle a deep copy situation
            if (typeof target === "boolean") {
                deep = target;

                // Skip the boolean and the target
                target = arguments[i] || {};
                i++;
            }
            if (typeof target !== "object" && !isFunction(target)) {
                target = {};
            } // Extend jQuery itself if only one argument is passed
            if (i === length) {
                target = this;
                i--;
            }
            for (; i < length; i++) {
                // Only deal with non-null/undefined values
                if ((options = arguments[i]) != null) {
                    // Extend the base object
                    for (name in options) {
                        src = target[name];
                        copy = options[name];

                        // Prevent never-ending loop
                        if (target === copy) {
                            continue;
                        }
                        if (deep && copy && (isPlainObject(copy) ||
                            (copyIsArray = Array.isArray(copy)))) {

                            if (copyIsArray) {
                                copyIsArray = false;
                                clone = src && Array.isArray(src) ? src : [];

                            } else {
                                clone = src && isPlainObject(src) ? src : {};
                            }
                            // Never move original objects, clone them
                            target[name] = cloneDeep(deep, clone, copy);
                            // Don't bring in undefined values
                        } else if (copy !== undefined) {
                            target[name] = copy;
                        }
                    }
                }
            }

            // Return the modified object
            return target;
        }
        var isFunction = function (fn) {
            return Object.prototype.toString.call(fn) === '[object Function]';
        }
        var isArray = function (arr) {
            return Object.prototype.toString.call(arr) === '[object Array]';
        }
        var isPlainObject = function (obj) {
            var proto, Ctor;
            if (!obj || Object.prototype.toString.call(obj) !== "[object Object]") {
                return false;
            }
            proto = Object.getPrototypeOf(obj);
            if (!proto) {
                return true;
            }
            Ctor = {}.hasOwnProperty.call(proto, "constructor") && proto.constructor;
            return typeof Ctor === "function" && {}.hasOwnProperty.toString.call(Ctor) === {}.hasOwnProperty.toString.call(Object);
        }
        var stringBuffer = function () {
            this._strings = new Array();
        }
        stringBuffer.prototype.append = function (_string) {
            this._strings.push(_string);
        };
        stringBuffer.prototype.toString = function () {
            var temp = this._strings.join("");
            this._strings = [];
            return temp;
        };
        var ready = function (fn) {
            if (document.addEventListener) {
                //标准浏览器  
                document.addEventListener('DOMContentLoaded', function () {
                    //注销事件，避免反复触发  
                    document.removeEventListener('DOMContentLoaded', arguments.callee, false);
                    //执行函数   
                    fn();
                }, false);
            } else if (document.attachEvent) {
                //IE浏览器  
                document.attachEvent('onreadystatechange', function () {
                    if (document.readyState == 'complete') {
                        document.detachEvent('onreadystatechange', arguments.callee);
                        //执行函数   
                        fn();
                    }
                });
            }
        }
        var newGuid = function () {
            function S4() {
                return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1)
            }
            return (S4() + S4() + S4() + S4() + S4() + S4() + S4() + S4())
        }
        var isUndefinedOrNull = function (value) {
            return value === undefined || value === null
        }
        var unique = function (arr, key) {
            var res = [];
            if (isUndefinedOrNull(key)) {
                res = arr.filter(function (item, index, array) {
                    return array.indexOf(item) === index;
                });
            } else {
                arr.forEach(function (v, k) {
                    var index = res.findIndex(function (o) {
                        return o[key] == v[key]
                    });
                    if (index == -1) {
                        res.push(v);
                    }
                })
            }
            return res;
        }
        var isEmpty = function (value) {
            var isValue = isUndefinedOrNull(value);
            if (!isValue) {
                if (value.trim().length > 0) {
                    isValue = false;
                } else {
                    isValue = true
                }
            }
            return isValue;
        }
        var isObject = function (value) {
            var type = typeof value
            return value != null && (type == 'object' || type == 'function')
        }
        var each = function (obj, callback) {
            var length, i = 0;
            if (isArrayLike(obj)) {
                length = obj.length;
                for (; i < length; i++) {
                    if (callback.call(obj[i], i, obj[i]) === false) {
                        break;
                    }
                }
            } else {
                for (i in obj) {
                    if (callback.call(obj[i], i, obj[i]) === false) {
                        break;
                    }
                }
            }

            return obj;
        }
        var isArrayLike = function (value) {
            return value != null && typeof value != 'function' && (typeof value.length == 'number' && value.length > -1 && value.length % 1 == 0 && value.length <= 9007199254740991)
        }
        var chunk = function (array, subGroupLength) {
            if (!isUndefinedOrNull(subGroupLength)) {
                var index = 0;
                var newArray = [];
                while (index < array.length) {
                    newArray.push(array.slice(index, index += subGroupLength));
                }
                return newArray;
            } else {
                return array;
            }
        }
        var baseException = function (message) {
            this.message = message;
        }
        var swap = function (array, first, second) {
            var tmp = array[second];
            array[second] = array[first];
            array[first] = tmp;
            return array;
        }
        var ajax = function (options) {
            options = options || {};
            options.type = (options.type || "GET").toUpperCase();
            options.dataType = options.dataType || 'json';
            options.async = options.async || true;
            var params = getParams(options.data);
            options.beforeSend && options.beforeSend();

            var xhr = new XMLHttpRequest();
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 4) {
                    var status = xhr.status;
                    if ((xhr.status >= 200 && xhr.status < 300) || xhr.status == 304) {
                        options.success && options.success(xhr.responseText, xhr.status);
                    } else {
                        options.error && options.error(xhr.status);
                    }
                    options.complete && options.complete();
                }
            };
            if (options.type == 'GET') {
                var params = getParams(options.data);
                xhr.open("GET", options.url + '?' + params, options.async);
                xhr.send(null)
            } else if (options.type == 'POST') {
                xhr.open('POST', options.url, options.async);
                xhr.setRequestHeader('Content-Type', options.contentType || 'application/x-www-form-urlencoded');
                xhr.send(options.data);
            }

            function getParams(data) {
                var arr = [];
                for (var param in data) {
                    arr.push(encodeURIComponent(param) + '=' + encodeURIComponent(data[param]));
                }
                arr.push(('randomNumber=' + Math.random()).replace('.'));
                return arr.join('&');
            }
        }
        var getTemplate = function (tmpl) {
            var regEx = new RegExp("/\\*([\\S\\s]*)\\*/", "mg");
            tmpl = tmpl + "";
            var matches = tmpl.match(regEx) || [];
            var result = [];
            for (var i = 0; i < matches.length; i++) {
                result.push(matches[i].replace(regEx, "$1"));
            }
            return result.join("");
        }
        var fadeIn = function (el, speed, opacity, val) {
            speed = speed || 20;
            opacity = opacity || 100;
            val = val || 0;
            el.style.display = 'block';
            $bs(el).setOpacity(0);
            var t = setInterval(function () {
                if (val <= opacity) {
                    $bs(el).setOpacity(val);
                    val += 5;
                } else {
                    clearInterval(t);
                }
            }, speed);
        }
        var fadeOut = function (el, speed, opacity, val, isDelete) {
            speed = speed || 20;
            opacity = opacity || 0;
            val = val || 100;
            var t = setInterval(function () {
                if (val >= opacity) {
                    $bs(el).setOpacity(val);
                    val -= 5;
                } else if (val < 0) {
                    clearInterval(t);
                    if (isDelete) {
                        $bs(el).remove();
                    } else {
                        el.style.display = 'none';
                    }
                }
            }, speed);
        }
        var makeArray = function (values) {
            var arr = [];
            if (!isUndefinedOrNull(values)) {
                if (isArrayLike(values)) {
                    arr = Array.prototype.slice.call(values);
                } else {
                    for (var i = 0; i < values.length; i++) {
                        arr.push(values[i]);
                    }
                }
            }
            return arr;
        }
        var labelDrag = function (dragEl, el, modalDocument) {
            var down = false;
            var dx = 0;
            var dy = 0;
            var sx = 0;
            var sy = 0;
            modalDocument.onmousemove = function (e) {
                if (down) {
                    dragEl.style.cursor = "move";
                    var ev = e || event;
                    el.style.top = ev.clientY - (dy - sy) + 'px';
                    el.style.left = ev.clientX - (dx - sx) + 'px';
                }
            }
            dragEl.onmousedown = function (event) {
                dx = event.clientX;
                dy = event.clientY;
                sx = parseInt(el.offsetLeft);
                sy = parseInt(el.offsetTop);
                if (!down) {
                    down = true;
                }

            }
            modalDocument.onmouseup = function () {
                if (down) {
                    dragEl.style.cursor = "default";
                    down = false;
                }
            }
        }
        var animDelay = function (el, fn) {
            el.clientWidth;
            if (isFunction(fn)) {
                fn();
            }
        }
        var delay = function (fn, duration) {
            if (isFunction(fn)) {
                setTimeout(function () {
                    fn();
                }, duration || 300);
            }
        }
        var getHtml = function (selector) {
            var tpl = "";
            if (typeof selector === "string") {
                selector = selector.trim();
                if (!bs.isEmpty(selector)) {
                    if (selector[0] === "<" && selector[selector.length - 1] === ">" && selector.length >= 3) {
                        tpl = selector;
                    } else {
                        tpl = document.querySelector(selector).innerHTML;
                    }
                } else {
                    throw new bs.baseException('dom节点不能为空');
                }
            } else if (typeof selector === 'object') {
                tpl = selector.innerHTML;
            } else {
                throw new bs.baseException('dom节点不存在');
            }
            return tpl;
        }
        var http = function (ajax) {
            var promise = new Promise(function (resolve, reject) {
                $.when($.ajax(ajax)).then(function (result) {
                    resolve(result)
                }, function (err) {
                    reject(err)
                })
            });
            return promise;
        }
        return {
            extend: cloneDeep,
            cloneDeep: cloneDeep,
            isFunction: isFunction,
            isArray: isArray,
            isEmpty: isEmpty,
            isObject: isObject,
            isUndefinedOrNull: isUndefinedOrNull,
            stringBuffer: stringBuffer,
            newGuid: newGuid,
            ready: ready,
            each: each,
            unique: unique,
            ajax: ajax,
            swap: swap,
            fadeIn: fadeIn,
            fadeOut: fadeOut,
            makeArray: makeArray,
            chunk: chunk,
            labelDrag: labelDrag,
            animDelay: animDelay,
            delay: delay,
            getHtml: getHtml,
            getTemplate: getTemplate,
            http: http,
            baseException: baseException
        }
    })();
    //基本类型拓展
    (function () {
        if (!Array.prototype.find) {
            Array.prototype.find = function (predicate) {
                if (this == null) {
                    throw new bs.baseException('"this" is null or not defined');
                }
                var o = Object(this);
                var len = o.length >>> 0;
                if (typeof predicate !== 'function') {
                    throw new bs.baseException('predicate must be a function');
                }
                var thisArg = arguments[1];
                var k = 0;
                while (k < len) {
                    var kValue = o[k];
                    if (predicate.call(thisArg, kValue, k, o)) {
                        return kValue;
                    }
                    k++;
                }
                return undefined;
            }
        }
        if (!Array.prototype.findIndex) {
            Array.prototype.findIndex = function (predicate) {
                if (this === null) {
                    throw new TypeError('Array.prototype.findIndex called on null or undefined');
                }
                if (typeof predicate !== 'function') {
                    throw new TypeError('predicate must be a function');
                }
                var list = Object(this);
                var length = list.length >>> 0;
                var thisArg = arguments[1];
                var value;

                for (var i = 0; i < length; i++) {
                    value = list[i];
                    if (predicate.call(thisArg, value, i, list)) {
                        return i;
                    }
                }
                return -1;
            };
        }
        Array.prototype.isExist = function (cloumnName, value) {
            if (this == null) {
                throw new bs.baseException('"this" is null or not defined');
            }
            var index = -1;
            if (bs.isEmpty(value)) {
                index = this.findIndex(function (o) {
                    return o == cloumnName;
                })
            } else {
                index = this.findIndex(function (o) {
                    return o[cloumnName] == value;
                })
            }
            if (index == -1) {
                return false;
            } else {
                return true;
            }
        }
        Array.prototype.remove = function (predicate) {
            if (this == null) {
                throw new bs.baseException('"this" is null or not defined');
            }
            var o = Object(this);
            var len = o.length >>> 0;
            if (typeof predicate !== 'function') {
                throw new bs.baseException('predicate must be a function');
            }
            var k = 0;
            while (k < len) {
                var kValue = o[k];
                if (predicate.call(this, kValue, k, o)) {
                    this.splice(k, 1);
                    len--;
                } else {
                    k++;
                }
            }
        }
        Object.defineProperties(Array.prototype, {
            "isExist": {
                enumerable: false
            },
            "remove": {
                enumerable: false
            }
            // 等等.
        });
        Date.prototype.Format = function (fmt) {
            var o = {
                "M+": this.getMonth() + 1, //月份 
                "d+": this.getDate(), //日 
                "h+": this.getHours(), //小时 
                "m+": this.getMinutes(), //分 
                "s+": this.getSeconds(), //秒 
                "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
                "S": this.getMilliseconds() //毫秒 
            };
            if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
            for (var k in o)
                if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
            return fmt;
        }
        if (!Object.prototype.assign) {
            Object.defineProperty(Object, "assign", {
                value: function assign(target, varArgs) { // .length of function is 2
                    if (target == null) { // TypeError if undefined or null
                        throw new TypeError('Cannot convert undefined or null to object');
                    }
                    var to = Object(target);
                    for (var index = 1; index < arguments.length; index++) {
                        var nextSource = arguments[index];

                        if (nextSource != null) { // Skip over if undefined or null
                            for (var nextKey in nextSource) {
                                // Avoid bugs when hasOwnProperty is shadowed
                                if (Object.prototype.hasOwnProperty.call(nextSource, nextKey)) {
                                    to[nextKey] = nextSource[nextKey];
                                }
                            }
                        }
                    }
                    return to;
                },
                writable: true,
                configurable: true,
                enumerable: false
            });
        }
    })();
    //DOM操作
    (function () {
        var $bs = function (selector, context) {
            return new $bs.fn.F(selector, context);
        }
        $bs.fn = $bs.prototype = {
            constructor: $bs,
            F: function (selector, context) {
                if (bs.isUndefinedOrNull(selector)) {
                    return this;
                }
                if (Object.prototype.toString.call(selector) === "[object NodeList]") {
                    this.length = selector.length;
                    for (var i = 0; i < this.length; i += 1) {
                        this[i] = selector[i];
                    }
                    return this;
                } else if (typeof selector === "string") {
                    var els = (context || document).querySelectorAll(selector);
                    this.length = els.length;
                    for (var i = 0; i < this.length; i += 1) {
                        this[i] = els[i];
                    }
                    return this;
                } else if (selector.nodeType) {
                    this[0] = selector;
                    this.length = 1;
                    return this;
                } else {
                    throw new bs.baseException('参数异常');
                }
            },
            extend: bs.extend,
            each: function (callback) {
                return bs.each(this, callback);
            }
        }
        $bs.fn.F.prototype = $bs.fn;
        $bs.fn.extend({
            parent: function () {
                if (this.length > 0) {
                    var parent = this[0].parentNode;
                    return parent && parent.nodeType !== 11 ? parent : null;
                } else {
                    return this;
                }
            },
            hasClass: function (selector) {
                for (var i = 0; i < this.length; i++) {
                    if (this[i].nodeType === 1 && (" " + (this[i].className.match(/[^\x20\t\r\n\f]+/g) || []).join(" ") + " ").indexOf(" " + selector + " ") > -1) {
                        return true;
                    }
                }
                return false;
            },
            addClass: function (selector) {
                for (var i = 0; i < this.length; i++) {
                    if (!$bs(this[i]).hasClass(selector)) {
                        this[i].className += (this[i].className ? " " : "") + selector;
                    }
                }
                return this;
            },
            removeClass: function (selector) {
                for (var i = 0; i < this.length; i++) {
                    if ($bs(this[i]).hasClass(selector)) {
                        var reg = new RegExp('(\\s|^)' + selector + '(\\s|$)');
                        this[i].className = this[i].className.replace(reg, ' ');
                    }
                }
                return this;
            },
            toggleClass: function (selector) {
                for (var i = 0; i < this.length; i++) {
                    if ($bs(this[i]).hasClass(selector)) {
                        $bs(this[i]).removeClass(selector);
                    } else {
                        $bs(this[i]).addClass(selector);
                    }
                }
            },
            remove: function () {
                for (var i = 0; i < this.length; i++) {
                    this[i].parentNode.removeChild(this[i]);
                }
                return this;
            },
            siblings: function () {
                var els = [];
                for (var i = 0; i < this.length; i++) {
                    var n = this[i].parentNode.firstChild;
                    for (; n; n = n.nextSibling) {
                        if (n.nodeType === 1 && n !== this[i]) {
                            els.push(n);
                        }
                    }
                }
                return els;
            },
            find: function (selector) {
                if (bs.isEmpty(selector)) {
                    return this;
                }
                if (this.length > 0) {
                    if (!bs.isUndefinedOrNull(this[0].querySelector(selector))) {
                        this[0] = this[0].querySelector(selector);
                        this.length = 1;
                    } else {
                        delete this[0];
                        this.length = 0;
                    }
                } else {
                    delete this[0];
                    this.length = 0;
                }
                return this;
            },
            findAll: function (selector) {
                if (bs.isEmpty(selector)) {
                    return this;
                }
                var els = [],
                    k = 0;;
                for (var i = 0; i < this.length; i++) {
                    var tels = this[i].querySelectorAll(selector);
                    for (var j = 0; j < tels.length; j++) {
                        els[k] = tels[j];
                        k++;
                    }
                }
                for (var i = 0; i < els.length; i++) {
                    this[i] = els[i];
                }
                this.length = els.length;
                return this;
            },
            setOpacity: function (value) { //设置透明度
                for (var i = 0; i < this.length; i++) {
                    this[i].filters ? this[i].style.filter = 'alpha(opacity=' + value + ')' : this[i].style.opacity = value / 100;
                }
                return this;
            },
            fadeIn: function (speed, opacity, val) {
                for (var i = 0; i < this.length; i++) {
                    bs.fadeIn(this[i], speed, opacity, val);
                }
            },
            fadeOut: function (speed, opacity, val) {
                for (var i = 0; i < this.length; i++) {
                    bs.fadeOut(this[i], speed, opacity, val);
                }
            },
            hide: function () {
                for (var i = 0; i < this.length; i++) {
                    this[i].style.display = 'none';
                }
                return this;
            },
            show: function () {
                for (var i = 0; i < this.length; i++) {
                    this[i].style.display = 'block';
                }
                return this;
            },
            on: function (type, callback) {
                if (!callback) {
                    throw new bs.baseException('参数异常');
                }
                this.each(function (i, v) {
                    v.onclick = (function (v, i) {
                        return function (e) {
                            if (callback) {
                                callback.call(v, e, v, i)
                            }
                        }
                    })(v, i)
                    // v.addEventListener(type, (function (v, i) {
                    //     return function (e) {
                    //         if (callback) {
                    //             callback.call(v, e, v, i)
                    //         }
                    //     }
                    // })(v, i), false)
                });
                return this;
            },
            forEach: function (callback) {
                for (var i = 0; i < this.length; i++) {
                    callback.call(callback, i, this[i]);
                }
            },
            click: function (callback) {
                return this.on('click', callback);
            },
            children: function () {
                var els = [],
                    k = 0;;
                for (var i = 0; i < this.length; i++) {
                    if (this[i].children.length > 0) {
                        for (var j = 0; j < this[i].children.length; j++) {
                            els[k] = this[i].children[j];
                            k++;
                        }
                    }
                }
                for (var i = 0; i < els.length; i++) {
                    this[i] = els[i];
                }
                this.length = els.length;
                return this;
            },
            prop: function (value) {
                if (!bs.isUndefinedOrNull(value)) {
                    if (this.length > 0) {
                        return this[0][value];
                    }
                }
            },
            css: function (key, value) {
                if (bs.isUndefinedOrNull(value)) {
                    if (this.length > 0) {
                        return this[i].style[key];
                    } else {
                        return -1;
                    }
                } else {
                    for (var i = 0; i < this.length; i++) {
                        this[i].style[key] = value;
                    }
                }
            },
            rootParent: function (stopValue) {
                if (this.length > 0) {
                    var _self = this[0];
                    while (_self != null && !$bs(_self).hasClass(stopValue)) {
                        _self = $bs(_self).parent();
                    }
                    this[0] = _self;
                    this.length = 1;
                    return this;
                } else {
                    return this;
                }
            },
            attr: function (key, value) {
                if (this.length >= 1) {
                    if (bs.isUndefinedOrNull(value)) {
                        return this[0].getAttribute(key);
                    } else {
                        this.forEach(function (k, v) {
                            v.setAttribute(key, value);
                        })
                        return this;
                    }
                } else {
                    return this;
                }
            }
        })
        global.$bs = $bs;
    })();
    //模板渲染
    var template = (function () {
        var common = {
            query: function (type, _, __) {
                var types = [
                    '#([\\s\\S])+?',
                    '([^{#}])*?'
                ][type || 0];
                return new RegExp(((_ || '') + "{{" + types + "}}" + (__ || '')), 'g');
            },
            escape: function (html) {
                return String(html || '').replace(/&(?!#?[a-zA-Z0-9]+;)/g, '&amp;')
                    .replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/'/g, '&#39;').replace(/"/g, '&quot;');
            }
        };
        var compile = function () {
            this.cache = null;
        }
        compile.prototype.compiler = function (html, data) {
            var that = this,
                temp = null;
            var jss = new RegExp('^{{#', 'g'),
                jsse = new RegExp('}}$', 'g');
            temp = html.replace(/\s+|\r|\t|\n/g, ' ').replace(new RegExp('{{#', 'g'), '{{# ')
                .replace(new RegExp('}}}', 'g'), '} }}').replace(/\\/g, '\\\\')
                .replace(/(?="|')/g, '\\').replace(common.query(), function (str) {
                    str = str.replace(jss, '').replace(jsse, '');
                    return '";' + str.replace(/\\/g, '') + ';view+="';
                })
                .replace(common.query(1), function (str) {
                    var start = '"+(';
                    if (str.replace(/\s/g, '') === "{{}}") {
                        return '';
                    }
                    str = str.replace(new RegExp('{{|}}', 'g'), '');
                    if (/^=/.test(str)) {
                        str = str.replace(/^=/, '');
                        start = '"+_escape_(';
                    }
                    return start + str.replace(/\\/g, '') + ')+"';
                });

            temp = '"use strict"; var view = "' + temp + '"; return view;';
            try {
                that.cache = temp = new Function('d, _escape_', temp);
                return temp(data, common.escape);
            } catch (e) {
                delete that.cache;
                return new bs.baseException(e);;
            }
        }
        var render = function (selector, data, callback, flag) {
            flag = flag || false;
            var tpl = flag ? selector : bs.getHtml(selector);
            var com = new compile();
            tpl = com.cache ? com.cache(data, common.escape) : com.compiler(tpl, data);
            if (!callback)
                return tpl;
            callback(tpl);
        }
        return {
            render: render
        };
    })();
    var pager = function (options) {
        if (this.length < 0) {
            return this;
        }
        if (!bs.isUndefinedOrNull(this[0].querySelector(".bs-pagination"))) {
            $bs(this[0].querySelector(".bs-pagination")).remove();
        }
        var opts = bs.extend(true, {}, {
            pageIndex: 1,
            pageSize: 15,
            dataCount: 0,
            totalPages: 0,
            callback: null,
            isNeedSelect: false
        }, options);
        var pageDom = document.createElement("div");
        pageDom.className = "bs-pagination";
        var buffer = new bs.stringBuffer();
        buffer.append('<div class="pages-left">共有' + opts.dataCount + '项记录</div><div class="pages-right"><div class="pre"><i></i></div><div class="pageInfo">' + opts.pageIndex + '/');
        buffer.append(opts.totalPages + '</div><div class="next"><i></i></div><span>前往</span><input type="text" class="current input-text page-index-user" style="width: 60px;padding:0">页');
        if (opts.isNeedSelect) {
            buffer.append('<select class="current page-size-select bs-select">');
            [15, 30, 50].forEach(function (item, index) {
                buffer.append('<option ' + (item == opts.pageSize ? " selected " : "") + 'value="' + item + '">' + item + '</option>');
            })
            buffer.append('</select>');
        }
        buffer.append('<input type="button" class="bs-btn btn-default current" value="跳转"></div>');
        pageDom.innerHTML = buffer.toString();
        pageDom.querySelector('.pre').onclick = function () {
            if (opts.pageIndex <= 1) {
                bs.info("当前为第一页");
                return false;
            } else {
                opts.pageIndex--;
                if (opts.callback) {
                    opts.callback(opts.pageIndex, opts.pageSize);
                }
            }
        }
        pageDom.querySelector('.next').onclick = function () {
            if (opts.pageIndex >= opts.totalPages) {
                bs.info("当前为最后一页");
                return false;
            } else {
                opts.pageIndex++;
                if (opts.callback) {
                    opts.callback(opts.pageIndex, opts.pageSize);
                }
            }
        }
        pageDom.querySelector('.btn-default').onclick = function () {
            var temp = this.parentNode.querySelector(" .page-index-user").value;
            if (!bs.isEmpty(temp)) {
                temp = Number(temp);
                if (/^\+?[1-9][0-9]*$/.test(temp) && temp >= 1 && temp <= opts.totalPages) {
                    opts.pageIndex = temp;
                    if (opts.callback) {
                        opts.callback(opts.pageIndex, opts.pageSize);
                    }
                } else {
                    bs.warning("非法输入");
                    return false;
                }
            } else {
                bs.info("请输入正整数");
                return false;
            }
        }
        if (opts.isNeedSelect) {
            pageDom.querySelector(".page-size-select").onchange = function () {
                opts.pageIndex = 1;
                opts.pageSize = Number(this.value);
                if (opts.callback) {
                    opts.callback(opts.pageIndex, opts.pageSize);
                }
            }
        }
        this[0].appendChild(pageDom);
    }
    $bs.fn.extend({
        pager: pager
    });
    //提示插件
    var toast = (function () {
        var alert = function (options) {
            var opts = bs.extend({}, {
                title: '提示',
                content: '内容',
                text: '确定',
                btnCallBack: null
            }, options)
            var el = document.createElement("div");
            var id = bs.newGuid();
            el.id = id;
            el.innerHTML = '<div class="bs-modal-backdrop mask-fade mask-in"></div><div tabindex="-1" class="el-message-box__wrapper"><div class="el-message-box"><div class="el-message-box__header"><div class="el-message-box__title">' +
                opts.title + '</div><button type="button" aria-label="Close" class="el-message-box__headerbtn"><i class="el-message-box__close el-icon-close"></i></button></div>\
                <div class="el-message-box__content"><div class="el-message-box__status"></div><div class="el-message-box__message"><p>' + opts.content + '</p>\
                </div></div><div class="el-message-box__btns"><button type="button" class="el-button el-button--default el-button--primary save"><span>' + opts.text +
                '</span></button></div></div></div>'
            el.querySelector(".el-message-box__wrapper").style.zIndex = 6000;
            el.querySelector(".bs-modal-backdrop").style.zIndex = 5999;
            window.document.querySelector("body").appendChild(el);
            el.querySelector(".save").onclick = function (e) {
                if (opts.btnCallBack) {
                    opts.btnCallBack.call(this, el)
                }
                window.document.getElementById(id).remove()
            }
        }
        var confirm = function (options) {
            var opts = bs.extend({}, {
                title: '提示',
                content: '内容',
                cancel: '取消',
                text: '确定',
                btnCallBack: null,
                cancelCallBack: null,
            }, options)
            var el = document.createElement("div");
            var id = bs.newGuid();
            el.id = id;
            el.innerHTML = '<div class="bs-modal-backdrop mask-fade mask-in"></div><div tabindex="-1" class="el-message-box__wrapper"><div class="el-message-box"><div class="el-message-box__header">\
                            <div class="el-message-box__title">' + opts.title + '</div><button type="button" aria-label="Close" class="el-message-box__headerbtn">\
                            <i class="el-message-box__close el-icon-close"></i></button></div><div class="el-message-box__content"><div class="el-message-box__status"></div>\
                            <div class="el-message-box__message"><p>' + opts.content + '</p></div></div><div class="el-message-box__btns"><button type="button" class="el-button el-button--default cancel"><span>' +
                opts.cancel + '</span></button><button type="button" class="el-button el-button--default el-button--primary save"><span>' + opts.text + '</span></button></div></div></div>'
            el.querySelector(".el-message-box__wrapper").style.zIndex = 6000;
            el.querySelector(".bs-modal-backdrop").style.zIndex = 5999;
            window.document.querySelector("body").appendChild(el);
            el.querySelector(".cancel").onclick = function () {
                if (opts.cancelCallBack) {
                    opts.cancelCallBack.call(this, el)
                }
                window.document.getElementById(id).remove()
            }
            el.querySelector(".save").onclick = function (e) {
                if (opts.btnCallBack) {
                    opts.btnCallBack.call(this, el)
                }
                window.document.getElementById(id).remove()
            }
        }
        var message = function (options) {
            var opts = bs.extend({}, {
                type: 'info',
                content: '内容',
                duration: 3000
            }, options)
            var el = document.createElement("div");
            el.className = 'el-message fade'
            // el.style.zIndex = 4000
            var id = bs.newGuid();
            el.id = id;
            if (opts.type == 'success') {
                el.innerHTML = '<img src="data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9Im5vIj8+Cjxzdmcgd2lkdGg9IjQwcHgiIGhlaWdodD0iNDBweCIgdmlld0JveD0iMCAwIDQwIDQwIiB2ZXJzaW9uPSIxLjEiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgeG1sbnM6eGxpbms9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkveGxpbmsiPgogICAgPCEtLSBHZW5lcmF0b3I6IFNrZXRjaCAzOS4xICgzMTcyMCkgLSBodHRwOi8vd3d3LmJvaGVtaWFuY29kaW5nLmNvbS9za2V0Y2ggLS0+CiAgICA8dGl0bGU+aWNvbl9zdWNjZXNzPC90aXRsZT4KICAgIDxkZXNjPkNyZWF0ZWQgd2l0aCBTa2V0Y2guPC9kZXNjPgogICAgPGRlZnM+PC9kZWZzPgogICAgPGcgaWQ9IkVsZW1lbnQtZ3VpZGVsaW5lLXYwLjIuNCIgc3Ryb2tlPSJub25lIiBzdHJva2Utd2lkdGg9IjEiIGZpbGw9Im5vbmUiIGZpbGwtcnVsZT0iZXZlbm9kZCI+CiAgICAgICAgPGcgaWQ9Ik1lc3NhZ2UiIHRyYW5zZm9ybT0idHJhbnNsYXRlKC02MC4wMDAwMDAsIC0yMTIuMDAwMDAwKSI+CiAgICAgICAgICAgIDxnIGlkPSLluKblgL7lkJFf5L+h5oGvIiB0cmFuc2Zvcm09InRyYW5zbGF0ZSg2MC4wMDAwMDAsIDIxMi4wMDAwMDApIj4KICAgICAgICAgICAgICAgIDxnIGlkPSJSZWN0YW5nbGUtMiI+CiAgICAgICAgICAgICAgICAgICAgPGcgaWQ9Imljb25fc3VjY2VzcyI+CiAgICAgICAgICAgICAgICAgICAgICAgIDxyZWN0IGlkPSJSZWN0YW5nbGUtMiIgZmlsbD0iIzEzQ0U2NiIgeD0iMCIgeT0iMCIgd2lkdGg9IjQwIiBoZWlnaHQ9IjQwIj48L3JlY3Q+CiAgICAgICAgICAgICAgICAgICAgICAgIDxwYXRoIGQ9Ik0yNy44MjU1ODE0LDE3LjE0ODQzNTcgTDE5LjAxNzQ0LDI1LjgyODEyMTMgQzE4LjkwMTE2MDksMjUuOTQyNzA4MyAxOC43NjU1MDMzLDI2IDE4LjYxMDQ2NywyNiBDMTguNDU1NDI3LDI2IDE4LjMxOTc2OTMsMjUuOTQyNzA4MyAxOC4yMDM0ODY1LDI1LjgyODEyMTMgTDE4LjAyOTA3MTYsMjUuNjU2MjUgTDEzLjE3NDQxODYsMjAuODQzNzUgQzEzLjA1ODEzOTUsMjAuNzI5MTYzIDEzLDIwLjU5NTQ4MzcgMTMsMjAuNDQyNzA0NyBDMTMsMjAuMjg5OTI5MyAxMy4wNTgxMzk1LDIwLjE1NjI1IDEzLjE3NDQxODYsMjAuMDQxNjY2NyBMMTQuMzY2Mjc3MiwxOC44NjcxODU3IEMxNC40ODI1NiwxOC43NTI2MDIzIDE0LjYxODIxNzcsMTguNjk1MzEwNyAxNC43NzMyNTc3LDE4LjY5NTMxMDcgQzE0LjkyODI5NCwxOC42OTUzMTA3IDE1LjA2Mzk1MTYsMTguNzUyNjAyMyAxNS4xODAyMzA3LDE4Ljg2NzE4NTcgTDE4LjYxMDQ2NywyMi4yNzYwMzggTDI1LjgxOTc2OTMsMTUuMTcxODcxMyBDMjUuOTM2MDQ4NCwxNS4wNTcyODggMjYuMDcxNzA2LDE1IDI2LjIyNjc0MjMsMTUgQzI2LjM4MTc4MjMsMTUgMjYuNTE3NDQsMTUuMDU3Mjg4IDI2LjYzMzcyMjgsMTUuMTcxODcxMyBMMjcuODI1NTgxNCwxNi4zNDYzNTIzIEMyNy45NDE4NjA1LDE2LjQ2MDkzNTcgMjgsMTYuNTk0NjE1IDI4LDE2Ljc0NzM5NCBDMjgsMTYuOTAwMTczIDI3Ljk0MTg2MDUsMTcuMDMzODUyMyAyNy44MjU1ODE0LDE3LjE0ODQzNTcgTDI3LjgyNTU4MTQsMTcuMTQ4NDM1NyBaIiBpZD0iUGF0aCIgZmlsbD0iI0ZGRkZGRiI+PC9wYXRoPgogICAgICAgICAgICAgICAgICAgIDwvZz4KICAgICAgICAgICAgICAgIDwvZz4KICAgICAgICAgICAgPC9nPgogICAgICAgIDwvZz4KICAgIDwvZz4KPC9zdmc+" alt="" class="el-message__img">\
                <div class="el-message__group"><p>' + opts.content + '</p><div class="el-message__closeBtn el-icon-close"></div></div>'
            } else if (opts.type == 'error') {
                el.innerHTML = '<img src="data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9Im5vIj8+Cjxzdmcgd2lkdGg9IjQwcHgiIGhlaWdodD0iNDBweCIgdmlld0JveD0iMCAwIDQwIDQwIiB2ZXJzaW9uPSIxLjEiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgeG1sbnM6eGxpbms9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkveGxpbmsiPgogICAgPCEtLSBHZW5lcmF0b3I6IFNrZXRjaCAzOS4xICgzMTcyMCkgLSBodHRwOi8vd3d3LmJvaGVtaWFuY29kaW5nLmNvbS9za2V0Y2ggLS0+CiAgICA8dGl0bGU+aWNvbl9kYW5nZXI8L3RpdGxlPgogICAgPGRlc2M+Q3JlYXRlZCB3aXRoIFNrZXRjaC48L2Rlc2M+CiAgICA8ZGVmcz48L2RlZnM+CiAgICA8ZyBpZD0iRWxlbWVudC1ndWlkZWxpbmUtdjAuMi40IiBzdHJva2U9Im5vbmUiIHN0cm9rZS13aWR0aD0iMSIgZmlsbD0ibm9uZSIgZmlsbC1ydWxlPSJldmVub2RkIj4KICAgICAgICA8ZyBpZD0iTWVzc2FnZSIgdHJhbnNmb3JtPSJ0cmFuc2xhdGUoLTYwLjAwMDAwMCwgLTMzMi4wMDAwMDApIj4KICAgICAgICAgICAgPGcgaWQ9IuW4puWAvuWQkV/kv6Hmga8iIHRyYW5zZm9ybT0idHJhbnNsYXRlKDYwLjAwMDAwMCwgMzMyLjAwMDAwMCkiPgogICAgICAgICAgICAgICAgPGcgaWQ9IlJlY3RhbmdsZS0yIj4KICAgICAgICAgICAgICAgICAgICA8ZyBpZD0iaWNvbl9kYW5nZXIiPgogICAgICAgICAgICAgICAgICAgICAgICA8cmVjdCBpZD0iUmVjdGFuZ2xlLTIiIGZpbGw9IiNGRjQ5NDkiIHg9IjAiIHk9IjAiIHdpZHRoPSI0MCIgaGVpZ2h0PSI0MCI+PC9yZWN0PgogICAgICAgICAgICAgICAgICAgICAgICA8cGF0aCBkPSJNMjUuODE3MjYyNywxNi4zNDUxNzk2IEMyNS45MzkwOTAyLDE2LjIyMzM0ODMgMjYsMTYuMDc2MTQxOCAyNiwxNS45MDM1NTIzIEMyNiwxNS43MzA5NjI4IDI1LjkzOTA5MDIsMTUuNTgzNzU2MyAyNS44MTcyNjI3LDE1LjQ2MTkyODkgTDI0LjUwNzYxNTcsMTQuMTgyNzQxMSBDMjQuMzg1Nzg4MiwxNC4wNjA5MTM3IDI0LjI0MzY1NzUsMTQgMjQuMDgxMjE5NiwxNCBDMjMuOTE4NzgxNywxNCAyMy43NzY2NTEsMTQuMDYwOTEzNyAyMy42NTQ4MjM1LDE0LjE4Mjc0MTEgTDIwLDE3LjgzNzU2MzUgTDE2LjMxNDcyMTYsMTQuMTgyNzQxMSBDMTYuMTkyODkwMiwxNC4wNjA5MTM3IDE2LjA1MDc1OTUsMTQgMTUuODg4MzIxNiwxNCBDMTUuNzI1ODg3NiwxNCAxNS41ODM3NTY5LDE0LjA2MDkxMzcgMTUuNDYxOTI5NCwxNC4xODI3NDExIEwxNC4xNTIyODI0LDE1LjQ2MTkyODkgQzE0LjA1MDc1ODIsMTUuNTgzNzU2MyAxNCwxNS43MzA5NjI4IDE0LDE1LjkwMzU1MjMgQzE0LDE2LjA3NjE0MTggMTQuMDUwNzU4MiwxNi4yMjMzNDgzIDE0LjE1MjI4MjQsMTYuMzQ1MTc5NiBMMTcuODM3NTYwOCwyMC4wMDAwMDE5IEwxNC4xNTIyODI0LDIzLjY1NDgyNDMgQzE0LjA1MDc1ODIsMjMuNzc2NjUxNyAxNCwyMy45MjM4NTgyIDE0LDI0LjA5NjQ0NzcgQzE0LDI0LjI2OTAzNzIgMTQuMDUwNzU4MiwyNC40MTYyNDM3IDE0LjE1MjI4MjQsMjQuNTM4MDcxMSBMMTUuNDYxOTI5NCwyNS44MTcyNTg5IEMxNS41ODM3NTY5LDI1LjkzOTA4NjMgMTUuNzI1ODg3NiwyNiAxNS44ODgzMjE2LDI2IEMxNi4wNTA3NTk1LDI2IDE2LjE5Mjg5MDIsMjUuOTM5MDg2MyAxNi4zMTQ3MjE2LDI1LjgxNzI1ODkgTDIwLDIyLjE2MjQzNjUgTDIzLjY1NDgyMzUsMjUuODE3MjU4OSBDMjMuNzc2NjUxLDI1LjkzOTA4NjMgMjMuOTE4NzgxNywyNiAyNC4wODEyMTk2LDI2IEMyNC4yNDM2NTc1LDI2IDI0LjM4NTc4ODIsMjUuOTM5MDg2MyAyNC41MDc2MTU3LDI1LjgxNzI1ODkgTDI1LjgxNzI2MjcsMjQuNTM4MDcxMSBDMjUuOTM5MDkwMiwyNC40MTYyNDM3IDI2LDI0LjI2OTAzNzIgMjYsMjQuMDk2NDQ3NyBDMjYsMjMuOTIzODU4MiAyNS45MzkwOTAyLDIzLjc3NjY1MTcgMjUuODE3MjYyNywyMy42NTQ4MjQzIEwyMi4xMzE5ODA0LDIwLjAwMDAwMTkgTDI1LjgxNzI2MjcsMTYuMzQ1MTc5NiBaIiBpZD0iUGF0aCIgZmlsbD0iI0ZGRkZGRiI+PC9wYXRoPgogICAgICAgICAgICAgICAgICAgIDwvZz4KICAgICAgICAgICAgICAgIDwvZz4KICAgICAgICAgICAgPC9nPgogICAgICAgIDwvZz4KICAgIDwvZz4KPC9zdmc+" alt="" class="el-message__img">\
                <div class="el-message__group"><p>' + opts.content + '</p><div class="el-message__closeBtn el-icon-close"></div></div>'
            } else if (opts.type == 'warning') {
                el.innerHTML = '<img src="data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9Im5vIj8+Cjxzdmcgd2lkdGg9IjQwcHgiIGhlaWdodD0iNDBweCIgdmlld0JveD0iMCAwIDQwIDQwIiB2ZXJzaW9uPSIxLjEiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgeG1sbnM6eGxpbms9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkveGxpbmsiPgogICAgPCEtLSBHZW5lcmF0b3I6IFNrZXRjaCAzOS4xICgzMTcyMCkgLSBodHRwOi8vd3d3LmJvaGVtaWFuY29kaW5nLmNvbS9za2V0Y2ggLS0+CiAgICA8dGl0bGU+aWNvbl93YXJuaW5nPC90aXRsZT4KICAgIDxkZXNjPkNyZWF0ZWQgd2l0aCBTa2V0Y2guPC9kZXNjPgogICAgPGRlZnM+PC9kZWZzPgogICAgPGcgaWQ9IlBhZ2UtMSIgc3Ryb2tlPSJub25lIiBzdHJva2Utd2lkdGg9IjEiIGZpbGw9Im5vbmUiIGZpbGwtcnVsZT0iZXZlbm9kZCI+CiAgICAgICAgPGcgaWQ9Ik1lc3NhZ2UiIHRyYW5zZm9ybT0idHJhbnNsYXRlKC02MC4wMDAwMDAsIC0yNzIuMDAwMDAwKSI+CiAgICAgICAgICAgIDxnIGlkPSLluKblgL7lkJFf5L+h5oGvLWNvcHkiIHRyYW5zZm9ybT0idHJhbnNsYXRlKDYwLjAwMDAwMCwgMjcyLjAwMDAwMCkiPgogICAgICAgICAgICAgICAgPGcgaWQ9IlJlY3RhbmdsZS0yIj4KICAgICAgICAgICAgICAgICAgICA8ZyBpZD0iaWNvbl93YXJuaW5nIj4KICAgICAgICAgICAgICAgICAgICAgICAgPHJlY3QgaWQ9IlJlY3RhbmdsZS0yIiBmaWxsPSIjRjdCQTJBIiB4PSIwIiB5PSIwIiB3aWR0aD0iNDAiIGhlaWdodD0iNDAiPjwvcmVjdD4KICAgICAgICAgICAgICAgICAgICAgICAgPHBhdGggZD0iTTIxLjYxNTM4NDYsMjYuNTQzMjA5OSBDMjEuNjE1Mzg0NiwyNi45NDc4NzUxIDIxLjQ1ODMzNDgsMjcuMjkxODM2OCAyMS4xNDQyMzA4LDI3LjU3NTEwMjkgQzIwLjgzMDEyNjgsMjcuODU4MzY4OSAyMC40NDg3MTk0LDI4IDIwLDI4IEMxOS41NTEyODA2LDI4IDE5LjE2OTg3MzIsMjcuODU4MzY4OSAxOC44NTU3NjkyLDI3LjU3NTEwMjkgQzE4LjU0MTY2NTIsMjcuMjkxODM2OCAxOC4zODQ2MTU0LDI2Ljk0Nzg3NTEgMTguMzg0NjE1NCwyNi41NDMyMDk5IEwxOC4zODQ2MTU0LDE5Ljc0NDg1NiBDMTguMzg0NjE1NCwxOS4zNDAxOTA3IDE4LjU0MTY2NTIsMTguOTk2MjI5IDE4Ljg1NTc2OTIsMTguNzEyOTYzIEMxOS4xNjk4NzMyLDE4LjQyOTY5NjkgMTkuNTUxMjgwNiwxOC4yODgwNjU4IDIwLDE4LjI4ODA2NTggQzIwLjQ0ODcxOTQsMTguMjg4MDY1OCAyMC44MzAxMjY4LDE4LjQyOTY5NjkgMjEuMTQ0MjMwOCwxOC43MTI5NjMgQzIxLjQ1ODMzNDgsMTguOTk2MjI5IDIxLjYxNTM4NDYsMTkuMzQwMTkwNyAyMS42MTUzODQ2LDE5Ljc0NDg1NiBMMjEuNjE1Mzg0NiwyNi41NDMyMDk5IFogTTIwLDE1LjgwNDI5ODEgQzE5LjQ0NDQ0MjcsMTUuODA0Mjk4MSAxOC45NzIyMjQsMTUuNjE5MzY4NyAxOC41ODMzMzMzLDE1LjI0OTUwNDYgQzE4LjE5NDQ0MjcsMTQuODc5NjQwNiAxOCwxNC40MzA1MjU1IDE4LDEzLjkwMjE0OTEgQzE4LDEzLjM3Mzc3MjYgMTguMTk0NDQyNywxMi45MjQ2NTc1IDE4LjU4MzMzMzMsMTIuNTU0NzkzNSBDMTguOTcyMjI0LDEyLjE4NDkyOTUgMTkuNDQ0NDQyNywxMiAyMCwxMiBDMjAuNTU1NTU3MywxMiAyMS4wMjc3NzYsMTIuMTg0OTI5NSAyMS40MTY2NjY3LDEyLjU1NDc5MzUgQzIxLjgwNTU1NzMsMTIuOTI0NjU3NSAyMiwxMy4zNzM3NzI2IDIyLDEzLjkwMjE0OTEgQzIyLDE0LjQzMDUyNTUgMjEuODA1NTU3MywxNC44Nzk2NDA2IDIxLjQxNjY2NjcsMTUuMjQ5NTA0NiBDMjEuMDI3Nzc2LDE1LjYxOTM2ODcgMjAuNTU1NTU3MywxNS44MDQyOTgxIDIwLDE1LjgwNDI5ODEgWiIgaWQ9IkNvbWJpbmVkLVNoYXBlIiBmaWxsPSIjRkZGRkZGIiB0cmFuc2Zvcm09InRyYW5zbGF0ZSgyMC4wMDAwMDAsIDIwLjAwMDAwMCkgc2NhbGUoMSwgLTEpIHRyYW5zbGF0ZSgtMjAuMDAwMDAwLCAtMjAuMDAwMDAwKSAiPjwvcGF0aD4KICAgICAgICAgICAgICAgICAgICA8L2c+CiAgICAgICAgICAgICAgICA8L2c+CiAgICAgICAgICAgIDwvZz4KICAgICAgICA8L2c+CiAgICA8L2c+Cjwvc3ZnPg==" alt="" class="el-message__img">\
                <div class="el-message__group"><p>' + opts.content + '</p><div class="el-message__closeBtn el-icon-close"></div></div>'
            } else {
                el.innerHTML = '<img src="data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9Im5vIj8+Cjxzdmcgd2lkdGg9IjQwcHgiIGhlaWdodD0iNDBweCIgdmlld0JveD0iMCAwIDQwIDQwIiB2ZXJzaW9uPSIxLjEiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgeG1sbnM6eGxpbms9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkveGxpbmsiPgogICAgPCEtLSBHZW5lcmF0b3I6IFNrZXRjaCAzOS4xICgzMTcyMCkgLSBodHRwOi8vd3d3LmJvaGVtaWFuY29kaW5nLmNvbS9za2V0Y2ggLS0+CiAgICA8dGl0bGU+aWNvbl9pbmZvPC90aXRsZT4KICAgIDxkZXNjPkNyZWF0ZWQgd2l0aCBTa2V0Y2guPC9kZXNjPgogICAgPGRlZnM+PC9kZWZzPgogICAgPGcgaWQ9IkVsZW1lbnQtZ3VpZGVsaW5lLXYwLjIuNCIgc3Ryb2tlPSJub25lIiBzdHJva2Utd2lkdGg9IjEiIGZpbGw9Im5vbmUiIGZpbGwtcnVsZT0iZXZlbm9kZCI+CiAgICAgICAgPGcgaWQ9Ik1lc3NhZ2UiIHRyYW5zZm9ybT0idHJhbnNsYXRlKC02MC4wMDAwMDAsIC0xNTIuMDAwMDAwKSI+CiAgICAgICAgICAgIDxnIGlkPSLluKblgL7lkJFf5L+h5oGvIiB0cmFuc2Zvcm09InRyYW5zbGF0ZSg2MC4wMDAwMDAsIDE1Mi4wMDAwMDApIj4KICAgICAgICAgICAgICAgIDxnIGlkPSJSZWN0YW5nbGUtMiI+CiAgICAgICAgICAgICAgICAgICAgPGcgaWQ9Imljb25faW5mbyI+CiAgICAgICAgICAgICAgICAgICAgICAgIDxyZWN0IGlkPSJSZWN0YW5nbGUtMiIgZmlsbD0iIzUwQkZGRiIgeD0iMCIgeT0iMCIgd2lkdGg9IjQwIiBoZWlnaHQ9IjQwIj48L3JlY3Q+CiAgICAgICAgICAgICAgICAgICAgICAgIDxwYXRoIGQ9Ik0yMS42MTUzODQ2LDI2LjU0MzIwOTkgQzIxLjYxNTM4NDYsMjYuOTQ3ODc1MSAyMS40NTgzMzQ4LDI3LjI5MTgzNjggMjEuMTQ0MjMwOCwyNy41NzUxMDI5IEMyMC44MzAxMjY4LDI3Ljg1ODM2ODkgMjAuNDQ4NzE5NCwyOCAyMCwyOCBDMTkuNTUxMjgwNiwyOCAxOS4xNjk4NzMyLDI3Ljg1ODM2ODkgMTguODU1NzY5MiwyNy41NzUxMDI5IEMxOC41NDE2NjUyLDI3LjI5MTgzNjggMTguMzg0NjE1NCwyNi45NDc4NzUxIDE4LjM4NDYxNTQsMjYuNTQzMjA5OSBMMTguMzg0NjE1NCwxOS43NDQ4NTYgQzE4LjM4NDYxNTQsMTkuMzQwMTkwNyAxOC41NDE2NjUyLDE4Ljk5NjIyOSAxOC44NTU3NjkyLDE4LjcxMjk2MyBDMTkuMTY5ODczMiwxOC40Mjk2OTY5IDE5LjU1MTI4MDYsMTguMjg4MDY1OCAyMCwxOC4yODgwNjU4IEMyMC40NDg3MTk0LDE4LjI4ODA2NTggMjAuODMwMTI2OCwxOC40Mjk2OTY5IDIxLjE0NDIzMDgsMTguNzEyOTYzIEMyMS40NTgzMzQ4LDE4Ljk5NjIyOSAyMS42MTUzODQ2LDE5LjM0MDE5MDcgMjEuNjE1Mzg0NiwxOS43NDQ4NTYgTDIxLjYxNTM4NDYsMjYuNTQzMjA5OSBaIE0yMCwxNS44MDQyOTgxIEMxOS40NDQ0NDI3LDE1LjgwNDI5ODEgMTguOTcyMjI0LDE1LjYxOTM2ODcgMTguNTgzMzMzMywxNS4yNDk1MDQ2IEMxOC4xOTQ0NDI3LDE0Ljg3OTY0MDYgMTgsMTQuNDMwNTI1NSAxOCwxMy45MDIxNDkxIEMxOCwxMy4zNzM3NzI2IDE4LjE5NDQ0MjcsMTIuOTI0NjU3NSAxOC41ODMzMzMzLDEyLjU1NDc5MzUgQzE4Ljk3MjIyNCwxMi4xODQ5Mjk1IDE5LjQ0NDQ0MjcsMTIgMjAsMTIgQzIwLjU1NTU1NzMsMTIgMjEuMDI3Nzc2LDEyLjE4NDkyOTUgMjEuNDE2NjY2NywxMi41NTQ3OTM1IEMyMS44MDU1NTczLDEyLjkyNDY1NzUgMjIsMTMuMzczNzcyNiAyMiwxMy45MDIxNDkxIEMyMiwxNC40MzA1MjU1IDIxLjgwNTU1NzMsMTQuODc5NjQwNiAyMS40MTY2NjY3LDE1LjI0OTUwNDYgQzIxLjAyNzc3NiwxNS42MTkzNjg3IDIwLjU1NTU1NzMsMTUuODA0Mjk4MSAyMCwxNS44MDQyOTgxIFoiIGlkPSJDb21iaW5lZC1TaGFwZSIgZmlsbD0iI0ZGRkZGRiI+PC9wYXRoPgogICAgICAgICAgICAgICAgICAgIDwvZz4KICAgICAgICAgICAgICAgIDwvZz4KICAgICAgICAgICAgPC9nPgogICAgICAgIDwvZz4KICAgIDwvZz4KPC9zdmc+" alt="" class="el-message__img">\
                <div class="el-message__group"><p>' + opts.content + '</p><div class="el-message__closeBtn el-icon-close"></div></div>'
            }
            window.top.document.querySelector("body").appendChild(el);
            bs.animDelay(el, function () {
                $bs(el, window.top.document).addClass("in");
            })
            setTimeout(function () {
                bs.animDelay(el, function () {
                    $bs(el, window.top.document).removeClass("in");
                    bs.delay(function () {
                        window.top.document.getElementById(id).remove()
                    })
                })

            }, opts.duration);
        }
        var success = function (value, duration) {
            message({
                type: 'success',
                content: value,
                duration: duration
            })
        }
        var error = function (value, duration) {
            message({
                type: 'error',
                content: value,
                duration: duration
            })
        }
        var warning = function (value, duration) {
            message({ type: 'warning', content: value, duration: duration })
        }
        var info = function (value, duration) {
            message({ type: 'info', content: value, duration: duration })
        }
        var modal = function (options) {
            var opts = bs.extend(true, {
                iframe: null,
                key: '',
                el: '',
                title: '标题',
                ajax: {
                    isNeedAjax: false,
                    isAshx: false,
                    url: '',
                    data: {},
                    method: '',
                    success: null
                },
                width: 800,
                height: 500,
                render: null,
                mounted: null,
                onClose: null,
                onSave: null,
                isNeedCancel: true,
                cancelText: '取消',
                isNeedSave: true,
                saveText: '保存',
                isNeedCustOper: false,
                custOperEl: '',
                isNeedDrag: false
            }, options)
            var modalDocument = bs.isUndefinedOrNull(opts.iframe) ? document : (opts.iframe);
            var ele = modalDocument.createElement("div");
            ele.style.display = "block";
            var id = opts.key || bs.newGuid();
            ele.id = id;

            var html = '<div class="bs-modal-backdrop mask-fade mask-in"></div><div class="bs-modal-dialog fade"><div class="bs-modal-content"><div class="bs-modal-header">' +
                opts.title + '<a class="bs-modal_close"></a></div><div class="bs-modal-body" style="overflow:auto">' +
                (opts.ajax.isNeedAjax ? '' : (typeof opts.el == "string" ? opts.el : opts.el.innerHTML)) + '</div>\
                <div class="bs-modal-footer">' + (opts.isNeedCustOper ? opts.custOperEl : ((opts.isNeedCancel ? ('<a class="bs-btn cancel_btn cancel">' + opts.cancelText + '</a>') : '') +
                    (opts.isNeedSave ? ('<a class="bs-btn ensure_btn save">' + opts.saveText + '</a>') : ''))) + '</div></div></div>';
            ele.innerHTML = html;
            ele.querySelector(".bs-modal-dialog").style.zIndex = 5000;
            ele.querySelector(".bs-modal-dialog .bs-modal-body").style.height = opts.height + 'px';
            ele.querySelector(".bs-modal-dialog").style.width = opts.width + 'px';
            // ele.querySelector(".bs-modal-dialog").style.left = (document.body.offsetWidth - opts.width) / 2 + "px";
            // ele.querySelector(".modal-dialog .modal-body").style.height = opts.height - 104 + 'px';
            modalDocument.querySelector("body").appendChild(ele);
            bs.animDelay(ele, function () {
                $bs(ele.querySelector(".bs-modal-dialog")).addClass("in");
            })
            if (opts.isNeedDrag) {
                bs.labelDrag(ele.querySelector(".bs-modal-dialog .bs-modal-header"), ele.querySelector(".bs-modal-dialog"), modalDocument);
            }
            if (!opts.isNeedCustOper) {
                if (opts.isNeedCancel) {
                    ele.querySelector(".cancel").onclick = function () {
                        if (opts.onClose) {
                            opts.onClose(modalDocument.getElementById(id));
                        } else {
                            close();
                        }
                    }
                }
                if (opts.isNeedSave) {
                    ele.querySelector(".save").onclick = function () {
                        if (opts.onSave) {
                            opts.onSave(modalDocument.getElementById(id));
                        }
                    }
                }
            }

            ele.querySelector(".bs-modal_close").onclick = function () {
                if (opts.onClose) {
                    opts.onClose(modalDocument.getElementById(id));
                } else {
                    close();
                }
            }
            if (opts.ajax.isNeedAjax) {
                $.ajax(ajaxSettings({
                    url: opts.ajax.url,
                    contentType: opts.ajax.isAshx ? '' : undefined,
                    type: opts.ajax.method,
                    data: opts.ajax.data,
                    success: function (data) {
                        var temp = opts.ajax.success(data);
                        if (!bs.isEmpty(temp)) {
                            ele.querySelector(".bs-modal-body").innerHTML = temp;
                        } else {
                            bs.error("请求异常");
                            return false;
                        }
                        if (opts.render) {
                            opts.render(data);
                        }
                        if (opts.mounted) {
                            opts.mounted();
                        }
                    }
                }))
            } else {
                if (opts.render) {
                    opts.render();
                }
                if (opts.mounted) {
                    opts.mounted();
                }
            }

            function close() {
                // $bs(ele.querySelector(".bs-modal-backdrop")).removeClass("mask-in");
                // $bs(ele.querySelector(".bs-modal-dialog")).removeClass("in");
                bs.delay(function () {
                    $bs(ele).remove();
                })
            }
        }
        var loading = function (options) {
            var opts = bs.extend({}, {
                content: '内容',
                duration: 0
            }, options)
            var el = window.top.document.getElementById("toast-loading");
            if (bs.isUndefinedOrNull(el)) {
                var el = window.top.document.createElement("div");
                el.id = "toast-loading";
                el.className = "bs-loading-mask is-fullscreen fade";
                el.innerHTML = '<div class="bs-loading-spinner"><svg viewBox="25 25 50 50" class="circular"><circle cx="50" cy="50" r="20" fill="none" class="path"></circle></svg>\
                                <p class="bs-loading-text">' + opts.content + '</p></div>'
                window.top.document.querySelector("body").appendChild(el);
            } else {
                el.querySelector(".bs-loading-text").innerText = opts.content;
            }
            bs.animDelay(el, function () {
                $bs(el, window.top.document).removeClass("none").addClass("in");
            })
            if (opts.duration > 0) {
                bs.delay(function () {
                    $bs(el, window.top.document).removeClass("in");
                    bs.delay(function () {
                        $bs(el, window.top.document).addClass("none");
                    })
                }, opts.duration)
            }
        }
        var unloading = function () {
            $bs("#toast-loading", window.top.document).removeClass("in");
            bs.delay(function () {
                $("#toast-loading", window.top.document).addClass("none");
            })
        }
        var previewView = function (options) {
            var opts = bs.extend(true, {
                iframe: null,
                key: '',
                tpl: '',
                title: '标题',
                width: 800,
                height: 500,
                mounted: null,
                onCancel: null,
                cancelText: '取消',
            }, options);
            var preDocument = bs.isUndefinedOrNull(opts.iframe) ? document : (opts.iframe);
            var el = preDocument.createElement("div");
            el.innerHTML = '<div class="bs-preview fade"><div class="bs-preview-header"><span style="font-size:20px;">' + opts.title +
                '</span><i class="bs-preview-close"></i></div><div class="bs-preview-body">' + (typeof opts.tpl == "string" ? opts.tpl : opts.tpl.innerHTML) +
                '</div><div class="bs-preview-footer"><a class="bs-btn btn_active">关闭</a></div></div><div class="bs-modal-mask fade"></div>';
            preDocument.querySelector("body").appendChild(el);
            bs.animDelay(el, function () {
                $bs(el.querySelector(".bs-preview"), preDocument).addClass("in");
                $bs(el.querySelector(".bs-modal-mask"), preDocument).addClass("in");
            })
            el.querySelector(".bs-preview-close").onclick = function () {
                close();
            }
            el.querySelector(".bs-modal-mask").onclick = function () {
                close();
            }
            el.querySelector(".bs-preview-footer .bs-btn").onclick = function () {
                close();
            }

            function close() {
                $bs(el.querySelector(".bs-preview"), preDocument).removeClass("in");
                $bs(el.querySelector(".bs-modal-mask"), preDocument).removeClass("in");
                bs.delay(function () {
                    $(el, preDocument).remove();
                })
            }
        }
        var xModal = function (options) {
            var opts = bs.extend(true, {
                iframe: null,
                el: '',
                title: '标题',
                width: 800,
                height: 500,
                className: '',
                mounted: null,
                onClose: null,
                onSave: null,
                isNeedCancel: true,
                cancelText: '取消',
                isNeedSave: true,
                saveText: '保存',
            }, options)
            var modalDocument = bs.isUndefinedOrNull(opts.iframe) ? document : (opts.iframe);
            var ele = modalDocument.createElement("div");
            var html = '<div class="bs-modal-backdrop mask-fade mask-in"></div>\
                        <div class="bs-xmodal xmodal-fade {{d.className}}" style="width:{{d.width}}px;margin-left:-{{d.width / 2}}px;margin-top:-{{d.height / 2}}px">\
                        <div class="bs-xmodal-header">{{d.title}}<a class="bs-modal_close"></a></div>\
                        <div class="bs-xmodal-body" style="overflow:auto;height:{{d.height - 104}}px"><div class="bs-loading-mask fade none">\
                        <div class="bs-loading-spinner"><svg viewBox="25 25 50 50" class="circular">\
                        <circle cx="50" cy="50" r="20" fill="none" class="path"></circle></svg><p class="bs-loading-text">正在加载中</p></div></div>'
                + (bs.isEmpty(opts.el) ? '' : bs.getHtml(opts.el)) + '</div>\
                        <div class="bs-xmodal-footer">{{#if(d.isNeedCancel){ }}<a class="bs-btn cancel_btn cancel">{{d.cancelText}}</a>{{#  } }}\
                        {{#if(d.isNeedSave){ }}<a class="bs-btn ensure_btn save">{{d.saveText}}</a>{{#  } }}</div></div>';
            bs.render(html, {
                className: opts.className, height: opts.height, width: opts.width, title: opts.title, isNeedCancel: opts.isNeedCancel,
                cancelText: opts.cancelText, isNeedSave: opts.isNeedSave, saveText: opts.saveText
            }, function (result) {
                ele.innerHTML = result;
            });
            ele.querySelector(".bs-modal-backdrop").style.zIndex = 4999 + modalDocument.querySelectorAll(".bs-modal-backdrop").length;
            ele.querySelector(".bs-xmodal").style.zIndex = 5000 + modalDocument.querySelectorAll(".bs-modal-backdrop").length;
            var loading = {
                open: function () {
                    $bs(ele.querySelector(".bs-loading-mask")).removeClass("none").addClass("in");
                },
                close: function () {
                    bs.delay(function () {
                        $bs(ele.querySelector(".bs-loading-mask")).removeClass("in");
                        bs.delay(function () {
                            $bs(ele.querySelector(".bs-loading-mask")).addClass("none");
                        })
                    }, 50)
                }
            }
            modalDocument.querySelector("body").appendChild(ele);
            bs.animDelay(ele, function () {
                $bs(ele.querySelector(".bs-xmodal")).addClass("in");
            })
            if (opts.mounted) {
                var promise = new Promise(function (resolve, reject) {
                    opts.mounted(ele, resolve, loading);
                });
                promise.then(function (result) {
                    if (result) {
                        close();
                    }
                })
            }
            // if (opts.isNeedDrag) {
            //     bs.labelDrag(ele.querySelector(".bs-modal-dialog .bs-modal-header"), ele.querySelector(".bs-modal-dialog"), modalDocument);
            // }

            if (opts.isNeedCancel) {
                ele.querySelector(".cancel").onclick = function () {
                    if (opts.onClose) {
                        var promise = new Promise(function (resolve, reject) {
                            opts.onClose(resolve);
                        });
                        promise.then(function (result) {
                            if (result) {
                                close();
                            }
                        })
                    } else {
                        close();
                    }
                }
            }
            if (opts.isNeedSave) {
                ele.querySelector(".save").onclick = function () {
                    if (opts.onSave) {
                        var promise = new Promise(function (resolve, reject) {
                            opts.onSave(resolve);
                        });
                        promise.then(function (result) {
                            if (result) {
                                close();
                            }
                        })
                    } else {
                        close();
                    }
                }
            }

            ele.querySelector(".bs-modal_close").onclick = function () {
                if (opts.onClose) {
                    var promise = new Promise(function (resolve, reject) {
                        opts.onClose(resolve);
                    });
                    promise.then(function (result) {
                        if (result) {
                            close();
                        }
                    })
                } else {
                    close();
                }
            }

            function close() {
                // $bs(ele.querySelector(".bs-modal-backdrop")).removeClass("mask-in");
                // $bs(ele.querySelector(".bs-xmodal")).removeClass("in");
                bs.delay(function () {
                    $bs(ele).remove();
                })
            }
        }
        return {
            message: message,
            alert: alert,
            confirm: confirm,
            success: success,
            error: error,
            warning: warning,
            info: info,
            modal: modal,
            preview: previewView,
            loading: loading,
            unloading: unloading,
            xModal: xModal
        }
    })()

    var preview = (function () {
        function showPosition(el, value, opts, type) {
            if (type == 1) {
                el.innerHTML = value;
            } else {
                el.innerText = value;
            }
            el.style.display = "block";
            var scrollTop = document.body.scrollTop,
                scrollLeft = document.body.scrollLeft,
                x = event.clientX + scrollLeft,
                y = event.clientY;
            if (x - scrollLeft > document.body.clientWidth / 2) {
                x = x - el.offsetWidth - 2 * opts.offsetX;
            }
            if ((y + el.offsetHeight + opts.offsetY) > document.body.clientHeight) {
                y = document.body.clientHeight - el.offsetHeight - opts.offsetY;
            }
            el.style.left = (x + opts.offsetX) + 'px';
            el.style.top = (y + opts.offsetY) + 'px';
        }

        function infoPreview(el, options) {
            var opts = bs.extend({}, {
                width: 200,
                height: 150,
                offsetX: 10,
                offsetY: 20
            }, options)
            var createDom = document.querySelector("#bs-infoPreview");
            if (bs.isUndefinedOrNull(createDom)) {
                createDom = document.createElement("div");
                createDom.id = "bs-infoPreview";
                createDom.style.position = "fixed";
                createDom.style.display = "none";
                createDom.style.zIndex = 6000;
                document.body.appendChild(createDom);
            }
            if (!bs.isUndefinedOrNull(el)) {
                bs.makeArray(el).forEach(function (v, k) {
                    v.onmousemove = (function (dom) {
                        return function (e) {
                            show(v, e);
                        }
                    })(v)
                    v.onmouseout = (function (dom) {
                        return function (e) {
                            none();
                        }
                    })(v)
                })
            }

            function show(v) {
                if (!bs.isEmpty(v.innerText)) {
                    showPosition(createDom, v.innerText, opts);
                }
            }

            function none() {
                document.querySelector("#bs-infoPreview").style.display = "none";
            }
        }

        function imagePreview(el, options) {
            var opts = bs.extend({}, {
                width: 200,
                height: 150,
                offsetX: 10,
                offsetY: 20
            }, options)
            var createDom = document.querySelector("#bs-imagePreview");
            if (bs.isUndefinedOrNull(createDom)) {
                createDom = document.createElement("div");
                createDom.id = "bs-imagePreview";
                createDom.style.position = "fixed";
                createDom.style.display = "none";
                createDom.style.zIndex = 6000;
                document.body.appendChild(createDom);
            }
            if (!bs.isUndefinedOrNull(el)) {
                bs.makeArray(el).forEach(function (v, k) {
                    v.onmousemove = (function (dom) {
                        return function (e) {
                            show(v, e);
                        }
                    })(v)
                    v.onmouseout = (function (dom) {
                        return function (e) {
                            none();
                        }
                    })(v)
                })
            }

            function show(v) {
                showPosition(createDom, '<img style="width:' + opts.width + 'px;" src="' + v.src + '" />', opts, 1);
            }

            function none() {
                document.querySelector("#bs-imagePreview").style.display = "none";
            }
        }
        return {
            infoPreview: infoPreview,
            imagePreview: imagePreview
        }
    })();
    bs.extend(bs, toast, template, preview);
})(window);

(function () {
    function tabPanel(options) {
        var opts = bs.extend(true, {
            el: null,
            isGenId: true,
            panelId: '',
            mounted: null,
            tabList: [],
            tabWidth: 10,
            isNeedValue: false,
            current: '',
        }, options);
        if (bs.isEmpty(opts.el)) {
            bs.error("参数配置错误");
            return false;
        }
        if (opts.tabList.length <= 0) {
            bs.error("参数配置错误");
            return false;
        }
        var rootNode = document.createElement("div");

        function init() {
            if (opts.isGenId) {
                opts.tabList.forEach(function (v, k) {
                    v.tabId = 'key' + bs.newGuid();
                })
            }
            var tabsNode = document.createElement("div");
            tabsNode.className = "bs-tabs__header";
            tabsNode.innerHTML = renderTabs();

            var contentNode = document.createElement("div");
            contentNode.className = "bs-tabs__content";
            if (bs.isEmpty(opts.panelId)) {
                contentNode.innerHTML = renderContent();
            } else {
                contentNode.innerHTML = document.getElementById(opts.panelId).innerHTML;
            }
            rootNode.appendChild(tabsNode);
            rootNode.appendChild(contentNode);
            registEvent();
            document.getElementById(opts.el).appendChild(rootNode);
            renderTpl(0);
        }

        function renderTabs(list) {
            list = list || opts.tabList;
            var index = list.findIndex(function (o) {
                return o.selected;
            });
            var html = '<div class="bs-tabs__nav-wrap"><div class="bs-tabs__nav"><div class="bs-tabs__active-bar" style="transform: translateX(' + 100 * index + '%);width:' + opts.tabWidth + '%"></div>';
            list.forEach(function (v, k) {
                if (v.selected) {
                    opts.current = v.tabId;
                }
                html += '<div id=' + v.tabId + ' class="bs-tabs__item ' + (v.selected ? 'is-active' : '') +
                    '" style="width: ' + opts.tabWidth + '%" tagpanel=' + k + '><span>' +
                    v.tabName + '</span>' + (opts.isNeedValue ? (v.value ? ('<b>' + v.value + '</b>') : '') : '') + '</div>';
            })
            html += '</div></div>';
            return html;
        }

        function renderUpdateTabs(list) {
            var els = rootNode.querySelectorAll(".bs-tabs__header .bs-tabs__item");
            list.forEach(function (v, k) {
                els[k].innerHTML = '<span>' + v.tabName + '</span>' + (opts.isNeedValue ? (v.value ? ('<b>' + v.value + '</b>') : '') : '');
            })
        }

        function renderContent() {
            var html = ''
            opts.tabList.forEach(function (v, k) {
                html += ' <div id=' + v.tabId + ' render="false" class="bs-tab-pane bs-fade  ' + (v.selected ? '' : 'none') + '" panel=' + k + '></div>';
            })
            return html;
        }

        function renderTpl(flag) {
            var currentData = opts.tabList.find(function (o) {
                return o.tabId == opts.current;
            });
            if (bs.isEmpty(opts.panelId)) {
                var bindNode = rootNode.querySelector(".bs-tabs__content .bs-tab-pane#" + opts.current);
                if (bindNode.getAttribute("render") == 'false') {
                    if (!bs.isUndefinedOrNull(currentData.tabTplId)) {
                        bindNode.innerHTML = document.getElementById(currentData.tabTplId).innerHTML;
                    }
                }
                $bs(bindNode).siblings().forEach(function (v, k) {
                    $bs(v).addClass("none").removeClass("bs-in");
                })
                $bs(bindNode).removeClass("none");
                if (bindNode.getAttribute("render") == 'false') {
                    if (bs.isFunction(currentData.mounted)) {
                        currentData.mounted();
                    }
                    bindNode.setAttribute("render", true);
                }
                if (bs.isFunction(currentData.callBack)) {
                    currentData.callBack({
                        key: currentData.tabId,
                        name: currentData.tabName,
                        value: currentData.value,
                        data: currentData.tabdata
                    }, bindNode);
                }
                bs.delay(function () {
                    $(bindNode).addClass("bs-in");
                }, 100)
            } else {
                if (flag == 0 && bs.isFunction(opts.mounted)) {
                    opts.mounted(rootNode.querySelector(".bs-tabs__content"));
                }
                if (bs.isFunction(currentData.callBack)) {
                    currentData.callBack({
                        key: currentData.tabId,
                        name: currentData.tabName,
                        value: currentData.value,
                        data: currentData.tabdata
                    }, bindNode);
                }
            }
        }

        function registEvent() {
            $bs(rootNode).findAll(".bs-tabs__item").on("click", function () {
                if ($bs(this).hasClass("is-active")) {
                    return false;
                }
                $bs(this).siblings().forEach(function (v, k) {
                    $bs(v).removeClass("is-active");
                })
                $bs(this).addClass("is-active");
                opts.current = this.id;
                var index = Number(this.getAttribute("tagpanel"));
                rootNode.querySelector(".bs-tabs__active-bar").style.transform = "translateX(" + 100 * index + "%)";
                renderTpl();
            })
        }
        this.changeValue = function (id, value) {
            var index = opts.tabList.findIndex(function (o) {
                return o.tabId == id;
            });
            var currentData = opts.tabList[index];
            if (!bs.isUndefinedOrNull(currentData)) {
                rootNode.querySelector(".bs-tabs__item#" + currentData.tabId).innerHTML = '<span>' + currentData.tabName + '</span>' + (opts.isNeedValue ? (value ? ('<b>' + value + '</b>') : '') : '');
                opts.tabList[index].value = value;
            }
        }
        this.changeTabsValue = function (tabs) {
            if (opts.tabList.length != tabs.length) {
                bs.error("参数异常");
                return false;
            } else {
                opts.tabList.forEach(function (v, k) {
                    v.value = tabs[k].value;
                })
            }
            renderUpdateTabs(tabs)
        }
        this.getValue = function (id) {
            var currentData = opts.tabList.find(function (o) {
                return o.tabId == id;
            });
            return currentData || {};
        }
        init();
    }

    function tabs(options) {
        var opts = bs.cloneDeep(true, {}, {
            iframe: null,
            el: '',
            key: 'key',
            label: 'label',
            type: 'type',
            title: 'title',
            isNeedOper: false,
            removeCallback: null,
            recoveCallback: null,
            list: [],
        }, options);
        var rootNode = document.createElement("div");
        rootNode.className = "bs-tags";

        function renderNode(v, cnode) {
            cnode = cnode || document.createElement("div")
            cnode.className = "tag-item " + (bs.isUndefinedOrNull(v[opts.type]) ? 'default' : v[opts.type]);
            cnode.setAttribute("key", v[opts.key]);
            cnode.id = "el" + v[opts.key];
            cnode.title = v[opts.title];
            cnode.innerHTML = '<span>' + v[opts.label] + '</span>' + (opts.isNeedOper ?
                '<i class="iconfont ' + (v[opts.type] == 'disabled' ? 'icon-icon_selected' : 'icon-icon_close') + '"></i>' : '');
            return cnode;
        }

        function init() {
            rootNode.innerHTML = "";
            opts.list.forEach(function (v, k) {
                rootNode.appendChild(renderNode(v));
            })
            registEvent();
            var modalDocument = bs.isUndefinedOrNull(opts.iframe) ? document : (opts.iframe);
            modalDocument.querySelector(opts.el).appendChild(rootNode);
        }

        function registEvent() {
            if (opts.isNeedOper) {
                $bs(rootNode.querySelectorAll(".tag-item .iconfont")).on("click", function () {
                    if ($bs(this.parentNode).hasClass("disabled")) {
                        if (opts.recoveCallback) {
                            opts.recoveCallback(this.parentNode.getAttribute("key"));
                        }
                    } else {
                        if (opts.removeCallback) {
                            opts.removeCallback(this.parentNode.getAttribute("key"));
                        }
                    }
                })
            }
        }
        init();
        this.removeTab = function (key) {
            opts.list.remove(function (o) {
                return o[opts.key] == key;
            });
            $bs(rootNode.querySelector('#el' + key)).remove();
        }
        this.disabledTab = function (key) {
            var index = opts.list.findIndex(function (o) {
                return o[opts.key] == key;
            })
            if (index == -1) {
                bs.warning("该节点不存在!");
                return false;
            } else {
                opts.list[index][opts.type] = 'disabled';
                renderNode(opts.list[index], rootNode.querySelector('#el' + key));
                registEvent();
            }
        }
        this.undisabledTab = function (key, type) {
            var index = opts.list.findIndex(function (o) {
                return o[opts.key] == key;
            })
            if (index == -1) {
                bs.warning("该节点不存在!");
                return false;
            } else {
                opts.list[index][opts.type] = type;
                renderNode(opts.list[index], rootNode.querySelector('#el' + key));
                registEvent();
            }
        }
        this.updateTab = function (obj) {
            var index = opts.list.findIndex(function (o) {
                return o[opts.key] == obj[opts.key];
            })
            if (index == -1) {
                bs.warning("该节点不存在!");
                return false;
            } else {
                opts.list[index] = obj;
                var node = rootNode.querySelector('#el' + obj.key);
                renderNode(obj, node);
            }
        }

        this.updateAll = function (list) {
            if (bs.isArray(list)) {
                opts.list.length = 0;
                opts.list = list;
                init();
            }
        }
    }

    function lazyLoad(options) {
        var defaults = {
            el: null,
            delay: 0,
            loadImage: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAIwAAACMCAIAAAAhotZpAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAA3ZpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuNi1jMTM4IDc5LjE1OTgyNCwgMjAxNi8wOS8xNC0wMTowOTowMSAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDoxZTFhOWI4NS00ZjZmLWY3NDgtYjg3Ny01ZDc1NDUyZmFkYzAiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6NTMxQUFCRkJGQjQ4MTFFNzg3RUQ4NEU0NjAzOTRFQUIiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6NTMxQUFCRkFGQjQ4MTFFNzg3RUQ4NEU0NjAzOTRFQUIiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENDIDIwMTcgKFdpbmRvd3MpIj4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6MWQyYzRmYjMtNDc5Ny01ZDRhLTg4NzctMjNlNDc5OGVkM2U3IiBzdFJlZjpkb2N1bWVudElEPSJ4bXAuZGlkOjFlMWE5Yjg1LTRmNmYtZjc0OC1iODc3LTVkNzU0NTJmYWRjMCIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/Pn6zkWgAAAM9SURBVHja7N1dT9pQHIDxtryVAoVSaVHEF9TpzS73/b/DrpZsyeJcMswmjpdKdUr3T1xYDKLxwnpO+zwhBIgG5NdzeiioZpIkBqmdxVMAEoEEEoFEIIFEIBFIIBFIIBFIBBJIBBKBBBKBRCCBRCCBRCARSCARSAQSSAQSSAQSgQQSgUQggUQgEUggEUh5qajOQ0mS5OzH6OL37PZukcLdHR/0XKcM0ss6G47OLyap3d04upZzLZwUmu4uLmcp36M4Ta5uQHpB6cxyOjqxcNDACSQNnEDSwAkkDZxA0sApv0iLRaKLU36R4jjWZTwVc4s0nUZybtu2ZZmKH4/IL1KSGJNJJKd1XzA0jA/v95nuCCSQCCTSf+FgWVYvbLWbtVKxcBXfDH+OR+MIJKWEzJNBt1at3F+VCwc7QXk4EiqmO1UKfXcptGw79MqlIkiq5Lm11RtN02w2qkx3qlQoPL5tFaxntjnZgfVCz7HL34ajWRQzkl6xaH79otv/bY/Fwslgs9Nu1JzKQb/DdPe6yQJh9X9zTaN4un5wyGR4tBvaldL9Vb32XloiyZr78+n5n9u75S2T2fzL6fkT37LX8+tOhX1Sqo2n84+fzuo1WyaxeXwjp6dXgxteg4XDG7RIEhlAz36ZW6/2N32W4OpWKZdkjWCavE5SdqVuWUe7gcyHvJh942TBthW0Hl2t7fc7Vbucga1NbyQZJcf7XXl9ejLolh6OGLnRc51sTAkaI8meZtDv3I8h2feI1vJIhNesyfDKzLytMdJW4DXr/w/Wycz2bi+0LFMuDLY7RobSdQnecp3VsVJ37MOdwK6UH/0AEEhpL6zXjZVmwzEyl37TnYySw91g3YHwTKbfj7rX23AysbDOLFLgu36rbuQszZC2Q8/IX5ohXU6iHCJptrr7+v2XnBhJBBKBBBKBRCCBRCCBRCARSCARSA8qKvZmqzqPR6HnxffUejev01blM/4KvVXR77aNxEjtT04/PYb8Vr2nzBuM5upvYxH7JAIJJAKJQAKJQAKJQCKQQCKQCCSQCCQCCSQCCSQCiUACiUAikEAikEAikAgkkAgkAgkkAolAAolAAolAIpBAojfqrwADAM0tzBGGvZKuAAAAAElFTkSuQmCC',
            errorImage: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAIwAAACMCAIAAAAhotZpAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAA3ZpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuNi1jMTM4IDc5LjE1OTgyNCwgMjAxNi8wOS8xNC0wMTowOTowMSAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDoxZTFhOWI4NS00ZjZmLWY3NDgtYjg3Ny01ZDc1NDUyZmFkYzAiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6RTdDNUU0MDdGQjQ4MTFFN0FDRTc4MkZGNzZBNjk1MTEiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6RTdDNUU0MDZGQjQ4MTFFN0FDRTc4MkZGNzZBNjk1MTEiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENDIDIwMTcgKFdpbmRvd3MpIj4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6MWQyYzRmYjMtNDc5Ny01ZDRhLTg4NzctMjNlNDc5OGVkM2U3IiBzdFJlZjpkb2N1bWVudElEPSJ4bXAuZGlkOjFlMWE5Yjg1LTRmNmYtZjc0OC1iODc3LTVkNzU0NTJmYWRjMCIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/PlpvGpYAAAT5SURBVHja7JxrU+JIFIbJDTAwCSFAYBB0RudWW1u1X/b//4f5tLNrjSU4ijqARO4C2YNxp9iQWMYRprt9n6IsKyaW+ni639NpInmelwBsI+NPAEkAkiAJQBKAJEgCkAQgCZIAJEESgCQASZAEIAlAEiQBSAKQBEkAkiAJQBKAJEgCkAQgCZIAJEESgCQASZAEIAlAEiQBSAKQBEkAkl4KKjs/iud5zfNO+7o/my/iXvvHp7qmKn8ft3r90SMv+XBQNfQkKikezVbnou0+wRAxGE7o424l//hLeoOJO5xCUjza3f6Trx2Ol39uPZ0sWFnxPDEk6Wk1dF9Jo4n/SdWxZFkSzJMgwcEf7oikpjq2Geta9j0JIul2NqeX/3mlZKqqIpIncSL4j2JSZLlaysW9nGVPAkn6b1oiivlXqaQmjCdxJPkBz0eSpFrZesI3YdOTgMOdj2Vmsnr6gfMXC48XT+JIWs0OPrXKQ8U0Ho95qSeh1u4CxUSVRPUUdfLNzWA4HHFRT6pQkkaTnKGvHtl1rGt3GPrEZjrmugN6RX23ViLx5+9vUEnPnR1Gwf/9dEqjpMf77yULVknrB6lnos4JktjNDssBXVUqJROS2M0OPo5tJjWOZ19ef3RZlqtOLm9mNFWhNrZ11ev0BqHZ4e5kqepYx6dXqKRtGpI+vi2XC8v6kCQps5M6qJfKRTM0O/gULArkSUjaHo5tkJjAQUrb5Cw0O9yfEOe+LYa7n8UyQlpUKinz1Q59jLpqOp1B0vZQlPABIJNOFfLht89vBuOTszZNYDQ50bjXaHX6gzGGu233Q0TeyoRW0mQ6O2pcktqPbyvU22b01EGtiDlps1CWW1/pWSy80KaVjh+dXMzni3d7Tjp1f5OJr0TOpSTK3P+cXKz2rbP5Imr/ydfTKzp/v2pn9RTmpK3Suxl9/quZzaRVVbEMPR+x2n12ed3tDSgNFiyOV/A4XnFYeJ7bH6mKHGWo6w6+XXSN7E6tYnO9jML3shAJqEd0P6Px9GvzKpXUKCNIUgKSfg0k4LBeCo1zs9mcJi0pIb3bK8Xd3gVJz9kqvd93QhsmCn4UuCl2v6kVd7hdCuJeEhUP1dCPPB2gcdam1pWaVmttpRWStke9YtNsFPXV8XRmmZnX8fdHIoJvj1oln46/MxKV9Mw0zjurWyED6OlkrDdWQNJGSGpqShNwDBBHElXJ4V4paiEckphgv1rQhQjWwkoq2YadyyZeGJxJ2nWsxMuDM0nd6F3BAsNZRjo+/U6v9eOUI347fL3+xrHb2fzzl2bUrnxU0laZzxdfjpe3XwPHNVWhaQzDHStMprdHjcv1909UitgLzhJuf9Q4bwcHdEV2CgYkMcRl26VX4GC5aKo8N78C9u1UTO7/HwNFwx0NepDEEDQt3d30uw10wRq3t2jFXAFbD3uyLPF7h0nYZcr1sHf3BA4VkpgOe5LEazEJvuAfCHt2Lhu1MwKSWAl7VExVDpdoxZcUCHt5M8Pd7SiGJG2u3wyEvUcWEzv9L0OSbGuDd/NWw17O0B/zDgt2ntLBkKRaOe/Yxub+f1fD3sPFtFzusw12Zi8p9Lk7AHMSgCRIApAEIAmSACRBEoAkAEmQBCAJQBIkAUgCkARJAJIgCUASgCRIApAEIAmSACRBEoAkAEmQBCAJQBIkAUgCkARJAJIgCUASgCRIAr+CfwUYAGwl0VP+1rrGAAAAAElFTkSuQmCC',
        }
        var opts = bs.cloneDeep({}, defaults, options)
        function core() {
            var nodes = [];
            if (bs.isUndefinedOrNull(opts.el)) {
                nodes = document.querySelectorAll("img[v-lazy]")
            } else {
                if (typeof opts.el == "object") {
                    nodes = opts.el.querySelectorAll("img[v-lazy]");
                } else {
                    nodes = document.querySelector(opts.el).querySelectorAll("img[v-lazy]");
                }
            }
            bs.makeArray(nodes).forEach(function (v, k) {
                v.src = opts.loadImage;
                var image = new Image();
                var lazyUrl = v.getAttribute("v-lazy");
                v.removeAttribute("v-lazy")
                image.src = lazyUrl;
                image.onload = function (e) {
                    v.src = e.target.src;
                }
                image.error = function (e) {
                    v.src = opts.errorImage;
                }
            })
        }
        function init() {
            bs.delay(core, opts.delay)
        }
        init();
    }
    bs.extend({
        lazyLoad: lazyLoad,
        tabPanel: tabPanel,
        tabs: tabs,
    });
})()

// ajax 请求参数
var ajaxSettings = function (opt) {
    var url = opt.url;
    var href = location.href;
    // 判断是否跨域请求
    var requestType = 'json';
    if (url.indexOf(location.host) > -1)
        requestType = 'jsonp';
    requestType = opt.dataType || requestType;
    // 是否异步请求
    var async = (opt.async === undefined ? true : opt.async);
    var submit = (opt.submit === undefined ? "submit" : opt.submit);
    var contentType = (opt.contentType === undefined ? "application/json; charset=utf-8" : opt.contentType);

    var ajaxObj = {
        url: url,
        async: async,
        type: opt.type || 'get',
        dataType: requestType,
        contentType: contentType,
        cache: false,
        data: opt.data,
        beforeSend: function () {
            // 禁用按钮防止重复提交
            $("#" + submit).attr({
                disabled: "disabled"
            });
        },
        success: function (data, textStatus, xhr) {
            /*
             *如果dataType是json，怎判断返回数据是否为json格式，如果不是进行转换
             * 成功数据通用格式
             *   {
             *       "code": 200,
             *       "data": [], 
             *       "success": true // 成功
             *   }
             *   失败返回的数据
             *   {
             *       "code": 200, 
             *       "info": 'error', 
             *       "success": false // 失败
             *   }
             */
            //data = data.d;
            if ((requestType === 'json' || requestType === "jsonp") && typeof (data) === "string") {
                data = JSON.parse(data);
            }
            opt.success(data);
            // if (data.success) {
            //     opt.success(data);
            // }
            // if (opt.error) {
            //     opt.error(data);
            // }
        },
        complete: function () {
            $("#" + submit).removeAttr("disabled");
            $("#loading,.mask").hide();
        },
        error: function (xhr, status, handler) {
            if (!bs.isUndefinedOrNull(opt.error)) {
                opt.error();
            } else {
                var evt = window.top.event || top.arguments.callee.caller.arguments[0];
                var src = evt.srcElement || evt.target;
                if (src.outerHTML != "<i class=\"sysmenu\"></i>") {
                    window.top.bs.error("请求异常");
                }
                return false;
            }
        }
    }
    return ajaxObj;
};


(function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined' ? module.exports = factory() :
        typeof define === 'function' && define.amd ? define(factory) :
            (global.ES6Promise = factory());
}(this, (function () {
    'use strict';

    function objectOrFunction(x) {
        var type = typeof x;
        return x !== null && (type === 'object' || type === 'function');
    }

    function isFunction(x) {
        return typeof x === 'function';
    }

    var _isArray = undefined;
    if (Array.isArray) {
        _isArray = Array.isArray;
    } else {
        _isArray = function (x) {
            return Object.prototype.toString.call(x) === '[object Array]';
        };
    }

    var isArray = _isArray;

    var len = 0;
    var vertxNext = undefined;
    var customSchedulerFn = undefined;

    var asap = function asap(callback, arg) {
        queue[len] = callback;
        queue[len + 1] = arg;
        len += 2;
        if (len === 2) {
            // If len is 2, that means that we need to schedule an async flush.
            // If additional callbacks are queued before the queue is flushed, they
            // will be processed by this flush that we are scheduling.
            if (customSchedulerFn) {
                customSchedulerFn(flush);
            } else {
                scheduleFlush();
            }
        }
    };

    function setScheduler(scheduleFn) {
        customSchedulerFn = scheduleFn;
    }

    function setAsap(asapFn) {
        asap = asapFn;
    }

    var browserWindow = typeof window !== 'undefined' ? window : undefined;
    var browserGlobal = browserWindow || {};
    var BrowserMutationObserver = browserGlobal.MutationObserver || browserGlobal.WebKitMutationObserver;
    var isNode = typeof self === 'undefined' && typeof process !== 'undefined' && ({}).toString.call(process) === '[object process]';

    // test for web worker but not in IE10
    var isWorker = typeof Uint8ClampedArray !== 'undefined' && typeof importScripts !== 'undefined' && typeof MessageChannel !== 'undefined';

    // node
    function useNextTick() {
        // node version 0.10.x displays a deprecation warning when nextTick is used recursively
        // see https://github.com/cujojs/when/issues/410 for details
        return function () {
            return process.nextTick(flush);
        };
    }

    // vertx
    function useVertxTimer() {
        if (typeof vertxNext !== 'undefined') {
            return function () {
                vertxNext(flush);
            };
        }

        return useSetTimeout();
    }

    function useMutationObserver() {
        var iterations = 0;
        var observer = new BrowserMutationObserver(flush);
        var node = document.createTextNode('');
        observer.observe(node, {
            characterData: true
        });

        return function () {
            node.data = iterations = ++iterations % 2;
        };
    }

    // web worker
    function useMessageChannel() {
        var channel = new MessageChannel();
        channel.port1.onmessage = flush;
        return function () {
            return channel.port2.postMessage(0);
        };
    }

    function useSetTimeout() {
        // Store setTimeout reference so es6-promise will be unaffected by
        // other code modifying setTimeout (like sinon.useFakeTimers())
        var globalSetTimeout = setTimeout;
        return function () {
            return globalSetTimeout(flush, 1);
        };
    }

    var queue = new Array(1000);

    function flush() {
        for (var i = 0; i < len; i += 2) {
            var callback = queue[i];
            var arg = queue[i + 1];

            callback(arg);

            queue[i] = undefined;
            queue[i + 1] = undefined;
        }

        len = 0;
    }

    function attemptVertx() {
        try {
            var r = require;
            var vertx = r('vertx');
            vertxNext = vertx.runOnLoop || vertx.runOnContext;
            return useVertxTimer();
        } catch (e) {
            return useSetTimeout();
        }
    }

    var scheduleFlush = undefined;
    // Decide what async method to use to triggering processing of queued callbacks:
    if (isNode) {
        scheduleFlush = useNextTick();
    } else if (BrowserMutationObserver) {
        scheduleFlush = useMutationObserver();
    } else if (isWorker) {
        scheduleFlush = useMessageChannel();
    } else if (browserWindow === undefined && typeof require === 'function') {
        scheduleFlush = attemptVertx();
    } else {
        scheduleFlush = useSetTimeout();
    }

    function then(onFulfillment, onRejection) {
        var _arguments = arguments;

        var parent = this;

        var child = new this.constructor(noop);

        if (child[PROMISE_ID] === undefined) {
            makePromise(child);
        }

        var _state = parent._state;

        if (_state) {
            (function () {
                var callback = _arguments[_state - 1];
                asap(function () {
                    return invokeCallback(_state, child, callback, parent._result);
                });
            })();
        } else {
            subscribe(parent, child, onFulfillment, onRejection);
        }

        return child;
    }

    /**
      `Promise.resolve` returns a promise that will become resolved with the
      passed `value`. It is shorthand for the following:
  
      ```javascript
      let promise = new Promise(function(resolve, reject){
        resolve(1);
      });
  
      promise.then(function(value){
        // value === 1
      });
      ```
  
      Instead of writing the above, your code now simply becomes the following:
  
      ```javascript
      let promise = Promise.resolve(1);
  
      promise.then(function(value){
        // value === 1
      });
      ```
  
      @method resolve
      @static
      @param {Any} value value that the returned promise will be resolved with
      Useful for tooling.
      @return {Promise} a promise that will become fulfilled with the given
      `value`
    */
    function resolve$1(object) {
        /*jshint validthis:true */
        var Constructor = this;

        if (object && typeof object === 'object' && object.constructor === Constructor) {
            return object;
        }

        var promise = new Constructor(noop);
        resolve(promise, object);
        return promise;
    }

    var PROMISE_ID = Math.random().toString(36).substring(16);

    function noop() { }

    var PENDING = void 0;
    var FULFILLED = 1;
    var REJECTED = 2;

    var GET_THEN_ERROR = new ErrorObject();

    function selfFulfillment() {
        return new TypeError("You cannot resolve a promise with itself");
    }

    function cannotReturnOwn() {
        return new TypeError('A promises callback cannot return that same promise.');
    }

    function getThen(promise) {
        try {
            return promise.then;
        } catch (error) {
            GET_THEN_ERROR.error = error;
            return GET_THEN_ERROR;
        }
    }

    function tryThen(then$$1, value, fulfillmentHandler, rejectionHandler) {
        try {
            then$$1.call(value, fulfillmentHandler, rejectionHandler);
        } catch (e) {
            return e;
        }
    }

    function handleForeignThenable(promise, thenable, then$$1) {
        asap(function (promise) {
            var sealed = false;
            var error = tryThen(then$$1, thenable, function (value) {
                if (sealed) {
                    return;
                }
                sealed = true;
                if (thenable !== value) {
                    resolve(promise, value);
                } else {
                    fulfill(promise, value);
                }
            }, function (reason) {
                if (sealed) {
                    return;
                }
                sealed = true;

                reject(promise, reason);
            }, 'Settle: ' + (promise._label || ' unknown promise'));

            if (!sealed && error) {
                sealed = true;
                reject(promise, error);
            }
        }, promise);
    }

    function handleOwnThenable(promise, thenable) {
        if (thenable._state === FULFILLED) {
            fulfill(promise, thenable._result);
        } else if (thenable._state === REJECTED) {
            reject(promise, thenable._result);
        } else {
            subscribe(thenable, undefined, function (value) {
                return resolve(promise, value);
            }, function (reason) {
                return reject(promise, reason);
            });
        }
    }

    function handleMaybeThenable(promise, maybeThenable, then$$1) {
        if (maybeThenable.constructor === promise.constructor && then$$1 === then && maybeThenable.constructor.resolve === resolve$1) {
            handleOwnThenable(promise, maybeThenable);
        } else {
            if (then$$1 === GET_THEN_ERROR) {
                reject(promise, GET_THEN_ERROR.error);
                GET_THEN_ERROR.error = null;
            } else if (then$$1 === undefined) {
                fulfill(promise, maybeThenable);
            } else if (isFunction(then$$1)) {
                handleForeignThenable(promise, maybeThenable, then$$1);
            } else {
                fulfill(promise, maybeThenable);
            }
        }
    }

    function resolve(promise, value) {
        if (promise === value) {
            reject(promise, selfFulfillment());
        } else if (objectOrFunction(value)) {
            handleMaybeThenable(promise, value, getThen(value));
        } else {
            fulfill(promise, value);
        }
    }

    function publishRejection(promise) {
        if (promise._onerror) {
            promise._onerror(promise._result);
        }

        publish(promise);
    }

    function fulfill(promise, value) {
        if (promise._state !== PENDING) {
            return;
        }

        promise._result = value;
        promise._state = FULFILLED;

        if (promise._subscribers.length !== 0) {
            asap(publish, promise);
        }
    }

    function reject(promise, reason) {
        if (promise._state !== PENDING) {
            return;
        }
        promise._state = REJECTED;
        promise._result = reason;

        asap(publishRejection, promise);
    }

    function subscribe(parent, child, onFulfillment, onRejection) {
        var _subscribers = parent._subscribers;
        var length = _subscribers.length;

        parent._onerror = null;

        _subscribers[length] = child;
        _subscribers[length + FULFILLED] = onFulfillment;
        _subscribers[length + REJECTED] = onRejection;

        if (length === 0 && parent._state) {
            asap(publish, parent);
        }
    }

    function publish(promise) {
        var subscribers = promise._subscribers;
        var settled = promise._state;

        if (subscribers.length === 0) {
            return;
        }

        var child = undefined,
            callback = undefined,
            detail = promise._result;

        for (var i = 0; i < subscribers.length; i += 3) {
            child = subscribers[i];
            callback = subscribers[i + settled];

            if (child) {
                invokeCallback(settled, child, callback, detail);
            } else {
                callback(detail);
            }
        }

        promise._subscribers.length = 0;
    }

    function ErrorObject() {
        this.error = null;
    }

    var TRY_CATCH_ERROR = new ErrorObject();

    function tryCatch(callback, detail) {
        try {
            return callback(detail);
        } catch (e) {
            TRY_CATCH_ERROR.error = e;
            return TRY_CATCH_ERROR;
        }
    }

    function invokeCallback(settled, promise, callback, detail) {
        var hasCallback = isFunction(callback),
            value = undefined,
            error = undefined,
            succeeded = undefined,
            failed = undefined;

        if (hasCallback) {
            value = tryCatch(callback, detail);

            if (value === TRY_CATCH_ERROR) {
                failed = true;
                error = value.error;
                value.error = null;
            } else {
                succeeded = true;
            }

            if (promise === value) {
                reject(promise, cannotReturnOwn());
                return;
            }
        } else {
            value = detail;
            succeeded = true;
        }

        if (promise._state !== PENDING) {
            // noop
        } else if (hasCallback && succeeded) {
            resolve(promise, value);
        } else if (failed) {
            reject(promise, error);
        } else if (settled === FULFILLED) {
            fulfill(promise, value);
        } else if (settled === REJECTED) {
            reject(promise, value);
        }
    }

    function initializePromise(promise, resolver) {
        try {
            resolver(function resolvePromise(value) {
                resolve(promise, value);
            }, function rejectPromise(reason) {
                reject(promise, reason);
            });
        } catch (e) {
            reject(promise, e);
        }
    }

    var id = 0;

    function nextId() {
        return id++;
    }

    function makePromise(promise) {
        promise[PROMISE_ID] = id++;
        promise._state = undefined;
        promise._result = undefined;
        promise._subscribers = [];
    }

    function Enumerator$1(Constructor, input) {
        this._instanceConstructor = Constructor;
        this.promise = new Constructor(noop);

        if (!this.promise[PROMISE_ID]) {
            makePromise(this.promise);
        }

        if (isArray(input)) {
            this.length = input.length;
            this._remaining = input.length;

            this._result = new Array(this.length);

            if (this.length === 0) {
                fulfill(this.promise, this._result);
            } else {
                this.length = this.length || 0;
                this._enumerate(input);
                if (this._remaining === 0) {
                    fulfill(this.promise, this._result);
                }
            }
        } else {
            reject(this.promise, validationError());
        }
    }

    function validationError() {
        return new Error('Array Methods must be provided an Array');
    }

    Enumerator$1.prototype._enumerate = function (input) {
        for (var i = 0; this._state === PENDING && i < input.length; i++) {
            this._eachEntry(input[i], i);
        }
    };

    Enumerator$1.prototype._eachEntry = function (entry, i) {
        var c = this._instanceConstructor;
        var resolve$$1 = c.resolve;

        if (resolve$$1 === resolve$1) {
            var _then = getThen(entry);

            if (_then === then && entry._state !== PENDING) {
                this._settledAt(entry._state, i, entry._result);
            } else if (typeof _then !== 'function') {
                this._remaining--;
                this._result[i] = entry;
            } else if (c === Promise$2) {
                var promise = new c(noop);
                handleMaybeThenable(promise, entry, _then);
                this._willSettleAt(promise, i);
            } else {
                this._willSettleAt(new c(function (resolve$$1) {
                    return resolve$$1(entry);
                }), i);
            }
        } else {
            this._willSettleAt(resolve$$1(entry), i);
        }
    };

    Enumerator$1.prototype._settledAt = function (state, i, value) {
        var promise = this.promise;

        if (promise._state === PENDING) {
            this._remaining--;

            if (state === REJECTED) {
                reject(promise, value);
            } else {
                this._result[i] = value;
            }
        }

        if (this._remaining === 0) {
            fulfill(promise, this._result);
        }
    };

    Enumerator$1.prototype._willSettleAt = function (promise, i) {
        var enumerator = this;

        subscribe(promise, undefined, function (value) {
            return enumerator._settledAt(FULFILLED, i, value);
        }, function (reason) {
            return enumerator._settledAt(REJECTED, i, reason);
        });
    };

    /**
      `Promise.all` accepts an array of promises, and returns a new promise which
      is fulfilled with an array of fulfillment values for the passed promises, or
      rejected with the reason of the first passed promise to be rejected. It casts all
      elements of the passed iterable to promises as it runs this algorithm.
  
      Example:
  
      ```javascript
      let promise1 = resolve(1);
      let promise2 = resolve(2);
      let promise3 = resolve(3);
      let promises = [ promise1, promise2, promise3 ];
  
      Promise.all(promises).then(function(array){
        // The array here would be [ 1, 2, 3 ];
      });
      ```
  
      If any of the `promises` given to `all` are rejected, the first promise
      that is rejected will be given as an argument to the returned promises's
      rejection handler. For example:
  
      Example:
  
      ```javascript
      let promise1 = resolve(1);
      let promise2 = reject(new Error("2"));
      let promise3 = reject(new Error("3"));
      let promises = [ promise1, promise2, promise3 ];
  
      Promise.all(promises).then(function(array){
        // Code here never runs because there are rejected promises!
      }, function(error) {
        // error.message === "2"
      });
      ```
  
      @method all
      @static
      @param {Array} entries array of promises
      @param {String} label optional string for labeling the promise.
      Useful for tooling.
      @return {Promise} promise that is fulfilled when all `promises` have been
      fulfilled, or rejected if any of them become rejected.
      @static
    */
    function all$1(entries) {
        return new Enumerator$1(this, entries).promise;
    }

    /**
      `Promise.race` returns a new promise which is settled in the same way as the
      first passed promise to settle.
  
      Example:
  
      ```javascript
      let promise1 = new Promise(function(resolve, reject){
        setTimeout(function(){
          resolve('promise 1');
        }, 200);
      });
  
      let promise2 = new Promise(function(resolve, reject){
        setTimeout(function(){
          resolve('promise 2');
        }, 100);
      });
  
      Promise.race([promise1, promise2]).then(function(result){
        // result === 'promise 2' because it was resolved before promise1
        // was resolved.
      });
      ```
  
      `Promise.race` is deterministic in that only the state of the first
      settled promise matters. For example, even if other promises given to the
      `promises` array argument are resolved, but the first settled promise has
      become rejected before the other promises became fulfilled, the returned
      promise will become rejected:
  
      ```javascript
      let promise1 = new Promise(function(resolve, reject){
        setTimeout(function(){
          resolve('promise 1');
        }, 200);
      });
  
      let promise2 = new Promise(function(resolve, reject){
        setTimeout(function(){
          reject(new Error('promise 2'));
        }, 100);
      });
  
      Promise.race([promise1, promise2]).then(function(result){
        // Code here never runs
      }, function(reason){
        // reason.message === 'promise 2' because promise 2 became rejected before
        // promise 1 became fulfilled
      });
      ```
  
      An example real-world use case is implementing timeouts:
  
      ```javascript
      Promise.race([ajax('foo.json'), timeout(5000)])
      ```
  
      @method race
      @static
      @param {Array} promises array of promises to observe
      Useful for tooling.
      @return {Promise} a promise which settles in the same way as the first passed
      promise to settle.
    */
    function race$1(entries) {
        /*jshint validthis:true */
        var Constructor = this;

        if (!isArray(entries)) {
            return new Constructor(function (_, reject) {
                return reject(new TypeError('You must pass an array to race.'));
            });
        } else {
            return new Constructor(function (resolve, reject) {
                var length = entries.length;
                for (var i = 0; i < length; i++) {
                    Constructor.resolve(entries[i]).then(resolve, reject);
                }
            });
        }
    }

    /**
      `Promise.reject` returns a promise rejected with the passed `reason`.
      It is shorthand for the following:
  
      ```javascript
      let promise = new Promise(function(resolve, reject){
        reject(new Error('WHOOPS'));
      });
  
      promise.then(function(value){
        // Code here doesn't run because the promise is rejected!
      }, function(reason){
        // reason.message === 'WHOOPS'
      });
      ```
  
      Instead of writing the above, your code now simply becomes the following:
  
      ```javascript
      let promise = Promise.reject(new Error('WHOOPS'));
  
      promise.then(function(value){
        // Code here doesn't run because the promise is rejected!
      }, function(reason){
        // reason.message === 'WHOOPS'
      });
      ```
  
      @method reject
      @static
      @param {Any} reason value that the returned promise will be rejected with.
      Useful for tooling.
      @return {Promise} a promise rejected with the given `reason`.
    */
    function reject$1(reason) {
        /*jshint validthis:true */
        var Constructor = this;
        var promise = new Constructor(noop);
        reject(promise, reason);
        return promise;
    }

    function needsResolver() {
        throw new TypeError('You must pass a resolver function as the first argument to the promise constructor');
    }

    function needsNew() {
        throw new TypeError("Failed to construct 'Promise': Please use the 'new' operator, this object constructor cannot be called as a function.");
    }

    /**
      Promise objects represent the eventual result of an asynchronous operation. The
      primary way of interacting with a promise is through its `then` method, which
      registers callbacks to receive either a promise's eventual value or the reason
      why the promise cannot be fulfilled.
  
      Terminology
      -----------
  
      - `promise` is an object or function with a `then` method whose behavior conforms to this specification.
      - `thenable` is an object or function that defines a `then` method.
      - `value` is any legal JavaScript value (including undefined, a thenable, or a promise).
      - `exception` is a value that is thrown using the throw statement.
      - `reason` is a value that indicates why a promise was rejected.
      - `settled` the final resting state of a promise, fulfilled or rejected.
  
      A promise can be in one of three states: pending, fulfilled, or rejected.
  
      Promises that are fulfilled have a fulfillment value and are in the fulfilled
      state.  Promises that are rejected have a rejection reason and are in the
      rejected state.  A fulfillment value is never a thenable.
  
      Promises can also be said to *resolve* a value.  If this value is also a
      promise, then the original promise's settled state will match the value's
      settled state.  So a promise that *resolves* a promise that rejects will
      itself reject, and a promise that *resolves* a promise that fulfills will
      itself fulfill.
  
  
      Basic Usage:
      ------------
  
      ```js
      let promise = new Promise(function(resolve, reject) {
        // on success
        resolve(value);
  
        // on failure
        reject(reason);
      });
  
      promise.then(function(value) {
        // on fulfillment
      }, function(reason) {
        // on rejection
      });
      ```
  
      Advanced Usage:
      ---------------
  
      Promises shine when abstracting away asynchronous interactions such as
      `XMLHttpRequest`s.
  
      ```js
      function getJSON(url) {
        return new Promise(function(resolve, reject){
          let xhr = new XMLHttpRequest();
  
          xhr.open('GET', url);
          xhr.onreadystatechange = handler;
          xhr.responseType = 'json';
          xhr.setRequestHeader('Accept', 'application/json');
          xhr.send();
  
          function handler() {
            if (this.readyState === this.DONE) {
              if (this.status === 200) {
                resolve(this.response);
              } else {
                reject(new Error('getJSON: `' + url + '` failed with status: [' + this.status + ']'));
              }
            }
          };
        });
      }
  
      getJSON('/posts.json').then(function(json) {
        // on fulfillment
      }, function(reason) {
        // on rejection
      });
      ```
  
      Unlike callbacks, promises are great composable primitives.
  
      ```js
      Promise.all([
        getJSON('/posts'),
        getJSON('/comments')
      ]).then(function(values){
        values[0] // => postsJSON
        values[1] // => commentsJSON
  
        return values;
      });
      ```
  
      @class Promise
      @param {function} resolver
      Useful for tooling.
      @constructor
    */
    function Promise$2(resolver) {
        this[PROMISE_ID] = nextId();
        this._result = this._state = undefined;
        this._subscribers = [];

        if (noop !== resolver) {
            typeof resolver !== 'function' && needsResolver();
            this instanceof Promise$2 ? initializePromise(this, resolver) : needsNew();
        }
    }

    Promise$2.all = all$1;
    Promise$2.race = race$1;
    Promise$2.resolve = resolve$1;
    Promise$2.reject = reject$1;
    Promise$2._setScheduler = setScheduler;
    Promise$2._setAsap = setAsap;
    Promise$2._asap = asap;

    Promise$2.prototype = {
        constructor: Promise$2,

        /**
        The primary way of interacting with a promise is through its `then` method,
        which registers callbacks to receive either a promise's eventual value or the
        reason why the promise cannot be fulfilled.
    
        ```js
        findUser().then(function(user){
          // user is available
        }, function(reason){
          // user is unavailable, and you are given the reason why
        });
        ```
    
        Chaining
        --------
    
        The return value of `then` is itself a promise.  This second, 'downstream'
        promise is resolved with the return value of the first promise's fulfillment
        or rejection handler, or rejected if the handler throws an exception.
    
        ```js
        findUser().then(function (user) {
          return user.name;
        }, function (reason) {
          return 'default name';
        }).then(function (userName) {
          // If `findUser` fulfilled, `userName` will be the user's name, otherwise it
          // will be `'default name'`
        });
    
        findUser().then(function (user) {
          throw new Error('Found user, but still unhappy');
        }, function (reason) {
          throw new Error('`findUser` rejected and we're unhappy');
        }).then(function (value) {
          // never reached
        }, function (reason) {
          // if `findUser` fulfilled, `reason` will be 'Found user, but still unhappy'.
          // If `findUser` rejected, `reason` will be '`findUser` rejected and we're unhappy'.
        });
        ```
        If the downstream promise does not specify a rejection handler, rejection reasons will be propagated further downstream.
    
        ```js
        findUser().then(function (user) {
          throw new PedagogicalException('Upstream error');
        }).then(function (value) {
          // never reached
        }).then(function (value) {
          // never reached
        }, function (reason) {
          // The `PedgagocialException` is propagated all the way down to here
        });
        ```
    
        Assimilation
        ------------
    
        Sometimes the value you want to propagate to a downstream promise can only be
        retrieved asynchronously. This can be achieved by returning a promise in the
        fulfillment or rejection handler. The downstream promise will then be pending
        until the returned promise is settled. This is called *assimilation*.
    
        ```js
        findUser().then(function (user) {
          return findCommentsByAuthor(user);
        }).then(function (comments) {
          // The user's comments are now available
        });
        ```
    
        If the assimliated promise rejects, then the downstream promise will also reject.
    
        ```js
        findUser().then(function (user) {
          return findCommentsByAuthor(user);
        }).then(function (comments) {
          // If `findCommentsByAuthor` fulfills, we'll have the value here
        }, function (reason) {
          // If `findCommentsByAuthor` rejects, we'll have the reason here
        });
        ```
    
        Simple Example
        --------------
    
        Synchronous Example
    
        ```javascript
        let result;
    
        try {
          result = findResult();
          // success
        } catch(reason) {
          // failure
        }
        ```
    
        Errback Example
    
        ```js
        findResult(function(result, err){
          if (err) {
            // failure
          } else {
            // success
          }
        });
        ```
    
        Promise Example;
    
        ```javascript
        findResult().then(function(result){
          // success
        }, function(reason){
          // failure
        });
        ```
    
        Advanced Example
        --------------
    
        Synchronous Example
    
        ```javascript
        let author, books;
    
        try {
          author = findAuthor();
          books  = findBooksByAuthor(author);
          // success
        } catch(reason) {
          // failure
        }
        ```
    
        Errback Example
    
        ```js
    
        function foundBooks(books) {
    
        }
    
        function failure(reason) {
    
        }
    
        findAuthor(function(author, err){
          if (err) {
            failure(err);
            // failure
          } else {
            try {
              findBoooksByAuthor(author, function(books, err) {
                if (err) {
                  failure(err);
                } else {
                  try {
                    foundBooks(books);
                  } catch(reason) {
                    failure(reason);
                  }
                }
              });
            } catch(error) {
              failure(err);
            }
            // success
          }
        });
        ```
    
        Promise Example;
    
        ```javascript
        findAuthor().
          then(findBooksByAuthor).
          then(function(books){
            // found books
        }).catch(function(reason){
          // something went wrong
        });
        ```
    
        @method then
        @param {Function} onFulfilled
        @param {Function} onRejected
        Useful for tooling.
        @return {Promise}
      */
        then: then,

        /**
        `catch` is simply sugar for `then(undefined, onRejection)` which makes it the same
        as the catch block of a try/catch statement.
    
        ```js
        function findAuthor(){
          throw new Error('couldn't find that author');
        }
    
        // synchronous
        try {
          findAuthor();
        } catch(reason) {
          // something went wrong
        }
    
        // async with promises
        findAuthor().catch(function(reason){
          // something went wrong
        });
        ```
    
        @method catch
        @param {Function} onRejection
        Useful for tooling.
        @return {Promise}
      */
        'catch': function _catch(onRejection) {
            return this.then(null, onRejection);
        }
    };

    /*global self*/
    function polyfill$1() {
        var local = undefined;

        if (typeof global !== 'undefined') {
            local = global;
        } else if (typeof self !== 'undefined') {
            local = self;
        } else {
            try {
                local = Function('return this')();
            } catch (e) {
                throw new Error('polyfill failed because global object is unavailable in this environment');
            }
        }

        var P = local.Promise;

        if (P) {
            var promiseToString = null;
            try {
                promiseToString = Object.prototype.toString.call(P.resolve());
            } catch (e) {
                // silently ignored
            }

            if (promiseToString === '[object Promise]' && !P.cast) {
                return;
            }
        }

        local.Promise = Promise$2;
    }

    // Strange compat..
    Promise$2.polyfill = polyfill$1;
    Promise$2.Promise = Promise$2;

    return Promise$2;

})));