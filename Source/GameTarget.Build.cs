using Flax.Build;

public class GameTarget : GameProjectTarget
{
    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        // Reference the modules for game
        Modules.Add("Game");
        Modules.Add("TrenchRun");
        Modules.Add("Game");
    }
}
