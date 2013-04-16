using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Text;
using System.Web;
using Coin.SDK.Model;

namespace Coin.SDK
{
    public class Html
    {
        public static IHtmlString Script(string scriptSourceUrl = null)
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items["CocoinScriptWritten"] != null)
                    return null;
                HttpContext.Current.Items["CocoinScriptWritten"] = true;
            }

            scriptSourceUrl = scriptSourceUrl ?? ((ConfigurationManager.AppSettings[Constants.CocoinSiteUrl] ?? "https://www.cocoin.com") + "/sdk/payment.js");

            var t = new StringBuilder();
            t.AppendLine("<span id='cocoin_root'></span>");
            t.AppendLine("<script type='text/javascript'>");
            t.AppendLine("/* DON'T EDIT BELOW THIS LINE */");
            t.AppendLine("(function () {");
            t.AppendLine("var dsq = document.createElement('script'); dsq.type = 'text/javascript'; dsq.async = true;");
            t.AppendLine(string.Format("dsq.src = '{0}';", scriptSourceUrl));
            t.AppendLine(
                "(document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);");
            t.AppendLine("})();");
            t.AppendLine("</script>");

            return new HtmlString(t.ToString());
        }

        public static IHtmlString PaymentButton(IBlankOrder order)
        {

            string[] messages;
            if (!order.Validate(out messages))
                throw new InvalidOperationException("Order did not validate when building payment button: " + string.Join("\n", messages));

            var data = new Dictionary<string, string>();
            order.FormatProperties((key, value) => data.Add(key, string.Format(CultureInfo.InvariantCulture, "{0}: '{1}'", key, value)));

            var signed = order as ISignedOrder;
            if (signed != null)
                data.Add("signature", string.Format(CultureInfo.InvariantCulture, "{0}: '{1}'", "signature", signed.Signature));

            var tab = "<span class='cocoin_pay' data-cocoin-args=\"{" + string.Join(", ", data.Values) + "}\"></span>";

            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items["CocoinScriptWritten"] == null)
                    tab = Script() + "\n" + tab;
            }

            var html = new HtmlString(tab);
            return html;
        }


    }
}