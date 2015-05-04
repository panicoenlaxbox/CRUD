using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.App_Start
{
    // http://panicoenlaxbox.blogspot.com.es/2014/02/convenciones-de-nombrado-para.html
    // http://timgthomas.com/2013/10/feature-folders-in-asp-net-mvc/
    public class FeatureViewLocationRazorViewEngine : RazorViewEngine
    {
        public FeatureViewLocationRazorViewEngine()
        {
            var featureFolderViewLocationFormats = new[]
            {
                "~/Features/{1}/{0}.cshtml",
                "~/Features/{1}/Views/{0}.cshtml",
                "~/Features/Shared/{0}.cshtml"
            };

            ViewLocationFormats = featureFolderViewLocationFormats;
            MasterLocationFormats = featureFolderViewLocationFormats;
            PartialViewLocationFormats = featureFolderViewLocationFormats;
        }
    }
}