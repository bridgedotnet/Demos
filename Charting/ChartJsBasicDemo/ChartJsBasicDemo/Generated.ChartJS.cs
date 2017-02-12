using Bridge.Html5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge;

namespace ChartJS 
{
    using Context = Union<string, HTMLElement, CanvasRenderingContext2D>;

    [External]
    public class Chart
    {
        [Template("new Chart({0}, {1})")]
        public Chart(Context ctx, ChartConfig options)
        {

        }

        /// <summary>
        /// Triggers an update of the chart. This can be safely called after replacing the entire data object. This will update all scales, legends, and then re-render the chart.
        /// </summary>
        public extern void Update();

        /// <summary>
        /// Use this to destroy any chart instances that are created. This will clean up any references stored to the chart object within Chart.js, along with any associated event listeners attached by Chart.js. This must be called before the canvas is reused for a new chart.
        /// </summary>
        public extern void Distroy();

        /// <summary>
        /// Triggers an update of the chart. This can be safely called after replacing the entire data object. This will update all scales, legends, and then re-render the chart.
        /// </summary>
        public extern void Update(int duration, bool lazy);

        /// <summary>
        /// Reset the chart to it's state before the initial animation. A new animation can then be triggered using update
        /// </summary>
        public extern void Reset();

        /// <summary>
        /// Triggers a redraw of all chart elements. Note, this does not update elements for new data. Use .update() in that case.
        /// </summary>
        /// <param name="duration">the time for the animation of the redraw in milliseconds</param>
        /// <param name="lazy">if true, the animation can be interrupted by other animations</param>
        public extern void Render(int duration, bool lazy);

        /// <summary>
        /// Use this to stop any current animation loop. This will pause the chart during any current animation frame. Call .render() to re-animate.
        /// </summary>
        public extern void Stop();

        /// <summary>
        /// Use this to manually resize the canvas element. This is run each time the canvas container is resized, but you can call this method manually if you change the size of the canvas nodes container element.
        /// </summary>
        public extern Chart Resize();

        /// <summary>
        /// Will clear the chart canvas. Used extensively internally between animation frames, but you might find it useful.
        /// </summary>
        /// <returns>this</returns>
        public extern Chart Clear();

        /// <summary>
        /// This returns a base 64 encoded string of the chart in it's current state
        /// </summary>
        /// <returns></returns>
        public extern string ToBase64Image();

        /// <summary>
        /// Returns an HTML string of a legend for that chart. The legend is generated from the legendCallback in the options.
        /// </summary>
        /// <returns></returns>
        public extern string GenerateLegend();

        /// <summary>
        /// Calling getElementAtEvent(event) on your Chart instance passing an argument of an event, or jQuery event, will return the single element at the event position. If there are multiple items within range, only the first is returned
        /// </summary>
        public extern HTMLElement GetElementAtEvent(Event ev);

        /// <summary>
        /// Looks for the element under the event point, then returns all elements at the same data index. This is used internally for 'label' mode highlighting. Calling getElementsAtEvent(event) on your Chart instance passing an argument of an event, or jQuery event, will return the point elements that are at that the same position of that event.
        /// </summary>
        public extern HTMLElement[] GetElementsAtEvent(Event ev);

        
        public DataConfig Data;

        [Template("Chart.defaults")]
        public static extern Defaults Defaults();
    }
}

namespace ChartJS
{
    [ObjectLiteral]
    [External]
    public class ChartConfig
    {
        public ChartType Type;
        public DataConfig Data;
        public ChartOptions Options;
        public bool PointDot;
        public int PointLabelFontSize;
    }
}

namespace ChartJS
{
    [ObjectLiteral]
    [External]
    public class ChartOptions
    {
        /// <summary>
        /// Resizes the chart canvas when its container does.
        /// </summary>
        public bool Responsive = true;
        /// <summary>
        /// Duration in milliseconds it takes to animate to new size after a resize event.
        /// </summary>
        public int ResponsiveAnimationDuration = 0;
        /// <summary>
        /// Maintain the original canvas aspect ratio (width / height) when resizing
        /// </summary>
        public bool MaintainAspectRatio = true;

        /// <summary>
        /// Events that the chart should listen to for tooltips and hovering
        /// </summary>
        public string[] Event = new string[] { "mousemove", "mouseout", "click", "touchstart", "touchmove", "touchend" };
        /// <summary>
        /// Called if the event is of type 'mouseup' or 'click'. Called in the context of the chart and passed an array of active elements
        /// </summary>
        public Action OnClick;
        /// <summary>
        /// Function to generate a legend. Receives the chart object to generate a legend from. Default implementation returns an HTML string.
        /// </summary>
        public Func<Chart, string> LegendCallback;

        public Action<Chart, object> OnResize;
        /// <summary>
        /// Legend configuration
        /// </summary>
        public LegendConfig Legend;
        /// <summary>
        /// Title configuration
        /// </summary>
        public TitleConfig Title;
        /// <summary>
        /// Layout config
        /// </summary>
        public LayoutConfig Layout;
        /// <summary>
        /// Tooltip config
        /// </summary>
        public TooltipConfig Tooltips;
        /// <summary>
        /// Hover configuration 
        /// </summary>
        public HoverConfig Hover;
        /// <summary>
        /// Animation configurations
        /// </summary>
        public AnimationConfig Animation;
        /// <summary>
        /// Elements configuration
        /// </summary>
        public ElementConfig Elements;
        /// <summary>
        /// Scales configuration
        /// </summary>
        public Scales Scales;
    }


    [ObjectLiteral]
    [External]
    public class Scales
    {
        [Name("xAxis")]
        public ScaleConfig[] XAxis;

        [Name("yAxis")]
        public ScaleConfig[] YAxis;
    }
}
namespace ChartJS
{
    [Enum(Emit.StringName)]
    [External]
    public enum ChartType
    {
        Line,
        Bar,
        HorizontalBar,
        Radar,
        Bubble,
        Pie, 
        Doughnut,
        PolarArea
    }
}
namespace ChartJS
{
    [ObjectLiteral]
    [External]
    public class DataConfig
    {
        public Dataset[] Datasets;
        public string[] Labels;
        public string[] XLabels;
        public string[] YLabels;
    }
}

namespace ChartJS
{
    [ObjectLiteral]
    [External]
    public class Dataset
    {

    }
}

namespace ChartJS
{
    [ObjectLiteral]
    [External]
    public class Defaults
    {
        /// <summary>
        /// options defined for every chart
        /// </summary>
        public ChartOptions Global;
    }
}
namespace ChartJS
{
    [ObjectLiteral]
    [External]
    public class LineDataPoint
    {
        public object X;
        public object Y;
    }
}
namespace ChartJS
{
    [Enum(Emit.StringName)]
    [External]
    public enum PointSyles
    {
        Circle,
        Triangle,
        Rect,
        [Name("rectRot")]
        RectRot,
        Cross,
        [Name("crossRot")]
        CrossRot,
        Star,
        Line,
        Dash
    }
}

namespace ChartJS
{
    [ObjectLiteral]
    [External]
    public class AnimationConfig
    {
        /// <summary>
        /// The number in milliseconds an animation takes.
        /// </summary>
        public int Duration;
        /// <summary>
        /// Easing function to use. Available options are: 'linear', 'easeInQuad', 'easeOutQuad', 'easeInOutQuad', 'easeInCubic', 'easeOutCubic', 'easeInOutCubic', 'easeInQuart', 'easeOutQuart', 'easeInOutQuart', 'easeInQuint', 'easeOutQuint', 'easeInOutQuint', 'easeInSine', 'easeOutSine', 'easeInOutSine', 'easeInExpo', 'easeOutExpo', 'easeInOutExpo', 'easeInCirc', 'easeOutCirc', 'easeInOutCirc', 'easeInElastic', 'easeOutElastic', 'easeInOutElastic', 'easeInBack', 'easeOutBack', 'easeInOutBack', 'easeInBounce', 'easeOutBounce', 'easeInOutBounce'.
        /// </summary>
        public string Easing;
        /// <summary>
        /// Callback called on each step of an animation. 
        /// </summary>
        public Action<AnimationDetails> OnProgress;
        /// <summary>
        /// Callback called at the end of an animation
        /// </summary>
        public Action<AnimationDetails> OnComplete;
    }

    [ObjectLiteral]
    [External]
    public class AnimationDetails
    {
        public Chart ChartInstance;
        /// <summary>
        /// Contains details of the on-going animation
        /// </summary>
        public AnimationObject AnimationObject;
    }

    [ObjectLiteral]
    [External]
    public class AnimationObject
    {
        /// <summary>
        /// Current Animation frame number
        /// </summary>
        public double CurrentStep;
        /// <summary>
        /// Number of animation frames
        /// </summary>
        public double NumSteps;
        /// <summary>
        /// Animation easing to use
        /// </summary>
        public string Easing;
        /// <summary>
        /// Function that renders the chart
        /// </summary>
        public dynamic Render;
        /// <summary>
        /// User callback
        /// </summary>
        public dynamic OnAnimationProgress;
        /// <summary>
        /// User callback
        /// </summary>
        public dynamic OnAnimationComplete;
    }
}
namespace ChartJS
{
    [ObjectLiteral]
    [External]
    public class ElementConfig
    {
        /// <summary>
        /// Arcs are used in the polar area, doughnut and pie charts
        /// </summary>
        public ArcConfig Arc;
        /// <summary>
        /// Line elements are used to represent the line in a line chart
        /// </summary>
        public LineConfig Line;
        /// <summary>
        /// Point elements are used to represent the points in a line chart or a bubble chart. 
        /// </summary>
        public PointConfig Point;
        /// <summary>
        /// Rectangle elements are used to represent the bars in a bar chart
        /// </summary>
        public RectangleConfig Rectangle;
    }
}

namespace ChartJS
{
    [ObjectLiteral]
    [External]
    public class HoverConfig
    {
        /// <summary>
        /// Sets which elements appear in the tooltip
        /// </summary>
        public IntersectionMode Mode;

        /// <summary>
        /// if true, the hover mode only applies when the mouse position intersects an item on the chart
        /// </summary>
        public bool Intersect;

        /// <summary>
        /// Duration in milliseconds it takes to animate hover style changes
        /// </summary>
        public int AnimationDuration;

        /// <summary>
        /// Called when any of the events fire. Called in the context of the chart and passed an array of active elements (bars, points, etc)
        /// </summary>
        public Action<object[]> OnHover;
    }


    [Enum(Emit.StringName)]
    [External]
    public enum IntersectionMode
    {
        /// <summary>
        /// Finds all of the items that intersect the point
        /// </summary>
        Point,
        /// <summary>
        /// Gets the item that is nearest to the point. The nearest item is determined based on the distance to the center of the chart item (point, bar). If 2 or more items are at the same distance, the one with the smallest area is used. If intersect is true, this is only triggered when the mouse position intersects an item in the graph. This is very useful for combo charts where points are hidden behind bars.
        /// </summary>
        Nearest,
        /// <summary>
        /// Finds item at the same index. If the intersect setting is true, the first intersecting item is used to determine the index in the data. If intersect false the nearest item is used to determine the index. 
        /// </summary>
        Index,
        /// <summary>
        /// Finds items in the same dataset. If the intersect setting is true, the first intersecting item is used to determine the index in the data. If intersect false the nearest item is used to determine the index.
        /// </summary>
        Dataset,
        /// <summary>
        /// Returns all items that would intersect based on the X coordinate of the position only. Would be useful for a vertical cursor implementation. Note that this only applies to cartesian charts
        /// </summary>
        X,
        /// <summary>
        /// Returns all items that would intersect based on the Y coordinate of the position. This would be useful for a horizontal cursor implementation. Note that this only applies to cartesian charts.
        /// </summary>
        Y
    }
}
namespace ChartJS
{
    [ObjectLiteral]
    [External]
    public class LayoutPadding
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
    }
    
    [ObjectLiteral]
    [External]
    public class LayoutConfig
    {

        /// <summary>
        /// The padding to add inside the chart. If this value is a number, it is applied to all sides of the chart (left, top, right, bottom). If this value is an object, the left property defines the left padding. Similarly the right, top, and bottom properties can also be specified.
        /// </summary>
        public Union<int, LayoutPadding> Padding = 0;
    }
}

namespace ChartJS
{
    [ObjectLiteral]
    [External]
    public class LegendConfig
    {
        /// <summary>
        /// Is the legend displayed, default is true
        /// </summary>
        public bool Display = true;
        /// <summary>
        /// Position of the legend. 
        /// </summary>
        public Position Position = Position.Top;
        /// <summary>
        /// Marks that this box should take the full width of the canvas (pushing down other boxes)
        /// </summary>
        public bool FullWidth = true;
        /// <summary>
        /// Legend will show datasets in reverse order
        /// </summary>
        public bool Reverse = false;
        /// <summary>
        /// A callback that is called when a 'click' event is registered on top of a label item
        /// </summary>
        public Action<Event, object> OnClick;
        /// <summary>
        /// A callback that is called when a 'mousemove' event is registered on top of a label item
        /// </summary>
        public Action<Event, object> OnHover;
        /// <summary>
        /// label configuration of legend
        /// </summary>
        public LegendLabelConfig Labels;

    }
}

namespace ChartJS
{
    [ObjectLiteral]
    [External]
    public class LegendLabelConfig
    {
        /// <summary>
        /// Width of coloured box
        /// </summary>
        public int BoxWidth = 40;

        /// <summary>
        /// Font size inherited from global configuration
        /// </summary>
        public int FontSize = 12;
        /// <summary>
        /// Font style inherited from global configuration
        /// </summary>
        public string FontStyle = "normal";
        /// <summary>
        /// Font color inherited from global configuration
        /// </summary>
        public string FontColor = "#666";
        /// <summary>
        /// "'Helvetica Neue', 'Helvetica', 'Arial', sans-serif"
        /// </summary>
        public string FontFamily = "'Helvetica Neue', 'Helvetica', 'Arial', sans-serif";
        /// <summary>
        /// Padding between rows of colored boxes
        /// </summary>
        public int Padding = 10;
        /// <summary>
        /// Generates legend items for each thing in the legend. Default implementation returns the text + styling for the color box
        /// </summary>
        public Action<Chart> GenerateLabels;
        /// <summary>
        /// Label style will match corresponding point style (size is based on fontSize, boxWidth is not used in this case).
        /// </summary>
        public bool UsePointStyle = false;
    }
}

namespace ChartJS
{
    [ObjectLiteral]
    [External]
    public class ScaleConfig
    {
        /// <summary>
        /// Type of scale being employed.
        /// </summary>
        public ScaleType Scale;
        /// <summary>
        /// If true, show the scale including gridlines, ticks, and labels. Overrides gridLines.display, scaleLabel.display, and ticks.display.
        /// </summary>
        public bool Display;
        /// <summary>
        /// Position of the scale
        /// </summary>
        public Position Position;
        /// <summary>
        /// The ID is used to link datasets and scale axes together. The properties datasets.xAxisID or datasets.yAxisID have to match the scale properties scales.xAxes.id or scales.yAxes.id. This is especially needed if multi-axes charts are used.
        /// </summary>
        public string Id;
        /// <summary>
        /// Callback called before the update process starts
        /// </summary>
        public Action<ScaleConfig> BeforeUpdate;
        /// <summary>
        /// Callback that runs before dimensions are set
        /// </summary>
        public Action<ScaleConfig> BeforeSetDimensions;
        /// <summary>
        /// Callback that runs after dimensions are set. 
        /// </summary>
        public Action<ScaleConfig> AfterSetDimensions;
        /// <summary>
        /// Callback that runs before data limits are determined
        /// </summary>
        public Action<ScaleConfig> BeforeDataLimits;
        /// <summary>
        /// Callback that runs after data limits are determined
        /// </summary>
        public Action<ScaleConfig> AfterDataLimits;
        /// <summary>
        /// Callback that runs before ticks are created
        /// </summary>
        public Action<ScaleConfig> BeforeBuildTicks;
        /// <summary>
        /// Callback that runs after ticks are created
        /// </summary>
        public Action<ScaleConfig> AfterBuildTicks;
        /// <summary>
        /// Callback that runs before ticks are converted into strings. Passed a single argument, the scale instance.
        /// </summary>
        public Action<ScaleConfig> BeforeTickToLabelConversion;
        /// <summary>
        /// Callback that runs after ticks are converted into strings
        /// </summary>
        public Action<ScaleConfig> AfterTickToLabelConversion;
        /// <summary>
        /// Callback that runs before tick rotation is determined
        /// </summary>
        public Action<ScaleConfig> BeforeCalculateTickRotation;
        /// <summary>
        /// Callback that runs after tick rotation is determined
        /// </summary>
        public Action<ScaleConfig> AfterCalculateTickRotation;
        /// <summary>
        /// Callback that runs before the scale fits to the canvas
        /// </summary>
        public Action<ScaleConfig> BeforeFit;
        /// <summary>
        /// Callback that runs after the scale fits to the canvas
        /// </summary>
        public Action<ScaleConfig> AfterFit;
        /// <summary>
        /// Callback that runs at the end of the update process
        /// </summary>
        public Action<ScaleConfig> AfterUpdate;
        /// <summary>
        /// Ticks configurations
        /// </summary>
        public TicksConfig Ticks;
        /// <summary>
        /// Time configurations
        /// </summary>
        public TimeConfig Time;
        /// <summary>
        /// If true, circular arcs are used else straight lines are used. The former is used by the polar area chart and the latter by the radar chart
        /// </summary>
        public bool LineArc;
        /// <summary>
        /// Config for angle lines, The following options are used to configure angled lines that radiate from the center of the chart to the point labels. They can be found in the angleLines sub options. Note that these options only apply if lineArc is false.
        /// </summary>
        public AngleLineConfig AngleLines;
        /// <summary>
        /// The following options are used to configure the point labels that are shown on the perimeter of the scale. They can be found in the pointLabels sub options. Note that these options only apply if lineArc is false.
        /// </summary>
        public PointLabelConfig PointLabels;



    }

    [Enum(Emit.StringName)]
    [External]
    public enum ScaleType
    {
        Category,
        Linear,
        Logarithmic,
        Time,
        RadialLinear
    }
}

namespace ChartJS
{
    [ObjectLiteral]
    [External]
    public class TimeConfig
    {
        public DiplayFormatConfig DisplayFormats;
        /// <summary>
        /// If true and the unit is set to 'week', iso weekdays will be used.
        /// </summary>
        public bool IsoWeekday;
        /// <summary>
        /// The moment js format string to use for the tooltip.
        /// </summary>
        public bool TooltipFormat;
        /// <summary>
        /// If defined, will force the unit to be a certain type. See Time Units section below for details.
        /// </summary>
        public TimeUnit Unit;
        /// <summary>
        /// The minimum display format to be used for a time unit
        /// </summary>
        public TimeUnit MinUnit;
        /// <summary>
        /// The number of units between grid lines.
        /// </summary>
        public Union<int, double> UnitStepSize;
        /// <summary>
        /// If defined, dates will be rounded to the start of this unit. See Time Units below for the allowed units.
        /// </summary>
        public string Round;
    }

    [Enum(Emit.StringName)]
    [External]
    public enum TimeUnit
    {
        Millisecond,
        Second,
        Minute,
        Hour,
        Day,
        Week,
        Month,
        Quarter,
        Year
    }
}

namespace ChartJS
{
    using Color = Union<string, CanvasGradient, CanvasPattern, string[], CanvasGradient[], CanvasPattern[]>;

    [Enum(Emit.StringName)]
    [External]
    public enum Position
    {
        Top,
        Left,
        Right,
        Bottom
    }

    

    [ObjectLiteral]
    [External]
    public class TitleConfig
    {
        /// <summary>
        /// Display the title block
        /// </summary>
        public bool Display = false;
        /// <summary>
        /// Position of the title.
        /// </summary>
        public Position Position = Position.Top;
        /// <summary>
        /// Marks that this box should take the full width of the canvas (pushing down other boxes)
        /// </summary>
        public bool FullWidth = true;
        /// <summary>
        /// Font size inherited from global configuration
        /// </summary>
        public int FontSize = 12;
        /// <summary>
        /// Font family inherited from global configuration
        /// </summary>
        public string FontFamily = "'Helvetica Neue', 'Helvetica', 'Arial', sans-serif";
        /// <summary>
        /// Font color 
        /// </summary>
        public Color FontColor = "#666";
        /// <summary>
        /// Font style, default is 'bold'
        /// </summary>
        public string FontStyle = "bold";
        /// <summary>
        /// Number of pixels to add above and below the title text
        /// </summary>
        public int Padding = 10;
        /// <summary>
        /// Title text
        /// </summary>
        public string Text = "";
    }
}

namespace ChartJS
{

    using Color = Union<string, CanvasGradient, CanvasPattern, string[], CanvasGradient[], CanvasPattern[]>;

    using TooltipCallback = Action<TooltipItem[], DataConfig>;

    using Strings = Union<string, string[]>;

    [ObjectLiteral]
    [External]
    public class TooltipConfig
    {
        /// <summary>
        /// Are tooltips enabled
        /// </summary>
        public bool Enabled = true;
        /// <summary>
        /// Sets which elements appear in the tooltip. 
        /// </summary>
        public IntersectionMode Mode = IntersectionMode.Nearest;
        /// <summary>
        /// if true, the tooltip mode applies only when the mouse position intersects with an element. If false, the mode will be applied at all times.
        /// </summary>
        public bool Intersect = true;
        /// <summary>
        /// The mode for positioning the tooltip. 'average' mode will place the tooltip at the average position of the items displayed in the tooltip. 'nearest' will place the tooltip at the position of the element closest to the event position. New modes can be defined by adding functions to the Chart.Tooltip.positioners map.
        /// </summary>
        public string Position = "average";
        /// <summary>
        /// Allows sorting of tooltip items. Must implement at minimum a function that can be passed to Array.prototype.sort. This function can also accept a third parameter that is the data object passed to the chart
        /// </summary>
        public object ItemSort;
        /// <summary>
        /// Allows filtering of tooltip items. Must implement at minimum a function that can be passed to Array.prototype.filter. This function can also accept a second parameter that is the data object passed to the chart.
        /// </summary>
        public object Filter;
        /// <summary>
        /// Background color of the tooltip
        /// </summary>
        public Color BackgroundColor = "rgba(0,0,0,0.8)";
        /// <summary>
        /// Font family for tooltip title inherited from global font family
        /// </summary>
        public string TitleFontFamily;
        /// <summary>
        /// Font size for tooltip title inherited from global font size
        /// </summary>
        public int TitleFontSize = 12;

        public string TitleFontStyle;

        public Color TitleFontColor;

        /// <summary>
        /// Spacing to add to top and bottom of each title line.
        /// </summary>
        public int TitleSpacing = 2;
        /// <summary>
        /// Margin to add on bottom of title section
        /// </summary>
        public int TitleMarginBottom = 6;

        public string BodyFontFamily;

        public int BodyFontSize = 12;

        public string BodyFontStyle;

        public Color BodyFontColor;

        public int BodySpacing = 2;

        public string FooterFontFamily;

        public int FooterFontSize = 12;

        public string FooterFontStyle;

        public Color FooterFontColor;

        public int FooterSpacing = 2;

        /// <summary>
        /// Padding to add on left and right of tooltip
        /// </summary>
        public int XPadding = 6;

        /// <summary>
        /// Padding to add on top and bottom of tooltip
        /// </summary>
        public int YPadding = 6;

        /// <summary>
        /// Size, in px, of the tooltip arrow
        /// </summary>
        public int CaretSize = 5;

        /// <summary>
        /// Radius of tooltip corner curves
        /// </summary>
        public int CornerRadius = 6;

        /// <summary>
        /// Color to draw behind the colored boxes when multiple items are in the tooltip
        /// </summary>
        public Color MultiKeyBackground;

        /// <summary>
        /// if true, color boxes are shown in the tooltip
        /// </summary>
        public bool DisplayColors;

        public TooltipCallbacks Callbacks;
    }


    [ObjectLiteral]
    [External]
    public class TooltipCallbacks
    {
        /// <summary>
        /// Text to render before the title
        /// </summary>
        public Func<TooltipItem[], DataConfig, Strings> BeforeTitle;
        /// <summary>
        /// Text to render as the title
        /// </summary>
        public Func<TooltipItem[], DataConfig, Strings> Title;
        /// <summary>
        /// Text to render after the title
        /// </summary>
        public Func<TooltipItem[], DataConfig, Strings> AfterTitle;
        /// <summary>
        /// Text to render before the body section
        /// </summary>
        public Func<TooltipItem[], DataConfig, Strings> BeforeBody;
        /// <summary>
        /// Text to render before an individual label
        /// </summary>
        public Func<TooltipItem, DataConfig, Strings> BeforeLabel;
        /// <summary>
        /// Text to render for an individual item in the tooltip
        /// </summary>
        public Func<TooltipItem, DataConfig, Strings> Label;
        /// <summary>
        /// Returns the colors to render for the tooltip item. Return as an object containing two parameters: borderColor and backgroundColor.
        /// </summary>
        public Func<TooltipItem, Chart, LabelColorObject> LabelColor;
        /// <summary>
        /// Text to render after an individual label
        /// </summary>
        public Func<TooltipItem, DataConfig, Strings> AfterLabel;
        /// <summary>
        /// Text to render after the body section
        /// </summary>
        public Func<TooltipItem[], DataConfig, Strings> AfterBody;
        /// <summary>
        /// Text to render before the footer section
        /// </summary>
        public Func<TooltipItem[], DataConfig, Strings> BeforeFooter;
        /// <summary>
        /// Text to render as the footer
        /// </summary>
        public Func<TooltipItem[], DataConfig, Strings> Footer;
        /// <summary>
        /// Text to render after the footer section
        /// </summary>
        public Func<TooltipItem[], DataConfig, Strings> AfterFooter;
        /// <summary>
        /// List of matching point informations
        /// </summary>
        public Func<TooltipItem[], Strings> DataPoints;


    }


    [ObjectLiteral]
    [External]
    public class LabelColorObject
    {
        public Color BorderColor;
        public Color BackgroundColor;
    }


    [ObjectLiteral]
    [External]
    public class TooltipItem
    {
        /// <summary>
        /// X Value of the tooltip as a string
        /// </summary>
        public string xLabel;
        /// <summary>
        /// Y value of the tooltip as a string
        /// </summary>
        public string yLabel;
        /// <summary>
        /// Index of the dataset the item comes from
        /// </summary>
        public int DatasetIndex;
        /// <summary>
        /// Index of this data item in the dataset
        /// </summary>
        public int Index;
        /// <summary>
        /// X position of matching point
        /// </summary>
        public int x;
        /// <summary>
        /// Y position of matching point
        /// </summary>
        public int y;
    }
}

namespace ChartJS
{
    using Color = Union<string, CanvasGradient, CanvasPattern, string[], CanvasGradient[], CanvasPattern[]>;
    /// <summary>
    /// Arcs are used in the polar area, doughnut and pie charts
    /// </summary>
    [ObjectLiteral]
    [External]
    public class ArcConfig
    {
        /// <summary>
        /// Default fill color for arcs. Inherited from the global default
        /// </summary>
        public Color BackgroundColor;
        /// <summary>
        /// Default stroke color for arcs
        /// </summary>
        public Color BorderColor;
        /// <summary>
        /// Default stroke width for arcs
        /// </summary>
        public int BorderWidth;
    }
}

namespace ChartJS
{
    using Color = Union<string, CanvasGradient, CanvasPattern, string[], CanvasGradient[], CanvasPattern[]>;
    using Number = Union<int, double>;

    [ObjectLiteral]
    [External]
    public class LineConfig
    {
        /// <summary>
        /// Default bezier curve tension. Set to 0 for no bezier curves.
        /// </summary>
        public Number Tension;
        /// <summary>
        /// Default line fill color
        /// </summary>
        public Color BackgroundColor;
        /// <summary>
        /// Default line stroke width
        /// </summary>
        public int BorderWidth;

        /// <summary>
        /// Default line stroke color
        /// </summary>
        public Color BorderColor;
        /// <summary>
        /// Default line cap style
        /// </summary>
        public string BorderCapStyle;
        /// <summary>
        /// Default line dash
        /// </summary>
        public int[] BorderDash;
        /// <summary>
        /// Default line dash offset
        /// </summary>
        public double BorderDashOffset;
        /// <summary>
        /// Default line join style
        /// </summary>
        public string BorderJoinStyle;
        /// <summary>
        /// If true, bezier control points are kept inside the chart. If false, no restriction is enforced.
        /// </summary>
        public bool CapBezierPoints;
        /// <summary>
        /// If true, the fill is assumed to be to zero. String values are 'zero', 'top', and 'bottom' to fill to different locations. If false, no fill is added
        /// </summary>
        public Union<string, bool> Fill;
        /// <summary>
        /// If true, the line is shown as a stepped line and 'tension' will be ignored
        /// </summary>
        public bool Stepped;
    }
}


namespace ChartJS
{
    using Color = Union<string, CanvasGradient, CanvasPattern, string[], CanvasGradient[], CanvasPattern[]>;

    [ObjectLiteral]
    [External]
    public class PointConfig
    {
        /// <summary>
        /// Default point radius
        /// </summary>
        public int Radius;
        /// <summary>
        /// Default point style
        /// </summary>
        public PointSyles PointStyle;
        /// <summary>
        /// Default line fill color
        /// </summary>
        public Color BackgroundColor;
        /// <summary>
        /// Default line stroke width
        /// </summary>
        public int BorderWidth;
        /// <summary>
        /// Default line stroke color
        /// </summary>
        public Color BorderColor;
        /// <summary>
        /// Extra radius added to point radius for hit detection
        /// </summary>
        public int HitRadius;
        /// <summary>
        /// Default point radius when hovered
        /// </summary>
        public int HoverRadius;
        /// <summary>
        /// Default stroke width when hovered
        /// </summary>
        public int HoverBorderWidth;

    }
}


namespace ChartJS
{
    using Color = Union<string, CanvasGradient, CanvasPattern, string[], CanvasGradient[], CanvasPattern[]>;

    [ObjectLiteral]
    [External]
    public class RectangleConfig
    {
        /// <summary>
        /// Default line fill color
        /// </summary>
        public Color BackgroundColor;
        /// <summary>
        /// Default line stroke width
        /// </summary>
        public int BorderWidth;
        /// <summary>
        /// Default line stroke color
        /// </summary>
        public Color BorderColor;
        /// <summary>
        /// Default skipped (excluded) border for rectangle.
        /// </summary>
        public BorderSkippedPattern BorderSkipped;
    }

    [Enum(Emit.StringName)]
    [External]
    public enum BorderSkippedPattern
    {
        Top,
        Bottom,
        Left,
        Right
    }
}


namespace ChartJS
{
    using Number = Union<int, double>;
    using Color = Union<string, CanvasGradient, CanvasPattern, string[], CanvasGradient[], CanvasPattern[]>;

    [ObjectLiteral]
    [External]
    public class AngleLineConfig
    {
        /// <summary>
        /// If true, angle lines are shown.
        /// </summary>
        public bool Display;
        /// <summary>
        /// Color of angled lines
        /// </summary>
        public Color Color;
        /// <summary>
        /// Width of angled lines
        /// </summary>
        public Number LineWidth;
    }
}
namespace ChartJS
{
    [ObjectLiteral]
    [External]
    public class DiplayFormatConfig
    {
        public string Millisecond;
        public string Second;
        public string Minute;
        public string Hour;
        public string Day;
        public string Week;
        public string Month;
        public string Year;
        public string Quarter;
    }
}

namespace ChartJS
{
    using Color = Union<string, CanvasGradient, CanvasPattern, string[], CanvasGradient[], CanvasPattern[]>;

    [ObjectLiteral]
    [External]
    public class PointLabelConfig
    {
        /// <summary>
        /// Font size in pixels
        /// </summary>
        public int FontSize;
        /// <summary>
        /// Font Style to use
        /// </summary>
        public string FontStyle;
        /// <summary>
        /// Font family to render
        /// </summary>
        public string FontFamily;
        /// <summary>
        /// Font color
        /// </summary>
        public Color FontColor;
        /// <summary>
        /// Callback function to transform data label to axis label
        /// </summary>
        public dynamic Callback;
    }
}

namespace ChartJS
{
    using Color = Union<string, CanvasGradient, CanvasPattern, string[], CanvasGradient[], CanvasPattern[]>;
    using Number = Union<int, double>;

    [ObjectLiteral]
    [External]
    public class TicksConfig
    {
        /// <summary>
        /// If true, automatically calculates how many labels that can be shown and hides labels accordingly. Turn it off to show all labels no matter what
        /// </summary>
        public bool AutoSkip;
        /// <summary>
        /// Padding between the ticks on the horizontal axis when autoSkip is enabled. Note: Only applicable to horizontal scales.
        /// </summary>
        public int AutoSkipPadding;
        /// <summary>
        /// If true, show the ticks.
        /// </summary>
        public bool Display;
        /// <summary>
        /// Font color for the tick labels
        /// </summary>
        public Color FontColor;
        /// <summary>
        /// Font family for the tick labels, follows CSS font-family options.
        /// </summary>
        public string FontFamily;
        /// <summary>
        /// Font size for the tick labels.
        /// </summary>
        public int FontSize;
        /// <summary>
        /// Distance in pixels to offset the label from the centre point of the tick (in the y direction for the x axis, and the x direction for the y axis). Note: this can cause labels at the edges to be cropped by the edge of the canvas
        /// </summary>
        public int LabelOffset;
        /// <summary>
        /// Maximum rotation for tick labels when rotating to condense labels. Note: Rotation doesn't occur until necessary. Note: Only applicable to horizontal scales.
        /// </summary>
        public Union<int, double, float> MaxRotation;
        /// <summary>
        /// Minimum rotation for tick labels. Note: Only applicable to horizontal scales.
        /// </summary>
        public Union<int, double, float> MinRotation;
        /// <summary>
        /// Flips tick labels around axis, displaying the labels inside the chart instead of outside. Note: Only applicable to vertical scales.
        /// </summary>
        public bool Mirror;
        /// <summary>
        /// Padding between the tick label and the axis. Note: Only applicable to horizontal scales.
        /// </summary>
        public int Padding;
        /// <summary>
        /// Reverses order of tick labels.
        /// </summary>
        public bool Reverse;
        /// <summary>
        /// if true, scale will include 0 if it is not already included.
        /// </summary>
        public bool BeginAtZero;
        /// <summary>
        /// User defined minimum number for the scale, overrides minimum value from data.
        /// </summary>
        public Number Min;
        /// <summary>
        /// User defined maximum number for the scale, overrides maximum value from data.
        /// </summary>
        public Number Max;
        /// <summary>
        /// Maximum number of ticks and gridlines to show. If not defined, it will limit to 11 ticks but will show all gridlines.
        /// </summary>
        public int MaxTicksLimit;
        /// <summary>
        /// User defined fixed step size for the scale. If set, the scale ticks will be enumerated by multiple of stepSize, having one tick per increment. If not set, the ticks are labeled automatically using the nice numbers algorithm.
        /// </summary>
        public Number FixedStepSize;
        /// <summary>
        /// if defined, it can be used along with the min and the max to give a custom number of steps. See the example below.
        /// </summary>
        public Number StepSize;
        /// <summary>
        /// User defined maximum number for the scale, overrides maximum value except for if it is lower than the maximum value.
        /// </summary>
        public Number SuggestedMax;
        /// <summary>
        /// User defined minimum number for the scale, overrides minimum value except for if it is higher than the minimum value.
        /// </summary>
        public Number SuggestedMin;
        /// <summary>
        /// Color of label backdrops
        /// </summary>
        public Color BackdropColor;
        /// <summary>
        /// Horizontal padding of label backdrop
        /// </summary>
        [Name("backdropPaddingX")]
        public Number BackdropPaddingX;
        /// <summary>
        /// Vertical padding of label backdrop
        /// </summary>
        [Name("backdropPaddingY")]
        public Number BackdropPaddingY;
        /// <summary>
        /// If true, draw a background behind the tick labels
        /// </summary>
        public bool ShowLabelBackdrop;
    }
}

namespace ChartJS
{
    using Number = Union<int, double>;
    using NumberOrNumbers = Union<int, double, int[], double[]>;

    using Color = Union<string, CanvasGradient, CanvasPattern, string[], CanvasGradient[], CanvasPattern[]>;

    [ObjectLiteral]
    [External]
    public class BarDataset : Dataset
    {
        public Union<int[], double[], decimal[]> Data;
        /// <summary>
        /// The label for the dataset which appears in the legend and tooltips
        /// </summary>
        public string Label;
        /// <summary>
        /// The ID of the x axis to plot this dataset on
        /// </summary>
        [Name("xAxisID")]
        public string XAxisId;
        /// <summary>
        /// The ID of the y axis to plot this dataset on
        /// </summary>
        [Name("yAxisID")]
        public string YAxisId;
        /// <summary>
        /// The fill color under the line
        /// </summary>
        public Color BackgroundColor;
        /// <summary>
        /// Which edge to skip drawing the border for. Options are 'bottom', 'left', 'top', and 'right'
        /// </summary>
        public Union<string, string[]> BorderSkipped;

        /// <summary>
        /// The color of the line.
        /// </summary>
        public Color BorderColor;
        /// <summary>
        /// Point background color when hovered
        /// </summary>
        public Color HoverBackgroundColor;

        /// <summary>
        /// Border width of bar in pixels
        /// </summary>
        public NumberOrNumbers BorderWidth;
        /// <summary>
        /// border color when hovered
        /// </summary>
        public Color HoverBorderColor;
        /// <summary>
        /// Border width when hovered
        /// </summary>
        public Number HoverBorderWidth;
    }
}

namespace ChartJS
{

    using Number = Union<int, double, float, byte>;

    using NumberOrNumbers = Union<int, double, int[], double[]>;

    using Color = Union<string, CanvasGradient, CanvasPattern, string[], CanvasGradient[], CanvasPattern[]>;

    [ObjectLiteral]
    [External]
    public class BubbleDataset : Dataset
    {
        public BubbleDataPoint[] Data;
        /// <summary>
        /// The label for the dataset which appears in the legend and tooltips
        /// </summary>
        public string Label;
        /// <summary>
        /// The fill color under the line
        /// </summary>
        public Color BackgroundColor;
        /// <summary>
        /// The color of the line.
        /// </summary>
        public Color BorderColor;
        /// <summary>
        /// Point background color when hovered
        /// </summary>
        public Color HoverBackgroundColor;
        /// <summary>
        /// Border width of bar in pixels
        /// </summary>
        public NumberOrNumbers BorderWidth;
        /// <summary>
        /// border color when hovered
        /// </summary>
        public Color HoverBorderColor;
        /// <summary>
        /// Border width when hovered
        /// </summary>
        public NumberOrNumbers HoverBorderWidth;
        /// <summary>
        /// Additional radius to add to data radius on hover
        /// </summary>
        public NumberOrNumbers HoverRadius;
    }


    [ObjectLiteral]
    [External]
    public class BubbleDataPoint
    {
        [Name("x")]
        public Number X;
        [Name("y")]
        public Number Y;
        /// <summary>
        /// Radius of the bubble
        /// </summary>
        [Name("r")]
        public Number Radius; 
    }
}

namespace ChartJS
{
    using Number = Union<int, double>;
    using NumberOrNumbers = Union<int, double, int[], double[]>;

    using Color = Union<string, CanvasGradient, CanvasPattern, string[], CanvasGradient[], CanvasPattern[]>;

    [ObjectLiteral]
    [External]
    public class LineDataset : Dataset
    {
        public Union<int[], double[], decimal[], LineDataPoint[]> Data;
        /// <summary>
        /// The label for the dataset which appears in the legend and tooltips
        /// </summary>
        public string Label;
        /// <summary>
        /// If true, fill the area under the line
        /// </summary>
        public bool Fill;
        /// <summary>
        /// Bezier curve tension of the line. Set to 0 to draw straightlines. This option is ignored if monotone cubic interpolation is used. Note This was renamed from 'tension' but the old name still works.
        /// </summary>
        public Number LineTension;
        /// <summary>
        /// The ID of the x axis to plot this dataset on
        /// </summary>
        [Name("xAxisID")]
        public string XAxisId;
        /// <summary>
        /// The ID of the y axis to plot this dataset on
        /// </summary>
        [Name("yAxisID")]
        public string YAxisId;
        /// <summary>
        /// Algorithm used to interpolate a smooth curve from the discrete data points. Options are 'default' and 'monotone'. The 'default' algorithm uses a custom weighted cubic interpolation, which produces pleasant curves for all types of datasets. The 'monotone' algorithm is more suited to y = f(x) datasets : it preserves monotonicity (or piecewise monotonicity) of the dataset being interpolated, and ensures local extremums (if any) stay at input data points. If left untouched (undefined), the global options.elements.line.cubicInterpolationMode property is used.
        /// </summary>
        public string CubicInterpolationMode;
        /// <summary>
        /// The fill color under the line
        /// </summary>
        public Color BackgroundColor;
        /// <summary>
        /// The color of the line.
        /// </summary>
        public Color BorderColor;
        /// <summary>
        /// Cap style of the line
        /// </summary>
        public string BorderCapStyle;
        /// <summary>
        /// Length and spacing of dashes. 
        /// </summary>
        public NumberOrNumbers BorderDash;
        /// <summary>
        /// Offset for line dashes
        /// </summary>
        public Number BorderDashOffset;
        /// <summary>
        /// Line joint style
        /// </summary>
        public string BorderJoinStyle;
        /// <summary>
        /// The border color for points.
        /// </summary>
        public Color PointBorderColor;
        /// <summary>
        /// The fill color for points
        /// </summary>
        public Color PointBackgroundColor;
        /// <summary>
        /// The width of the point border in pixels
        /// </summary>
        public Color PointBorderWidth;
        /// <summary>
        /// The radius of the point shape. If set to 0, nothing is rendered. 
        /// </summary>
        public Number PointRadius;
        /// <summary>
        /// The radius of the point when hovered
        /// </summary>
        public Number PointHoverRadius;
        /// <summary>
        /// The pixel size of the non-displayed point that reacts to mouse events
        /// </summary>
        public Number PointHitRadius;
        /// <summary>
        /// Point background color when hovered
        /// </summary>
        public Color PointHoverBackgroundColor;
        /// <summary>
        /// Point border color when hovered
        /// </summary>
        public Color PointHoverBorderColor;
        /// <summary>
        /// Border width of point when hovered
        /// </summary>
        public Number PointHoverBorderWidth;
        /// <summary>
        /// The style of point. Options are 'circle', 'triangle', 'rect', 'rectRot', 'cross', 'crossRot', 'star', 'line', and 'dash'. If the option is an image, that image is drawn on the canvas using drawImage. 
        /// </summary>
        public Union<PointSyles, PointSyles[]> PointStyle;
        /// <summary>
        /// The style of point. Options are 'circle', 'triangle', 'rect', 'rectRot', 'cross', 'crossRot', 'star', 'line', and 'dash'. If the option is an image, that image is drawn on the canvas using drawImage. 
        /// </summary>
        [Name("pointStyle")]
        public Union<ImageData, ImageData[]> PointSyleAsImage;
        /// <summary>
        /// If false, the line is not drawn for this dataset
        /// </summary>
        public bool ShowLine;
        /// <summary>
        /// If true, lines will be drawn between points with no or null data
        /// </summary>
        public bool ShowGaps;
        /// <summary>
        /// If true, the line is shown as a stepped line and 'lineTension' will be ignored
        /// </summary>
        public bool SteppedLine;
    }
}

namespace ChartJS
{
    using NumberOrNumbers = Union<int, double, int[], double[]>;

    using Color = Union<string, CanvasGradient, CanvasPattern, string[], CanvasGradient[], CanvasPattern[]>;

    [ObjectLiteral]
    [External]
    public class PieOrDoughnutDataset : Dataset
    {
        public Union<int[], double[], decimal[]> Data;
        /// <summary>
        /// The label for the dataset which appears in the legend and tooltips
        /// </summary>
        public string Label;
        /// <summary>
        /// The fill color under the line
        /// </summary>
        public Color BackgroundColor;
        /// <summary>
        /// The color of the line.
        /// </summary>
        public Color BorderColor;
        /// <summary>
        /// Point background color when hovered
        /// </summary>
        public Color HoverBackgroundColor;

        /// <summary>
        /// Border width of bar in pixels
        /// </summary>
        public NumberOrNumbers BorderWidth;
        /// <summary>
        /// border color when hovered
        /// </summary>
        public Color HoverBorderColor;
        /// <summary>
        /// Border width when hovered
        /// </summary>
        public NumberOrNumbers HoverBorderWidth;
    }
}

namespace ChartJS
{
    using Number = Union<int, double>;
    using NumberOrNumbers = Union<int, double, int[], double[]>;

    using Color = Union<string, CanvasGradient, CanvasPattern, string[], CanvasGradient[], CanvasPattern[]>;

    [ObjectLiteral]
    [External]
    public class PolarAreaDataset : Dataset
    {
        public Union<int[], double[], decimal[]> Data;
        /// <summary>
        /// The label for the dataset which appears in the legend and tooltips
        /// </summary>
        public string Label;
        /// <summary>
        /// The fill color under the line
        /// </summary>
        public Color BackgroundColor;
        /// <summary>
        /// The color of the line.
        /// </summary>
        public Color BorderColor;
        /// <summary>
        /// Point background color when hovered
        /// </summary>
        public Color HoverBackgroundColor;

        /// <summary>
        /// Border width of bar in pixels
        /// </summary>
        public NumberOrNumbers BorderWidth;
        /// <summary>
        /// border color when hovered
        /// </summary>
        public Color HoverBorderColor;
        /// <summary>
        /// Border width when hovered
        /// </summary>
        public NumberOrNumbers HoverBorderWidth;
    }
}

namespace ChartJS
{
    using Number = Union<int, double>;
    using NumberOrNumbers = Union<int, double, int[], double[]>;

    using Color = Union<string, CanvasGradient, CanvasPattern, string[], CanvasGradient[], CanvasPattern[]>;

    [ObjectLiteral]
    [External]
    public class RadarDataset : Dataset
    {
        public Union<int[], double[], decimal[]> Data;
        /// <summary>
        /// The label for the dataset which appears in the legend and tooltips
        /// </summary>
        public string Label;
        /// <summary>
        /// If true, fill the area under the line
        /// </summary>
        public bool Fill;
        /// <summary>
        /// Bezier curve tension of the line. Set to 0 to draw straightlines. This option is ignored if monotone cubic interpolation is used. Note This was renamed from 'tension' but the old name still works.
        /// </summary>
        public Number LineTension;
        /// <summary>
        /// Algorithm used to interpolate a smooth curve from the discrete data points. Options are 'default' and 'monotone'. The 'default' algorithm uses a custom weighted cubic interpolation, which produces pleasant curves for all types of datasets. The 'monotone' algorithm is more suited to y = f(x) datasets : it preserves monotonicity (or piecewise monotonicity) of the dataset being interpolated, and ensures local extremums (if any) stay at input data points. If left untouched (undefined), the global options.elements.line.cubicInterpolationMode property is used.
        /// </summary>
        public string CubicInterpolationMode;
        /// <summary>
        /// The fill color under the line
        /// </summary>
        public Color BackgroundColor;
        /// <summary>
        /// The color of the line.
        /// </summary>
        public Color BorderColor;
        /// <summary>
        /// Cap style of the line
        /// </summary>
        public string BorderCapStyle;
        /// <summary>
        /// Length and spacing of dashes. 
        /// </summary>
        public NumberOrNumbers BorderDash;
        /// <summary>
        /// Offset for line dashes
        /// </summary>
        public Number BorderDashOffset;
        /// <summary>
        /// Line joint style
        /// </summary>
        public string BorderJoinStyle;
        /// <summary>
        /// The border color for points.
        /// </summary>
        public Color PointBorderColor;
        /// <summary>
        /// The pixel size of the non-displayed point that reacts to mouse events
        /// </summary>
        public NumberOrNumbers HitRadius;
        /// <summary>
        /// The fill color for points
        /// </summary>
        public Color PointBackgroundColor;
        /// <summary>
        /// The width of the point border in pixels
        /// </summary>
        public Color PointBorderWidth;
        /// <summary>
        /// The radius of the point shape. If set to 0, nothing is rendered. 
        /// </summary>
        public Number PointRadius;
        /// <summary>
        /// The radius of the point when hovered
        /// </summary>
        public Number PointHoverRadius;
        /// <summary>
        /// The pixel size of the non-displayed point that reacts to mouse events
        /// </summary>
        public Number PointHitRadius;
        /// <summary>
        /// Point background color when hovered
        /// </summary>
        public Color PointHoverBackgroundColor;
        /// <summary>
        /// Point border color when hovered
        /// </summary>
        public Color PointHoverBorderColor;
        /// <summary>
        /// Border width of point when hovered
        /// </summary>
        public Number PointHoverBorderWidth;
        /// <summary>
        /// The style of point. Options are 'circle', 'triangle', 'rect', 'rectRot', 'cross', 'crossRot', 'star', 'line', and 'dash'. If the option is an image, that image is drawn on the canvas using drawImage. 
        /// </summary>
        public Union<PointSyles, PointSyles[]> PointStyle;
        /// <summary>
        /// The style of point. Options are 'circle', 'triangle', 'rect', 'rectRot', 'cross', 'crossRot', 'star', 'line', and 'dash'. If the option is an image, that image is drawn on the canvas using drawImage. 
        /// </summary>
        [Name("pointStyle")]
        public Union<ImageData, ImageData[]> PointSyleAsImage;
    }
}