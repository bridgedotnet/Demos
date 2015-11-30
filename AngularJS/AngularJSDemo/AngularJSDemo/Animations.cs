using Bridge;
using Bridge.AngularJS;
using Bridge.AngularJS.Resource;
using Bridge.AngularJS.Route;
using Bridge.Html5;
using Bridge.jQuery2;
using System;

namespace PhoneCat
{
    public partial class App
    {
        public static void InitAnimations()
        {
            var animations = Angular.Module("phonecatAnimations",
                new string[] { "ngAnimate" });

            animations.Animation(".phone", () =>
            {
                Func<jQuery, string, Action, Action<bool>> animateUp =
                    (jQuery element, string className, Action done) =>
                    {
                        if (className != "active")
                        {
                            return null;
                        }

                        element.Css(
                        new
                        {
                            Position = Position.Absolute,
                            Top = 500,
                            Left = 0,
                            Display = Display.Block
                        });

                        element.Animate(new
                        {
                            Top = 0
                        }, 400, "swing", done);

                        return (cancel) =>
                        {
                            if (cancel)
                            {
                                element.Stop();
                            };
                        };
                    };

                Func<jQuery, string, Action, Action<bool>> animateDown =
                    (jQuery element, string className, Action done) =>
                    {
                        if (className != "active")
                        {
                            return null;
                        }

                        element.Css(
                        new
                        {
                            Position = Position.Absolute,
                            Top = 0,
                            Left = 0
                        });

                        element.Animate(new
                        {
                            Top = -500
                        }, 400, "swing", done);

                        return (cancel) =>
                        {
                            if (cancel)
                            {
                                element.Stop();
                            };
                        };
                    };

                return new Bridge.AngularJS.jQuery.Animation
                {
                    AddClass = animateUp,
                    RemoveClass = animateDown
                };
            });
        }
    }
}