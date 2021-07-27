using System;
public interface IWorldOverlayable
{
    // Set to overlay mode -- Creation canvas
    public void SetOverlayMode();

    // Set to world space mode -- Creation canvas
    public void SetWorldMode();

    // Clears active menu canvas and deals with camera resets
    public void ClearActiveMenuCanvas();
}
