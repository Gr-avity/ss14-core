using System.Linq;
using Content.Shared.GameTicking.Prototypes;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Graphics.RSI;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client._Art.UI.AnimatedBackground;
public sealed class AnimatedBackgroundControl : TextureRect
{
    [Dependency] private readonly IResourceCache _resourceCache = default!;
    [Dependency] private readonly IClyde _clyde = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    private string _rsiPath = "/Textures/_Art/Lobby/space-hole.rsi";
    public RSI? _RSI;
    private const int States = 1;
    private IRenderTexture? _buffer;
    private readonly float[] _timer = new float[States];
    private readonly float[][] _frameDelays = new float[States][];
    private readonly int[] _frameCounter = new int[States];
    private readonly Texture[][] _frames = new Texture[States][];
    public AnimatedBackgroundControl()
    {
        IoCManager.InjectDependencies(this);
        InitializeStates();
    }
    private void InitializeStates()
    {
        if (_RSI == null)
        {
            try
            {
                _RSI = _resourceCache.GetResource<RSIResource>(_rsiPath).RSI;
            }
            catch
            {
                return;
            }
        }

        if (_RSI == null) return;

        for (var i = 0; i < States; i++)
        {
            if (!_RSI.TryGetState((i + 1).ToString(), out var state))
            {
                _frames[i] = Array.Empty<Texture>();
                _frameDelays[i] = Array.Empty<float>();
                continue;
            }
            _frames[i] = state.GetFrames(RsiDirection.South);
            _frameDelays[i] = state.GetDelays();
            _frameCounter[i] = 0;
        }
    }
    public void SetRSI(RSI? rsi)
    {
        _RSI = rsi;
        InitializeStates();
    }
    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);
        for (var i = 0; i < _frames.Length; i++)
        {
            var frames = _frames[i];
            var delays = _frameDelays[i];
            if (frames == null || delays == null || delays.Length == 0)
                continue;
            _timer[i] += args.DeltaSeconds;
            var currentFrameIndex = _frameCounter[i];
            if (!(_timer[i] >= delays[currentFrameIndex]))
                continue;
            _timer[i] -= delays[currentFrameIndex];
            _frameCounter[i] = (currentFrameIndex + 1) % frames.Length;
            Texture = frames[_frameCounter[i]];
        }
    }
    protected override void Draw(DrawingHandleScreen handle)
    {
        base.Draw(handle);
        if (_buffer is null)
            return;
        handle.DrawTextureRect(_buffer.Texture, PixelSizeBox);
    }
    protected override void Resized()
    {
        base.Resized();
        _buffer?.Dispose();
        _buffer = _clyde.CreateRenderTarget(PixelSize, RenderTargetColorFormat.Rgba8Srgb);
    }
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _buffer?.Dispose();
    }
    public void RandomizeBackground()
    {
        var backgroundsProto = _prototypeManager.EnumeratePrototypes<LobbyBackgroundPrototype>().Where(x => x.Background.Extension == "rsi").ToList();
        if (backgroundsProto.Count == 0) return;
        var random = new Random();
        var index = random.Next(backgroundsProto.Count);
        _rsiPath = backgroundsProto[index].Background.ToString();
        _RSI = null;
        InitializeStates();
    }
}
