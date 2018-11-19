var viewModel = function () {
    this.form = {
        username: ko.observable(""),
        password: ko.observable(""),
        remember: ko.observable(false)
    };
    this.isDis = ko.observable(false);
    this.fullName = ko.observable("");
    this.loginEnable = ko.observable(true);
    this.loginText = ko.observable("登 录");
    this.loginClick = function () {
        var self = this;
        if (bs.isEmpty(self.form.username())) {
            self.isDis(true);
            self.fullName("请输入用户名");
            return;
        }
        if (bs.isEmpty(self.form.password())) {
            self.isDis(true);
            self.fullName("请输入密码");
            return;
        }
        $.ajax({
            type: "POST",
            url: url,
            data: ko.toJSON(self.form),
            contentType: "application/json",
            beforeSend: function () {
                self.loginEnable(false);
                self.loginText("正在登录...");
            },
            success: function (d) {
                if (d.status == "success") {
                    self.loginText("登录成功");
                    setTimeout(function () {
                        self.loginText("正在跳转...");
                        location.href = loginUrl;
                    }, 500);
                }
                else {
                    self.loginEnable(true);
                    self.loginText("登录");
                    self.isDis(true);
                    self.fullName(d.message);
                }
            },
            error: function () {
                self.loginEnable(true);
                self.loginText("登录");
                self.isDis(true);
                self.fullName("登录失败，请稍后重试！");
            }
        });
    };
    this.keyUp = function (o, e) {
        var self = this;
        if (e.keyCode == 13) {
            $("#btnLogin").focus();
            self.loginClick();
        }
    }
};