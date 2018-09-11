﻿(function ($) {
    // 增加Array扩展
    if (!$.isFunction(Array.prototype.filter)) {
        Array.prototype.filter = function (callback, thisObject) {
            if ($.isFunction(callback)) {
                var res = new Array();
                for (var i = 0; i < this.length; i++) {
                    callback.call(thisObject, this[i], i, this) && res.push(this[i]);
                }
                return res;
            }
        };
    }

    // 增加String扩展
    if (!$.isFunction(String.prototype.trim)) {
        String.prototype.trim = function () {
            if (this === null) return "";
            var trimLeft = /^\s+/, trimRight = /\s+$/;
            return this.replace(trimLeft, "").replace(trimRight, "");
        };
    }

    // 扩展Date
    if (!$.isFunction(Date.prototype.format)) {
        Date.prototype.format = function (format) {
            var o = {
                "M+": this.getMonth() + 1,
                "d+": this.getDate(),
                "h+": this.getHours() % 12 === 0 ? 12 : this.getHours() % 12,
                "H+": this.getHours(),
                "m+": this.getMinutes(),
                "s+": this.getSeconds(),
                "q+": Math.floor((this.getMonth() + 3) / 3),
                "S": this.getMilliseconds()
            };
            var week = {
                0: "日",
                1: "一",
                2: "二",
                3: "三",
                4: "四",
                5: "五",
                6: "六"
            };

            if (/(y+)/.test(format))
                format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));

            if (/(E+)/.test(format))
                format = format.replace(RegExp.$1, (RegExp.$1.length > 1 ? RegExp.$1.length > 2 ? "星期" : "周" : "") + week[this.getDay()]);

            for (var k in o)
                if (new RegExp("(" + k + ")").test(format))
                    format = format.replace(RegExp.$1, RegExp.$1.length === 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
            return format;
        };
    }

    // 扩展format
    $.extend({
        "format": function (source, params) {
            if (params === undefined) {
                return source;
            }
            if (arguments.length > 2 && params.constructor !== Array) {
                params = $.makeArray(arguments).slice(1);
            }
            if (params.constructor !== Array) {
                params = [params];
            }
            $.each(params, function (i, n) {
                source = source.replace(new RegExp("\\{" + i + "\\}", "g"), function () {
                    return n;
                });
            });
            return source;
        }
    });

    // enhance window.console.log
    if (!window.console) {
        window.console = {
            log: function () {

            }
        };
    }
    window.console = window.console || {};
    console.log || (console.log = opera.postError);

    // client
    jQuery.browser = {
        versions: function () {
            var u = navigator.userAgent;
            return {         //移动终端浏览器版本信息
                trident: u.indexOf('Trident') > -1, //IE内核
                presto: u.indexOf('Presto') > -1, //opera内核
                webKit: u.indexOf('AppleWebKit') > -1, //苹果、谷歌内核
                gecko: u.indexOf('Gecko') > -1 && u.indexOf('KHTML') === -1, //火狐内核
                mobile: !!u.match(/AppleWebKit.*Mobile.*/), //是否为移动终端
                ios: !!u.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/), //ios终端
                android: u.indexOf('Android') > -1 || u.indexOf('Linux') > -1, //android终端或uc浏览器
                iPhone: u.indexOf('iPhone') > -1, //是否为iPhone或者QQHD浏览器
                iPod: u.indexOf('iPod') > -1, //是否为iPod或者QQHD浏览器
                iPad: u.indexOf('iPad') > -1, //是否iPad
                webApp: u.indexOf('Safari') === -1 //是否web应该程序，没有头部与底部
            };
        }(),
        language: (navigator.browserLanguage || navigator.language).toLowerCase()
    };

    $.extend({
        bc: function (options) {
            options = $.extend({
                id: "",
                url: "",
                data: {},
                htmlTemplate: '<div class="form-group col-md-3 col-sm-4 col-6"><div class="form-check"><label class="form-check-label" title="{3}" data-toggle="tooltip"><input type="checkbox" class="form-check-input" value="{0}" {2}/><span>{1}</span></label></div></div>',
                title: "",
                modal: false,
                loading: false,
                loadingTimeout: 10000,
                callback: false,
                cors: false,
                contentType: 'application/json',
                dataType: 'json',
                method: 'get'
            }, options);

            if (!options.url || options.url === "") {
                toastr.error('参数错误: 未设置请求地址Url');
                return;
            }

            if (options.loading && options.modal) {
                $(options.modal).find('.close').addClass('hidden');
                $(options.modal).modal('show');
                setTimeout(function () {
                    $(options.modal).find('.close').removeClass('hidden');
                }, options.loadingTimeout);
            }


            var data = options.method === 'get' ? options.data : JSON.stringify(options.data);
            var ajaxSettings = {
                url: $.formatUrl(options.url) + options.id,
                data: data,
                method: options.method,
                contentType: options.contentType,
                dataType: options.dataType,
                crossDomain: false,
                success: function (result) {
                    success(result);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    success(false);
                }
            };
            if (options.cors) $.extend(ajaxSettings, {
                xhrFields: { withCredentials: true },
                crossDomain: true
            });
            $.ajax(ajaxSettings);

            function success(result) {
                if ($.isFunction(options.callback)) {
                    options.callback.call(options, result);
                }
                if (options.modal && (result || options.loading)) {
                    $(options.modal).modal('hide');
                }
                if (options.title) toastr[result ? 'success' : 'error'](options.title + (result ? "成功" : "失败"));
            }
        },
        lgbSwal: function (options) {
            if ($.isFunction(swal)) {
                swal($.extend({ html: true, showConfirmButton: false, showCancelButton: false, timer: 1000, title: '未设置', type: "success" }, options));
            }
        },
        getUID: function (prefix) {
            if (!prefix) prefix = 'lgb';
            do prefix += ~~(Math.random() * 1000000);
            while (document.getElementById(prefix));
            return prefix;
        },
        fullScreenStatus: function fullScreenStatus(value) {
            if (value !== undefined) window.fullscreen = value;
            return document.fullscreen ||
                document.mozFullScreen ||
                document.webkitIsFullScreen || window.fullscreen ||
                false;
        },
        formatter: function (key) {
            if (!this[key]) {
                this[key] = {};
                var that = this;
                $.each($('#' + key).children(), function (index, element) {
                    that[key][$(element).attr('value')] = $(element).text();
                });
            }
            return this;
        },
        formatUrl: function (url) {
            if (!url) return url;
            if (url.substr(0, 4) === "http") return url;
            var base = $('#pathBase').attr('href');
            return base + url;
        }
    });

    window.lgbSwal = $.lgbSwal;

    $.fn.extend({
        fixCollapse: function () {
            var $root = this;
            var $collapse = $root.find('a[data-toggle="collapse"]:visible');
            $collapse.each(function () {
                var $this = $(this);
                if ($this.attr('href') !== '#') return;
                var $target = $this.parent().next();
                var tId = $.getUID('collapse');
                $target.attr('id', tId);
                $this.attr('href', '#' + tId);
            });
            return this;
        },
        adjustDialog: function () {
            var $modal_dialog = this;
            var m_top = Math.max(0, ($(window).height() - $modal_dialog.height()) / 2);
            $modal_dialog.css({ 'margin': m_top + 'px auto' });
            return this;
        },
        autoCenter: function (options) {
            options = $.extend({ top: 0 }, options);
            var that = this;
            var defaultVal = parseFloat(that.css('marginTop').replace('px', ''));
            var getHeight = function () {
                return Math.max(defaultVal, ($(window).height() - options.top - that.outerHeight()) / 2 + $(document).scrollTop());
            };
            $(window).resize(function () {
                that.css({ marginTop: getHeight() });
            });
            that.css({ marginTop: getHeight(), transition: "all .5s linear" });
            return this;
        },
        footer: function (options) {
            if ($(window).width() >= 768) { return this.addClass('fixed'); }
            var op = $.extend({ header: "header", content: ".main-content" }, options);
            return $(op.header).outerHeight() + $(op.content).outerHeight() + this.outerHeight() > $(window).height() ? this.removeClass('fixed') : this.addClass('fixed');
        },
        lgbTable: function (options) {
            var bsa = new DataTable($.extend(options.dataBinder, { url: options.url }));

            var settings = $.extend({
                url: options.url,
                checkbox: true,
                edit: true,
                editTitle: "编辑",
                editField: "Id",
                queryButton: false
            }, options.smartTable);
            if (settings.edit) settings.columns.unshift({
                title: settings.editTitle,
                field: settings.editField,
                events: bsa.idEvents(),
                formatter: function (value, row, index) {
                    return "<a class='edit' title='" + value + "' href='javascript:void(0)'>" + this.title + "</a>";
                }
            });
            if (settings.checkbox) settings.columns.unshift({ checkbox: true });
            return this.smartTable(settings);
        },
        smartTable: function (options) {
            var settings = $.extend({
                toolbar: '#toolbar',                //工具按钮用哪个容器
                striped: true,                      //是否显示行间隔色
                cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
                pagination: true,                   //是否显示分页（*）
                sortOrder: "asc",                   //排序方式
                sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
                pageNumber: 1,                      //初始化加载第一页，默认第一页
                pageSize: 20,                       //每页的记录行数（*）
                pageList: [20, 40, 80, 120],        //可供选择的每页的行数（*）
                showColumns: true,                  //是否显示所有的列
                showRefresh: true,                  //是否显示刷新按钮
                showToggle: true,                   //是否显示详细视图和列表视图的切换按钮
                cardView: $(window).width() < 768,                    //是否显示详细视图
                footer: '.site-footer',
                queryButton: '#btn_query',
                onLoadSuccess: function () {
                    $(settings.footer).footer();
                }
            }, options);
            settings.url = $.formatUrl(settings.url);
            this.bootstrapTable(settings);
            $(settings.toolbar).removeClass('d-none').find('.toolbar').on('click', 'a', function (e) {
                e.preventDefault();
                $('#' + $(this).attr('id').replace('tb_', 'btn_')).trigger("click");
            }).insertBefore(this.parents('.bootstrap-table').find('.fixed-table-toolbar > .bs-bars'));
            if (settings.queryButton) {
                $(settings.queryButton).on('click', this, function (e) {
                    e.data.bootstrapTable('refresh');
                });
            }
            return this;
        },
        msgHandler: function (options) {
            var settings = {
                url: 'api/WS/',
                interval: 10000,
                sendMessage: '',
                timerHandler: null,
                onopen: function (e) { },
                onmessage: function (e) { },
                onclose: function (e) { },
                errorHandler: function (e) { if (toastr && $.isFunction(toastr.error)) toastr.error("连接服务器失败！", "系统错误"); },
                loop: function () {
                    var that = this;
                    var uri = window.location.protocol + "//" + window.location.host + $.formatUrl(settings.url);
                    $.bc({
                        url: uri,
                        id: this.sendMessage,
                        method: "post",
                        callback: function (result) {
                            if (!result) {
                                that.errorHandler.call(that.target);
                                return;
                            }
                            that.onmessage.call(that.target, { data: JSON.stringify(result) });
                        }
                    });

                    if (this.timerHandler !== null) clearTimeout(this.timerHandler);
                    this.timerHandler = setTimeout(function () { that.loop(); }, that.interval);
                }
            };
            $.extend(settings, options, { target: this });
            settings.loop();
            return this;
        },
        socketHandler: function (options) {
            // WebSocket消息处理方法
            var settings = {
                url: 'WS',
                interval: 30000,
                sendMessage: 'keepalive',
                timerHandler: null,
                onopen: function (e) { },
                onerror: function (e) { },
                errorHandler: function (e) { if (window.toastr && $.isFunction(window.toastr.error)) toastr.error("连接服务器失败！", "系统错误"); },
                onmessage: function (e) { },
                onclose: function (e) { },
                loop: function (socket) {
                    var that = this;
                    if (socket.readyState === 1) {
                        socket.send(this.sendMessage);
                        if (this.timerHandler !== null) clearTimeout(this.timerHandler);
                        this.timerHandler = setTimeout(function () { that.loop(socket); }, that.interval);
                    }
                    else {
                        this.errorHandler();
                    }
                }
            };
            $.extend(settings, options, { target: this });
            var uri = "ws://" + window.location.host + $.formatUrl(settings.url);
            var socket = new WebSocket(uri);
            socket.onopen = function (e) { settings.onopen.call(settings.target, e); settings.loop(socket); };
            socket.onerror = function (e) {
                settings.onerror.call(settings.target, e);
                settings.target.msgHandler(options);
            };
            socket.onmessage = function (e) { settings.onmessage.call(settings.target, e); };
            socket.onclose = function (e) { settings.onclose.call(settings.target, e); };
            return this;
        }
    });

    //extend dropdown method
    $.extend($.fn.dropdown.Constructor.prototype, {
        val: function () {
            var $element = $(this._element);
            var $op = $(this._menu).find('[data-val="' + $element.val() + '"]:first');
            $element.text($op.text());
        },
        select: function () {
            var $element = $(this._element);
            $(this._menu).on('click', 'a', function (event) {
                event.preventDefault();
                var $op = $(this);
                $element.text($op.text()).val($op.attr('data-val'));
            });
        }
    });

    // Roles
    Role = {
        url: 'api/Roles/',
        title: "分配角色"
    };

    // Users
    User = {
        url: 'api/Users/',
        title: "分配用户"
    };

    // Groups
    Group = {
        url: 'api/Groups/',
        title: "分配部门"
    };

    // Menus
    Menu = {
        url: 'api/Menus/',
        iconView: 'Admin/IconView',
        title: "分配菜单"
    };

    // Exceptions
    Exceptions = {
        url: 'api/Exceptions/',
        title: "程序异常日志"
    };

    // Dicts
    Dicts = {
        url: 'api/Dicts/'
    };

    // Profiles
    Profiles = {
        url: 'api/Profiles/'
    };

    // Settings
    Settings = {
        url: 'api/Settings/'
    };

    // Messages
    Messages = {
        url: 'api/Messages/'
    };

    // Tasks
    Tasks = {
        url: 'api/Tasks/'
    };

    // Notifications
    Notifications = {
        url: 'api/Notifications/'
    };
})(jQuery);