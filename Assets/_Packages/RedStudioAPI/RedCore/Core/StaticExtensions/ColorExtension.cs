using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtension
{

    public static Color SetRed(this Color @this, float newRed) => new Color(newRed, @this.g, @this.b, @this.a);
    public static Color SetGreen(this Color @this, float newGreen) => new Color(@this.r, newGreen, @this.b, @this.a);
    public static Color SetBlue(this Color @this, float newBlue) => new Color(@this.r, @this.g, newBlue, @this.a);
    public static Color SetAlpha(this Color @this, float newAlpha) => new Color(@this.r, @this.g, @this.b, newAlpha);

}
