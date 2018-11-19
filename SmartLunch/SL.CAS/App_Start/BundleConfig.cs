using System.Web.Optimization;

namespace SL.CAS
{
    /// <summary>
    /// Content bundles
    /// </summary>
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.Bundles.IgnoreList.Clear();
            BundleTable.Bundles.IgnoreList.Ignore(".min.js", OptimizationMode.Always);
            BundleTable.Bundles.IgnoreList.Ignore(".min.css", OptimizationMode.Always);

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/plugins/layer/layer.js",
                        "~/Scripts/plugins/laydate/laydate.js",
                        "~/Scripts/plugins/slimscroll/jquery.slimscroll.js",
                        "~/Scripts/plugins/bootstrap-table/bootstrap-table.js",
                        "~/Scripts/plugins/bootstrap-table/locale/bootstrap-table-zh-CN.min.js",
                        "~/Scripts/common.js",
                        "~/Scripts/formatterfunction.js",
                        "~/Scripts/demo/bootstrap-select.js"
                    ));

            bundles.Add(new ScriptBundle("~/Scripts/plugins").Include(
                        "~/Scripts/plugins/validform/js/Validform_v5.3.2.js",
                        //"~/Scripts/plugins/prettyfile/bootstrap-prettyfile.js",
                        "~/Scripts/plugins/filestyle/bootstrap-filestyle.js",
                        "~/Scripts/plugins/webuploader/webuploader.js"
                    ));

            //基本样式
            bundles.Add(new StyleBundle("~/Content/themes").Include(
                        "~/Content/themes/css/bootstrap.css",
                        "~/Content/themes/css/font-awesome.css",
                        "~/Content/themes/css/animate.css",
                        "~/Content/themes/css/plugins/bootstrap-table/bootstrap-table.css",
                        "~/Content/themes/css/style.css",
                        "~/Content/themes/css/demo/bootstrap-select.css"));
            //插件样式
            bundles.Add(new StyleBundle("~/Content/themes/css").Include(
                        "~/Content/themes/css/plugins/webuploader/webuploader.css"
                    ));

            BundleTable.EnableOptimizations = false;
        }
    }
}
