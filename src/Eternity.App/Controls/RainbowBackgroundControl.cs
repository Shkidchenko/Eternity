using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace Eternity.App.Controls;

/// <summary>Animated rainbow blend background with optional disable flag for performance.</summary>
public sealed class RainbowBackgroundControl : Control
{
    /// <summary>Defines whether animation is enabled.</summary>
    public static readonly StyledProperty<bool> IsAnimationEnabledProperty =
        AvaloniaProperty.Register<RainbowBackgroundControl, bool>(nameof(IsAnimationEnabled), true);

    private readonly DispatcherTimer _timer;
    private double _offset;

    /// <summary>Initializes a new instance.</summary>
    public RainbowBackgroundControl()
    {
        _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(30), DispatcherPriority.Background, (_, _) =>
        {
            if (IsAnimationEnabled)
            {
                _offset += 0.01;
                InvalidateVisual();
            }
        });
        _timer.Start();
    }

    /// <summary>Gets or sets animation enablement.</summary>
    public bool IsAnimationEnabled
    {
        get => GetValue(IsAnimationEnabledProperty);
        set => SetValue(IsAnimationEnabledProperty, value);
    }

    /// <inheritdoc />
    public override void Render(DrawingContext context)
    {
        var b = Bounds;
        var gradient = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(Math.Sin(_offset) * 0.5 + 0.5, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(1, Math.Cos(_offset) * 0.5 + 0.5, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Color.Parse("#803B82F6"), 0),
                new GradientStop(Color.Parse("#806366F1"), 0.35),
                new GradientStop(Color.Parse("#80EC4899"), 0.7),
                new GradientStop(Color.Parse("#80F59E0B"), 1)
            }
        };

        context.FillRectangle(gradient, b);
    }
}
